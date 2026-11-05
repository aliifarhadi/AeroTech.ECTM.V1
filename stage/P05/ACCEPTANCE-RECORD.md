# STAGE ACCEPTANCE — P05 / Explainable phases (backend)

## Source lock
- **Backend:** `AeroTech.ECTM.V1`, uncommitted on top of `7b64401` (P02). It contains P03, P04 and P05.
- **References read:** GAP-ANALYSIS §2, DECISIONS.md and `docs/legacy-notes/phases.md`.
- **Migrations:** `AddAnalysisRevisionsAndPhases`, `RecordPhaseReference`.
  - It adds the tables AnalysisRevisions and FlightPhaseAssessments.
  - It adds `FlightQualities.AnalysisRevisionId`, which is unique and replaces the unique index on flight and rules document.
- **Development database (owner instruction after the P05 report: "run the migration, publish the seeds"):**
  - All 6 migrations are applied to `DotAirEctmV1`. The database had no flights.
  - The catalog, profile and QUALITY_RULES were already published as v1, and their content equals the current seeds, so they were kept.
  - PHASE_RULESET v1 was published from the startup seed draft.
  - After the owner's request to make the algorithm label understandable, the seed lost `algorithmVersion` and gained `title`/`description`. The new seed is published as v2, validated with 0 issues. v1 is RETIRED with a note: no flight used it, and the new code cannot read it.
  - Migration `RecordPhaseReference` is applied: AlgorithmVersion renamed to ReferenceAlgorithm, ReferenceMatch added.
  - The registry is parser-ready. This used a separate host on port 5199, stopped afterwards. `evidence/config-kinds-after-publish.json` holds the result.
- **Report, screen and route IDs:**
  - reports R-F3, R-F4 and R-D6 (descriptive data)
  - screens S8 Phases, Events and History, and S9/S9a for PHASE_RULESET
  - 7 new routes, 1 removed, 54 in total

## Basis of the rules (owner instruction of 2026-11-05)
1. **Pack and benchmark accuracy.** The seed expresses the oracle `phase-v1.0.3` exactly; the oracle is the only reference that reproduces the goldens. Six points of the pack AST differ from the goldens; each is a decision in DECISIONS.md.
2. **Easy to change after the flight engineer.** Every rule is document data: thresholds, durations, predicate trees, precedence, anchor pick, fallbacks, post-processing order and events. A change is draft → dry-run diff → publish → recompute. No code changes, unless a new operator is needed.
3. **Transparency.** The seed carries 13 decisions, each with its basis, the alternative, how to change it and the parts it applies to; the validator rejects a dangling link. Each operator is one file holding its data, meaning, explanation trace and validation. The explanation shows every operand of every rule for any sample.

## Independent scientific contract
See SCIENTIFIC-CONTRACT.md.

| Gate | Result |
|---|---|
| Regression: phase sections of the 8 goldens (anchors, bases, coverage, every segment field). The fixture hashes are checked by `FixtureIntegrityTests` against `tests/fixtures/MANIFEST.sha256`. | PASS 8/8 |
| Independent: pack oracle over 102 real files = C# result, field by field | PASS 102/102 |
| Independent: adverse vectors PH-04a–f, go-around with and without TOGA, explanation values re-checked at every transition of the 8 goldens | PASS |
| Approval: thresholds CANDIDATE; flight-data engineering sign-off | **PENDING** (never inferred) |

## Reproducible E2E (what the owner clicks, docs/14 P05)
1. Recreate the schema of the cleared dev database: `dotnet ef database update --project src/AeroTech.ECTM.Persistence --startup-project src/AeroTech.ECTM.ServiceHost`.
2. Publish the seeds: catalog, profile, QUALITY_RULES, PHASE_RULESET (registry, J2).
3. Import the 8 files (J3). Every flight gets revision 1 with quality and phases.
4. `GET flights/{24}/phases` shows:
   - CRUISE … LANDING_ROLL
   - no CLIMB in the last 120 s
   - lift-off PARTIAL_START with no invented index
   - touchdown AIRGROUND_DISCRETE_LAGGED at the golden index
   - a transition list with the fired rules
