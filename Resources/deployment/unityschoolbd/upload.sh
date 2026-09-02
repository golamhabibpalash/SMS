#!/bin/bash
# ============================================================
# Upload EIMS publish/  ->  eims.unityschoolbd.com (Plesk FTP)
#
# Uses lftp mirror (parallel, resumable) for the 12k+ files.
# Credentials are read from ENV VARS only — never hardcode them
# in this file or commit them.
#
# Usage:
#   export FTP_HOST="104.234.134.230"
#   export FTP_USER="unitySchoolBD"
#   export FTP_PASS="..."
#   export FTP_TARGET_DIR="/eims.unityschoolbd.com/httpdocs"   # docroot
#   bash Resources/deployment/unityschoolbd/upload.sh [publish-fd|publish]
#
# The optional arg selects the local source folder (default: publish-fd).
# Set CLEAN=1 to also delete remote files that are not in the local source.
# ============================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SRC="${1:-publish-fd}"
PUBLISH_DIR="$SCRIPT_DIR/$SRC"

FTP_HOST="${FTP_HOST:?Set FTP_HOST (e.g. 104.234.134.230)}"
FTP_USER="${FTP_USER:?Set FTP_USER}"
FTP_PASS="${FTP_PASS:?Set FTP_PASS}"
# Home dir for this FTP account is "/", so targets are absolute.
FTP_TARGET_DIR="${FTP_TARGET_DIR:-/eims.unityschoolbd.com/httpdocs}"

if ! command -v lftp >/dev/null 2>&1; then
    echo "ERROR: lftp is required (brew install lftp)." >&2
    exit 1
fi

if [ ! -d "$PUBLISH_DIR" ]; then
    echo "ERROR: publish dir not found: $PUBLISH_DIR" >&2
    echo "Run publish-windows.sh first." >&2
    exit 1
fi

echo "[INFO] Source   : $PUBLISH_DIR"
echo "[INFO] Target   : ftp://$FTP_HOST$FTP_TARGET_DIR"

# 1) Ensure the remote docroot exists
echo "[INFO] Ensuring remote docroot exists..."
lftp -u "$FTP_USER","$FTP_PASS" "ftp://$FTP_HOST" -e "mkdir -p '$FTP_TARGET_DIR'; quit" 2>/dev/null || true

# 2) Put the app offline so IIS stops the app during upload
echo "[INFO] Taking site offline (app_offline.htm)..."
printf '<html><body>maintenance</body></html>' > "$PUBLISH_DIR/app_offline.htm"

# 3) Mirror the publish contents up
echo "[INFO] Uploading $(find "$PUBLISH_DIR" -type f | wc -l | tr -d ' ') files..."
if [ "${CLEAN:-0}" = "1" ]; then
  echo "[INFO] Clean mirror enabled (deleting remote files not in source)..."
  MIRROR_OPTS="--continue --parallel=8 --delete --no-empty-dirs"
else
  MIRROR_OPTS="--continue --parallel=8 --no-empty-dirs"
fi
lftp -u "$FTP_USER","$FTP_PASS" "ftp://$FTP_HOST" <<EOF
set net:timeout 30
set net:max-retries 5
set net:reconnect-interval-base 5
set ftp:ssl-allow no
set pget:default-n 4
lcd "$PUBLISH_DIR"
cd "$FTP_TARGET_DIR"
mirror $MIRROR_OPTS . .
quit
EOF

# 4) Remove app_offline so the site comes back up
echo "[INFO] Bringing site back online..."
lftp -u "$FTP_USER","$FTP_PASS" "ftp://$FTP_HOST" -e "rm '$FTP_TARGET_DIR/app_offline.htm'; quit" 2>/dev/null || true
rm -f "$PUBLISH_DIR/app_offline.htm"

echo ""
echo "[OK] Upload complete."
echo "    Next: ensure write perms on httpdocs, httpdocs/logs, httpdocs/wwwroot,"
echo "          then browse https://eims.unityschoolbd.com"
