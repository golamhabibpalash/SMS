# CI/CD — one branch, one build, every instance

`.github/workflows/deploy.yml` builds the app **once** and deploys the identical
package to every instance. Instances are GitHub **Environments**; they differ only
by their secrets/variables — never by branch or by code.

```
push to FinalDeployment  ->  deploys ALL instances
Actions -> Deploy EIMS -> Run workflow -> pick one  ->  deploys just that instance
```

## One-time setup

### 1. Create an Environment per instance

**Repo → Settings → Environments → New environment.** Names must match exactly:

| Environment      | Site URL                              | Host   |
|------------------|--------------------------------------|--------|
| `Noble`          | https://eims.nobleschoolbd.com        | Linux  |
| `Mamitonnesa`    | https://eims.mamitonnessaghs.edu.bd   | Linux  |
| `DMCPS`          | https://eims.mcpsbd.com               | Linux  |
| `Unityschoolbd`  | https://eims.unityschoolbd.com        | IIS    |
| `Curiosity`      | https://curiosity.mcpsbd.com          | Linux  |

Add a new school by adding its Environment here **and** one line to the `client`
list in `setup` + the `workflow_dispatch` `options` in `deploy.yml`.

### 2. Per-environment secrets

| Secret         | Notes                                              |
|----------------|----------------------------------------------------|
| `FTP_SERVER`   | host only, no `ftp://`                              |
| `FTP_USERNAME` |                                                    |
| `FTP_PASSWORD` |                                                    |
| `FTP_PATH`     | web root on the server, e.g. `/httpdocs` or `/`     |
| `SSH_HOST`     | Linux hosts only — omit entirely on IIS            |
| `SSH_USER`     | Linux hosts only                                   |
| `SSH_KEY`      | Linux hosts only — full private key, PEM           |

If `SSH_*` is absent the restart step is skipped; on IIS, deleting `app_offline.htm`
recycles the app pool, which is enough.

### 3. Per-environment variables

| Variable       | Notes                                                       |
|----------------|------------------------------------------------------------|
| `SITE_URL`     | full URL for the post-deploy health check                   |
| `SERVICE_NAME` | optional; defaults to `eims-<client-lowercase>.service`     |

### 4. Migrating from the old per-client secrets

The previous workflow used repo secrets like `NOBLE_FTP_SERVER`, `MAMI_FTP_PASSWORD`,
`UNITY_FTP_PATH`, `DMCPS_SSH_KEY`, `CURIOSITY_*`. Copy each into the matching
Environment under the new short name and delete the old repo secret:

```
NOBLE_FTP_SERVER      -> Environment "Noble"         secret FTP_SERVER
MAMI_FTP_*            -> Environment "Mamitonnesa"   secret FTP_*
DMCPS_*              -> Environment "DMCPS"          secret *
UNITY_FTP_*           -> Environment "Unityschoolbd"  secret FTP_*   (no SSH_* — IIS)
CURIOSITY_*           -> Environment "Curiosity"      secret *
```

## What the pipeline never touches on a server

`appsettings*.json`, `Keys/` (data-protection), `logs/`, and uploaded images
(`wwwroot/Images/Student/Photo`, `wwwroot/Images/Employee/photo`,
`wwwroot/Images/Institute`). Everything else — DLLs, views, and the app's own
`wwwroot` assets (css/js/lib/fonts) — is deployed. The FTP mirror runs **without**
`--delete`, so excluded server files are safe; lftp only transfers files whose
size or timestamp changed, so the first deploy is large (~200 MB of `wwwroot/lib`)
and later ones push only what changed.

## Optional hardening

- **Approval gate per instance:** Environment → *Required reviewers*. That instance's
  deploy then waits for a click; the others proceed (`fail-fast: false`).
- **Staggered release:** run the workflow for one instance, confirm, then push to
  the branch (or run with `all`) for the rest.
- **Database migrations** are not run by CI. Apply EF migrations manually per
  instance when a release includes schema changes (SqlServer instances); the
  PostgreSQL instances are dump-provisioned.

## Rollback

Re-run the workflow from an earlier green commit (Actions → that run → *Re-run*),
or `git revert` on `FinalDeployment` and let it redeploy.