5. `GET flights/{24}/phases/explain?sampleIndex=3886` gives UNKNOWN(ALTITUDE_RATE_UNTRUSTED):
   - rule R-ALTITUDE_RATE_UNTRUSTED
   - mask 3867–3917 from the jump at 3887
   - smoothed rate null by the mask
   - every rule above and below with its values
6. Create a PHASE_RULESET draft, set conditions CRUISE `minSec` to 240, then `POST …/dry-run/phases`:
   - flight 24: 0 changed samples (its cruise lasts 1402 s)
   - flight 9072: 397 samples CRUISE → LEVEL (cruises of 213 s and 184 s)
   - the draft stays a DRAFT
7. **Do not publish.** The publish-and-recompute path is covered by `RecomputeWalkthroughTests` on a throwaway database.

Tests: `Flows/ImportWalkthroughTests` (J3 extended) and `Flows/RecomputeWalkthroughTests`.

**Negative paths tested:**
- the PH-04a–f vectors
- 13 broken documents, each naming its JSON path
- an unknown operator, an English instruction, a missing operator, a numeric enum and a text threshold, all refused
- explain outside the flight → 80320
- phases before a PHASE_RULESET → 80319
- recompute with the same versions → 80321; while pending → 80322; with a document of the wrong kind → 80323
- dry-run/phases on a catalog → 80111

**Other:**
- 401/403: NOT DELIVERED (security deferred by the owner).
- Tests: conformance 313/313 (73 new), acceptance 81/81 (`evidence/`).

## Delta inventory
| Kind | Added in P05 |
|---|---|
| Configuration | PHASE_RULESET authorable: typed AST seed with a decisions register; validator with JSON paths (catalog-checked) |
| Domain | Closed operator library, with one class per operator: 2 derived channels, 2 anchors, 5 conditions, 8 predicates, 3 post-processing steps. Also the evaluator, explanation, segmenter, event detector and dry-run comparison. AnalysisRevision and FlightPhaseAssessment aggregates. |
| Contracts | 12 enums: FlightPhaseCode, PhaseUnknownReason, PhaseConfidence, AnchorBasis, CompareOperator, TransitionPick, RevisionStatus, RevisionTrigger, EventEvaluability, CalibrationStatus, EngineeringSignOff, ReferenceMatch; ProcessingJobKind.RECOMPUTE |
| Tables | AnalysisRevisions, FlightPhaseAssessments; FlightQualities.AnalysisRevisionId |
| Endpoints | GET `flights/{id}/phases`, `phases/explain`, `events`, `revisions`; POST `flights/{id}/recompute`; POST `config/{kind}/documents/{id}/dry-run/phases`; GET `events/aircraft/{id}`. Removed: POST `flights/{id}/quality`. |
| Tests | 69 conformance tests; acceptance J3 extended, one new recompute walkthrough, route inventory and kind tests updated |

## Deviations
- **Carried over:** Consumers kept, Framework envelope, Ordering query pattern, no security.
- **New in P05:**
  - The rule set follows the oracle where the pack AST differs (difference dossier).
  - `POST flights/{id}/quality` is replaced by `recompute`.
  - `steps` of `flights/{id}/recompute` is not offered: every revision runs quality and phases.
  - `engines/{id}/recompute` moves to P06.
  - The phase dry-run of the registry is `dry-run/phases`, beside the P03 `dry-run/flight`.

## Stage closure
- **`SOFTWARE_VERIFIED`:** PASS (394/394 tests). Security remains deferred.
- **`ENGINEERING_APPROVED`:** PENDING. Thresholds are CANDIDATE and the sign-off is PENDING.
- **`OPERATIONAL_STATUS`:** phases AVAILABLE in `DotAirEctmV1` (all four seeds active) once the host runs this build; flights imported from now on get revision 1 with quality and phases.
- **Open owner decisions and inputs:**
  1. **For the flight engineer:** P05-D02 to P05-D10 (especially D07, the late landing roll, and D10, invalid GS on the ground).
  2. **Recorder documentation:** active literals for fire, stick shaker and the left A/T warning (D11).
  3. **Carried over from P04:** preview as a job; publish gate; I-01, I-02; THETA and DELTA; fixture publication; package vulnerability.
- **Owner signature and gate to start P06:**
