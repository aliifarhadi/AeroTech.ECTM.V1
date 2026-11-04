# P00 — Test plan

## Three gates
- **Regression gate:** not applicable in P00. The goldens are copied and their integrity is verified; they are not compared with any product output.
- **Correctness gate:** not applicable in P00.
- **Approval gate:** not applicable in P00.

## How to run
- **Requirements:** a clean clone, the .NET 10 SDK and a SQL Server login that can create a database.
- **Connection string:** `ECTM_TEST_SQLSERVER=<connection string>`. Without it, `ConnectionStrings:CommandDbContext` is read from `src/AeroTech.ECTM.ServiceHost/appsettings.Development.json`.
- **Database:** the acceptance run creates `DotAirEctmV1_Test_<utc stamp>_<random>`, migrates it and drops it at the end. No shared database is written.
- **Paths:** every fixture is under `tests/fixtures`. No path outside the repository is read.

## Cases
| Area | Positive | Negative | Project |
|---|---|---|---|
| Route inventory | the exact set of business routes equals the P00 contract | an extra route fails the test | AcceptanceTests |
| Correlation | `X-Correlation-Id` is present on 200 and on 404 | — | AcceptanceTests |
| Configuration kinds | 9 kinds; each `NO_ACTIVE_VERSION`; counts reflect a seeded draft | — | AcceptanceTests |
| Configuration documents | list and detail of a seeded draft with content and hash | unknown kind → 404; document of another kind → 404; unknown id → 404; page size 0 → 400 | AcceptanceTests |
| Audit | creating a document writes one CREATED entry with actor, time, after-JSON and no before-JSON; the API filters by entity type and entity id | `from` > `to` → 400 | AcceptanceTests |
| Persistence | migrations applied; model has no pending changes | — | AcceptanceTests |
| Domain | draft has version, DRAFT status and lower-case SHA-256 of its content | empty, non-JSON, non-object content and version < 1 are refused | ConformanceTests |
| Fixture integrity | 17 fixture files equal `tests/fixtures/MANIFEST.sha256`; each real CSV equals its golden `identity.sha256` | — | ConformanceTests |
| Anti-regression scans (docs/16 §3a) | — | no source ordinal access in `src/`; no absolute or parent-escaping fixture path in `tests/` | ConformanceTests |

## Not covered by automated tests
- **Security (401/403, permissions):** deferred by the owner on 2026-11-05.
- **Frontend shell and login:** these belong to the frontend repository.
- **CI pipeline:** `.gitlab-ci.yml` is infrastructure and is outside the agent's scope.
