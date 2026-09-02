#!/bin/bash
# ============================================================
# Publish EIMS for unityschoolbd.com — Windows shared hosting (Plesk + IIS)
# FRAMEWORK-DEPENDENT win-x64. Requires the server to have the
# ASP.NET Core 8 Hosting Bundle / runtime installed by the host.
# This produces a much smaller package than the self-contained one.
# Run from your Mac:  bash Resources/deployment/unityschoolbd/publish-windows-fd.sh
# ============================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../../.." && pwd)"
RELEASE_DIR="$SCRIPT_DIR/publish-fd"

BUILD_NUMBER="${BUILD_NUMBER:-0}"
GIT_COMMIT="${GIT_COMMIT:-local}"
BUILT_UTC="$(date -u '+%Y-%m-%d %H:%M:%S')"

echo "[INFO] Repo root : $REPO_ROOT"
echo "[INFO] Output    : $RELEASE_DIR"
echo "[INFO] Build     : $GIT_COMMIT @ $BUILT_UTC"

rm -rf "$RELEASE_DIR"

dotnet publish "$REPO_ROOT/SMS_App/SMS_App.csproj" \
    --configuration Release \
    --output "$RELEASE_DIR" \
    --runtime win-x64 \
    --self-contained false \
    -p:PublishSingleFile=false \
    -p:PublishTrimmed=false \
    -p:BuildNumber="$BUILD_NUMBER" \
    -p:GitCommit="$GIT_COMMIT" \
    -p:BuildTimestampUtc="$BUILT_UTC"

echo ""
echo "[OK] Framework-dependent publish complete. Next steps:"
echo "   1. Copy the production web.config (in-process) into the publish folder:"
echo "        cp Resources/deployment/unityschoolbd/web.config publish-fd/web.config"
echo "   2. Upload publish-fd/ contents to /eims.unityschoolbd.com/httpdocs"
echo "   3. Ensure the host has the .NET 8 Hosting Bundle installed and the"
echo "      subdomain is running ASP.NET Core (in-process)."
