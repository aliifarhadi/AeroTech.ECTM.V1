# STAGE ACCEPTANCE — P02 / Parameters, typed derived parameters, recorder profile (backend)

## Source lock
- **Backend:** `AeroTech.ECTM.V1`, uncommitted on top of `cf126e3`. It contains P00, P01 and P02.
- **Legacy references read:** see GAP-ANALYSIS §2 and `docs/legacy-notes/data-profile-and-catalog.md`.
- **Migration:** `AddConfigurationLifecycle` adds 6 nullable columns and alters nothing. It is applied to `DotAirEctmV1`: 3 migrations, verified on every throwaway test database, with no pending model change.
- **Report, screen and route IDs:** R-P05 and R-P06 data, S9 and S9a, and 10 new operations (33 in total).

## Independent scientific contract
See SCIENTIFIC-CONTRACT.md.
- 46 hand-calculated vectors over 4 evaluable kinds.
- THETA and DELTA are CANDIDATE.
- Vibration stays UNVERIFIED.
- No OEM coefficient anywhere.

## Reproducible E2E
- **Dataset:** `tests/fixtures/real/2026-10-01_EP-DOA_24.csv` (SHA-256 `ae6290…8718`, 3987 rows) for the profile header and the dry-run.
- **Owner path (J2):** `Flows/ParserSetupWalkthroughTests`:
  1. Catalog and profile active, so readiness reports `parserReady=true`.
  2. New catalog version from the active one, then add `EGT_OVER_N2`, then validate.
  3. Evaluate on typed inputs: `EGT_C[1] / N2_PCT[1] = 600 / 96 = 6.25`.
  4. Publish with a note; it becomes the active version.
  5. Diff: one `ADDED $.derivedParameters[4]`.
  6. Editing the published version → 409 80106. The parser stays ready.
- **Development database `DotAirEctmV1`:**
  - The seed drafts were created at startup (catalog v1 DRAFT, profile v1 DRAFT).
  - Readiness: `parserReady=false`, missing DATA_PROFILE and PARAMETER_CATALOG (`evidence/readiness-before-publish.json`).
  - **Nothing was published on the owner's behalf;** publishing is the owner's J2 step.
- **Negative paths, all tested:**
  - collision naming the first ordinal
  - swapped UTC occurrences
  - unknown unit, unit mismatch, conversion other than IDENTITY
  - duplicated or missing ordinal
  - publish without a required channel
  - derived parameter as a recorder column
  - field outside the schema
  - unknown or later-phase kind, `formula` field, forward reference, exponent outside a CORRECTION_MODEL
  - catalog breaking the active profile
  - publish without a note, or with issues
  - retire a draft
  - clone from another kind
  - non-UTF-8 file
  - dry-run of a catalog
  - swapped columns in a file
- **Audit:** publishing writes an UPDATED entry holding only the status, timestamp, actor and note, without the 40 KB content.
- **401/403:** NOT DELIVERED (security deferred by the owner).
- **Tests:** `evidence/conformance-tests.txt` 144/144 and `evidence/acceptance-tests.txt` 67/67. The throwaway databases were dropped.

## Delta inventory
| Kind | Added in P02 |
|---|---|
| Domain concepts | registry life cycle (publish, retire, draft-only edit) on ConfigurationDocument; Computation: catalog and profile content models, validators, typed evaluator, profile previewer, JSON diff; CsvRows (ported) |
| Seeds | `parameter-catalog-ep-doa-seed.json` (64 parameters, 4 typed derived), `data-profile-ep-doa-seed.json` (85 columns), both embedded |
| Tables | none; 6 columns on ConfigurationDocuments |
| Endpoints | 10: draft, edit, validate, publish, retire, dry-run, diff, typed kinds, typed evaluate, meta/readiness |
| Tests | 99 new conformance tests, 17 new acceptance tests |

## Deviations
- **Carried over:** Consumers kept, Framework envelope, Ordering query pattern, no security.
- **New in P02:**
  - ALT_RATE moved to P05 (one implementation, docs/16 §5).
  - The typed-kind list handler reads static data directly, without a service.
  - Error code 80114 is reserved and unused.

## Stage closure
- **`SOFTWARE_VERIFIED`:** PASS for the delivered scope (211/211 tests). Security remains deferred.
- **`ENGINEERING_APPROVED`:** PENDING. The catalog roles and units, THETA and DELTA, and the vibration channels await engineer review.
- **`OPERATIONAL_STATUS`:** parser configuration AVAILABLE once the owner publishes the two seed drafts. Vibration stays `UNVERIFIED_UNIT`.
- **Open owner decisions and inputs:**
  1. **Publish the catalog, then the profile, in J2.** I can do it on your instruction.
  2. **I-02:** the recorder frame manual (units of columns 24, 31–38, 46, 83; meaning of 25–30).
  3. **THETA and DELTA:** the OEM reference definitions.
  4. **Engine model of Eng1S1 and Eng2S2:** still missing (I-01).
  5. **Still open from P00:** fixture publication; package vulnerability.
- **Owner signature and gate to start P03:**  A
