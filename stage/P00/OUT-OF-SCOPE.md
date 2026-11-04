# P00 — Out of scope

| Item | Where it goes |
|---|---|
| creating, editing, validating, dry-running, publishing or retiring configuration documents; per-kind schemas and evaluators | with the first kind that needs them: P02 (DATA_PROFILE, PARAMETER_CATALOG, typed derived parameters), then P04, P05 and later phases |
| `config/typed-definitions/kinds` and `config/typed-definitions/evaluate` (typed-definition playground) | P02, because evaluators are deferred and docs/16 forbids building ahead |
| all security: fallback policy, Viewer/Engineer/Approver policies, service-level checks, 401/403 tests | deferred by the owner on 2026-11-05 until further notice |
| job runner, processing jobs, inbox, outbox, message retention | P03, if the import needs it |
| `meta/enums`, `meta/readiness` | not needed: enums are returned as `EnumValueDto`. Readiness comes in P15. |
| fleet, import, quality, phases and every later aggregate | P01..P16 |
| frontend shell, auth shell, generated client, S9 empty states | frontend repository `AeroTech.ECTM.Backoffice.V1` |
| CI pipeline changes, Docker, Kubernetes, IdentityServer | owner (infrastructure and identity are outside the agent's scope) |
| independent fixtures (`fixtures/independent/*`) | copied in P04 and P06, when they are first used |
