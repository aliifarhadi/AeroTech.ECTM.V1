# STAGE ACCEPTANCE — P00 / Foundations (backend)

## Source lock
- **New backend repo / commit:** `aliifarhadi/AeroTech.ECTM.V1`, branch `k8s-stg`. The skeleton is commit `cf126e3`. The P00 changes are **uncommitted** and wait for the owner's instruction.
- **New frontend repo / commit:** `AeroTech.ECTM.Backoffice.V1`. Not part of this delivery.
- **Legacy source references read:** `AeroTech.ECTM@0ad5f93` and `Aerotech.ECTM.Backoffice@fd01cb5`, both clean. The paths read are in GAP-ANALYSIS §3, `docs/legacy-notes/framework-host.md` and `docs/legacy-notes/test-harness.md`.
- **Database migration and rollback check:**
  - `20261104221458_InitialCreate` creates `Ectm.ConfigurationDocuments` and `Ectm.AuditEntries`.
  - It is applied to the new Development database `DotAirEctmV1` on `localhost\SQLEXPRESS`.
  - Every acceptance run migrates a throwaway database and drops it. `HasPendingModelChanges` is false.
  - The legacy `DotAirEctm` was not touched.
- **Related report / screen IDs and API routes:** S9 Configuration home and S13 Audit. Routes: `GET config/kinds`, `GET config/{kind}/documents`, `GET config/{kind}/documents/{documentId}` and `GET audit`.

## Independent scientific contract
NOT_APPLICABLE (SCIENTIFIC-CONTRACT.md). P00 computes no engineering value.

## Reproducible E2E
- **Dataset(s):** `tests/fixtures/real` (8 CSVs) and `tests/fixtures/golden` (9 JSON files).
  - All 17 SHA-256 values equal `tests/fixtures/MANIFEST.sha256`, which is taken from the pack manifest.
  - Each CSV equals its golden `identity.sha256`.
  - No ESN identity is involved.
- **Owner clicks (backend, real host in Development):**
  - `GET /Backoffice/v1/config/kinds` → 200 with 9 kinds, each `activeVersion=null`, `activeVersionReason=NO_ACTIVE_VERSION`, counts 0 (`evidence/config-kinds.json`).
  - `GET config/DATA_PROFILE/documents` → 200, an empty page.
  - `GET audit` → 200, an empty page.
  - Runtime Swagger has exactly the 4 routes (`evidence/swagger-paths.json`).
- **Actual values and trace:** not applicable.
- **Negative paths:**
  - Unknown kind → 404 code 80100.
  - Lower-case, numeric or comma-joined kind → 404 code 80100.
  - Document under the wrong kind → 404 code 80101.
  - Unknown id → 404 code 80101.
  - Page size 0 or 101, or page number 0 → 400.
  - Audit `from` > `to` → 400.
- **401 and 403 evidence for all routes:** **NOT DELIVERED.** On 2026-11-05 the owner said to write no security, 401 or similar code for now. Every route is open.
- **Recompute, immutability and determinism:** not applicable. P00 has no computation. The content hash is deterministic and checked against a SHA-256 computed outside the product.
- **Tests from a clean clone with no external file paths:**
  - Need only `ECTM_TEST_SQLSERVER`, or `appsettings.Development.json`.
  - An anti-regression scan confirms there is no absolute path in `tests/` and no recorder ordinal in `src/`.
  - Logs: `evidence/conformance-tests.txt` (17/17) and `evidence/acceptance-tests.txt` (23/23).
  - Caveat: `tests/fixtures/real` and `golden` are excluded from git (see open decision 1), so a clean clone of the public repo will not have them yet.
- **Report deep link:** not applicable.

## Delta inventory (docs/16 §5)
| Kind | Added |
|---|---|
| Domain concepts | `ConfigurationDocument` (draft creation, content hash), `AuditEntry` |
| Enums (Contracts) | `ConfigurationKind` (9), `ConfigurationDocumentStatus` (3), `AuditAction` (3) |
| Tables | `Ectm.ConfigurationDocuments`, `Ectm.AuditEntries` |
| Endpoints | 4 GET routes |
| UI pages | none (frontend repository) |
| Tests | 17 conformance, 23 acceptance |

## Deviations from the pack (owner rulings)
- **Consumers:** `AeroTech.ECTM.Consumers` with RabbitMQ is kept, though nothing consumes yet (docs/16 §0).
- **Envelope:** Framework `ApiResult` instead of the docs/13 envelope. The correlation id goes in the `X-Correlation-Id` header.
- **Query pattern:** each query has a Backoffice query, a handler and a service, following the Ordering pattern rather than the docs/16 §0 sentence on trivial reads.
- **Security:** none in P00, so the exit criterion "anonymous 401" and docs/20 are not met.

## Stage closure
- **`SOFTWARE_VERIFIED`:** PASS for the delivered scope; build is clean and all 40 tests pass. **FAIL against the full P00 exit criteria**, because the security criterion was deferred by the owner. Reviewer: owner. Date: 2026-11-05.
- **`ENGINEERING_APPROVED`:** NOT_APPLICABLE.
- **`OPERATIONAL_STATUS`:** OTHER. Foundations only; there is no functional feature yet.
- **Open owner decisions:**
  1. **Fixture publication.** `tests/fixtures/real` and `golden` are real EP-DOA recordings and derived values. They are kept out of git, because the GitHub repo may be public. The pack requires them inside the repo so tests can run from a clean clone. Decide whether to publish them, or to make the repo private and publish them.
  2. **When security is resumed.** The design is ready in memory: Ordering surface policy, `ectm.viewer`/`engineer`/`approver`, and fallback deny.
  3. **Package vulnerabilities.** `System.Security.Cryptography.Pkcs` 9.0.4 is flagged (NU1902/NU1903) through the Framework's package references. This is infrastructure, so it is for the owner.
  4. **docs/22 product decisions.** None is needed before P01. I-01 (fleet registrations, ESNs, installations) is needed for P01; without it, P01 uses test-labelled ESNs.
- **Deferred later-phase work:** see OUT-OF-SCOPE.md.
- **Owner signature and gate to start P01:** ______________________
