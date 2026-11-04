# P00 — Gap analysis

## 1. Source lock
| Repository | Expected | Local HEAD | Branch | Working tree |
|---|---|---|---|---|
| `aliifarhadi/AeroTech.ECTM` | `0ad5f93` | `0ad5f932e70cfb28a118c3a0cdfd4814598630a5` | `feature/trendalert-oem-closure` | clean |
| `aliifarhadi/Aerotech.ECTM.Backoffice` | `fd01cb5` | `fd01cb5177b8641c054d99a4f717302fd433fbe8` | `feature/trendalert-oem-closure` | clean |

Both match the source lock.

## 2. Pack verification
- **`verification/verify_pack.py`:** PASS (STATIC ONLY).
- **`verification/verify_v33.py`:** FAIL on one check, "manifest coverage of all files".
  - Cause: on Windows, `str(Path.relative_to())` produces `\` separators, while `MANIFEST.sha256` uses `/`.
  - Rechecked with POSIX paths: there is no file only in the manifest, no file only on disk, and no hash mismatch.
  - All other checks pass. This is a script defect, not a pack defect.

## 3. Legacy code read for P00, and the decision for each item
| Legacy path | Decision | Reason |
|---|---|---|
| `Framework/AeroTech.Framework.*` (already copied by the owner) | REUSE unchanged | approved framework: entity bases, `ApiResult` envelope, exception middleware, JWT, Snowflake ids, paging, health |
| `src/AeroTech.ECTM.ServiceHost/Program.cs` | RE-IMPLEMENT | same composition order. Legacy job, report and import registrations are dropped. |
| `src/AeroTech.ECTM.Persistence/EctmDbContext.cs` | RE-IMPLEMENT | keeps `Ectm` schema, migration history table, string length convention and concurrency mapping. All 30 legacy `DbSet`s are dropped. Audit capture is added. |
| `src/AeroTech.ECTM.Persistence/_Shared/PagingExtensions.cs` | PORT | unchanged helper |
| `src/AeroTech.ECTM.Application/_Shared/Behaviors/*` | PORT | logging and validation pipeline |
| `src/AeroTech.ECTM.Application/_Shared/Paging/*` | PORT | page limits and page validator |
| `src/AeroTech.ECTM.Application/_Shared/Enums/EnumValueDto.cs` | PORT | owner enum-display ruling |
| `src/AeroTech.ECTM.Application/_Shared/Events/*` | PORT | `CommandDbContext` requires an `IDomainEventDispatcher` |
| `src/AeroTech.ECTM.Domain/_Shared/Resources/ExceptionFactory.cs`, `ExceptionMessages.cs` | RE-IMPLEMENT | same shape. The code catalogue starts again at 80000 with only P00 codes. |
| `src/AeroTech.ECTM.RestApi/Swagger/*` | PORT | id-as-string schema, tag-by-aggregate. Title and description updated. |
| `src/AeroTech.ECTM.Consumers/DependencyInjection.cs`, `RabbitMqOptions.cs` | PORT, reduced | owner ruling 2026-11-05: keep Consumers. The bus registration is kept. Inbox filter, outbox publisher and pollers are not ported because nothing produces or consumes a message yet. |
| `tests/.../Fixtures/RestApiHost.cs`, `TestDatabase.cs`, `MigratedDatabaseFixture.cs`, `RepositoryPaths.cs`, `Fakes/FixedClock.cs`, `SequentialIdGenerator.cs` | RE-IMPLEMENT | disposable database per run, test authentication, no path outside the repository |
| `Ordering.Final/src/AeroTech.Ordering.RestApi/V1/_Shared/SurfaceAuthorization.cs` | DEFERRED | owner instruction 2026-11-05: no security, authorization or 401/403 code for now |
| every legacy aggregate (`*Aggregate/`), `Persistence/Inbox`, `Persistence/Outbox`, all 25 migrations, `RestApi/V1/*` | IGNORE | product domain. It belongs to later phases or is forbidden by docs/16 §3. |
| `Contracts/AeroTech.Messages/Ectm/Enums` (99 legacy enums) | IGNORE | the owner emptied the folder on 2026-11-05. P00 adds only its own three enums. |

No legacy analytics code is copied.

## 4. Pack items that conflict with owner rulings (recorded, not improvised)
| Pack text | Owner ruling (2026-11-05) | Effect |
|---|---|---|
| docs/16 §0: no RabbitMQ without a consumer | keep `AeroTech.ECTM.Consumers` | **Deviation.** The bus is registered with no consumers. |
| docs/13: envelope `{ok, data \| error, correlationId}` | keep Framework `ApiResult` `{data, errors[]}` | **Deviation.** The correlation id is returned in the `X-Correlation-Id` header. |
| docs/17 §5: a dedicated DB per run | accepted | Each acceptance run creates `DotAirEctmV1_Test_<stamp>` and drops it at the end. |
| docs/16 §0: no separate service + handler for every trivial read | Ordering pattern binds down to its details (standing rule) | **Deviation.** Each query has a Backoffice query, a handler and a service, as in Ordering. |
| P00 exit criterion "anonymous 401"; docs/20 fallback policy, 401/403 tests on every route | owner, 2026-11-05: no code for security, 401 and similar for now | **Deviation, blocking for docs/20.** The routes are open. The Framework JWT authentication stays registered, but no endpoint requires it. P00 cannot claim the security exit criterion. |

## 5. Decisions taken for small gaps
- **Kind codes:** routes take the kind code as an UPPER_SNAKE string, for example `config/DATA_PROFILE/documents`. An unknown code returns 404 with error code 80100. Lower case, numbers and comma-joined names are refused.
- **Active version:** the published document with the highest version number. Nothing can be published in P00, so every kind answers `NO_ACTIVE_VERSION`. CORRECTION_MODEL scoping per engine model arrives in P08.
- **Audit:** every added, modified or deleted aggregate is audited inside `EctmDbContext.SaveChanges`. Each entry records the actor, UTC time, entity type, entity id, action, before/after JSON and the correlation id. The audit entry itself is not audited.
- **Correlation id:** the W3C trace id of the request activity, also stored on audit entries.
- **Connection string for tests:** environment variable `ECTM_TEST_SQLSERVER`. When it is not set, `CommandDbContext` is read from `appsettings.Development.json`. A clean clone needs only that one value.
- **Settings:** the database name in `appsettings.Development.json` and `appsettings.Staging.json` was changed from `DotAirEctm` to `DotAirEctmV1`, so V1 never migrates the legacy database.

## 6. Delta inventory planned for P00
| Kind | Items |
|---|---|
| Domain concepts | `ConfigurationDocument`, `AuditEntry` |
| Tables | `Ectm.ConfigurationDocuments`, `Ectm.AuditEntries` |
| Endpoints | 4 business endpoints, see API-SCREEN-CONTRACT |
| UI pages | none in this repository (frontend repository) |
| Tests | route inventory, config read flow, audit, correlation header, migration consistency, fixture integrity, anti-regression scans, domain rules |
