# P02 — Gap analysis

## 1. Source lock
Legacy `AeroTech.ECTM@0ad5f93`, unchanged. V1 baseline: P00 + P01 (uncommitted on top of `cf126e3`).

## 2. Legacy code read, and the decision for each item
| Legacy path | Decision | Reason |
|---|---|---|
| `Domain/DataProfileAggregate/Profiles/ep-doa-profile-v1.json` | PORT as the DATA_PROFILE seed | Exact headers and ordinals are kept byte-for-byte; a test checks them against the real CSV. Codes, positions and units come from the pack catalog v2. |
| `Domain/DataProfileAggregate/DataProfiles.cs`, `DataProfile.cs`, `ProfileColumn.cs` | RE-IMPLEMENT | The profile is now a versioned registry document, not an embedded resource with a fixed loader. Signature check, occurrence binding and collision checks are added (docs/02 §1a). |
| `Domain/DataProfileAggregate/ParameterCatalog.cs`, `SourceChannelQualification*` | IGNORE | The catalog is the PARAMETER_CATALOG document (docs/03). Channel qualification comes with quality (P04). |
| `Application/ImportItemAggregate/Services/CsvRows.cs` | PORT | Now in `Domain/_Shared/Recording`, used by the profile dry-run. It returns 422 80112 instead of a custom exception. P03 reuses it. |
| `contracts/parameter-catalog-ep-doa-v1.json` (pack) | SEED with a transform | The 85 columns are grouped into 64 parameters with positions. Prose `derivedParameters` become typed definitions. ALT_RATE is left out (see §3). |
| `contracts/typed-definitions-v1.json` (pack) | IMPLEMENT | The closed kind list. 4 kinds are evaluable now; the other 6 are refused with their phase. |

## 3. Conflicts and their resolution
- **ALT_RATE_FPM:** the catalog contract lists it as a prose derived parameter. `contracts/phase-ruleset-runtime-v1.json` defines `ALT_RATE_FPM` / `ALT_RATE_SMOOTH_FPM` as typed derived channels of the PHASE_RULESET. Keeping both would derive the same value twice (docs/16 §5). It belongs to the phase rule set (P05), so the catalog seed holds `EGT_DIFF_E2_E1_C`, `TAT_K`, `THETA` and `DELTA` only.
- **"v2: AVM_UNITS for vibration" (PHASE-02.md):** the v3.3 focus note, docs/02 and docs/26 F06 keep recorder vibration unverified. The catalog unit is `RECORDER_VIBRATION_NATIVE_UNVERIFIED`; no AVM conversion is made.
- **R-P05 / R-P06:** same treatment as R-D5 in P01. The data is delivered here (`meta/readiness`, document diff, publish/retire who/when/note); the report screens come in P11.

## 4. Decisions taken for small gaps
- **Catalog shape:** parameters (code, role, unit, valueKind, positions, plausibility, note, OEM indication with citation) and typed derived parameters. Recorder ordinals live only in DATA_PROFILE.
- **Active version:** the published document with the highest version. Publishing does not retire older versions; retiring is explicit, and the active version then falls back to the previous published one.
- **Draft creation:**
  - Allowed only for kinds with a validator; P02 has DATA_PROFILE and PARAMETER_CATALOG. Other kinds are refused with 422 80105, naming their phase.
  - A draft may be cloned from a document of the same kind by sending `basedOnDocumentId` without content.
- **Publish:**
  - Requires a validation free of issues and an approval note.
  - A catalog publish is also refused if the active profile would no longer validate (`ACTIVE_PROFILE_INCOMPATIBLE`).
  - D-02 default applies: a single approver, audited.
- **Profile publish:** requires time, pressure altitude, CAS, Mach, TAT, groundspeed, radio height, and N1, N2, EGT and FF on both positions (docs/02 §1a and §4.3).
- **Profile dry-run:**
  - Multipart CSV upload, at most 50 MB.
  - A signature mismatch stops parsing.
  - Values are compared after trimming against the per-column missing tokens. Numeric cells parse with the invariant culture. Enum and text cells are shown as text.
- **Typed evaluation:**
  - Inputs are keyed by the resolved reference, for example `EGT_C[1]`, each with a unit that must equal the catalog unit.
  - Missing, null, wrong-unit, zero-denominator and non-finite cases give a status with no number.
  - Every result carries the formula with the numbers substituted.
- **Seed drafts:** created at startup when a kind has no document. A failure is logged and retried at the next start.
- **Audit:** an UPDATED entry now records only the changed properties. This keeps large document content out of status-change entries.
- **Error codes:** 80105–80116 are added. 80114 is reserved and unused.

## 5. Delta inventory planned
| Kind | Items |
|---|---|
| Domain concepts | registry life cycle on `ConfigurationDocument`; typed content models, validators, evaluator, previewer and diff (pure computation) |
| Tables | none; 6 columns on `ConfigurationDocuments` |
| Endpoints | 10 |
| UI pages | none here (S9 / S9a in the frontend repository) |
| Tests | seed, validator, evaluator-vector, diff and preview domain tests; lifecycle, walkthrough, dry-run and typed-definition acceptance tests |
