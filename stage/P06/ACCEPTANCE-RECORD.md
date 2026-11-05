# STAGE ACCEPTANCE — P06 / Stability points and single review (backend)

## Source lock
- **Backend:** `AeroTech.ECTM.V1`, uncommitted on top of `7b64401` (P02). It contains P03–P06.
- **References read:** GAP-ANALYSIS §2, DECISIONS.md, `docs/legacy-notes/stability-points-and-review.md`.
- **Migration `AddStabilityPointsAndReviews`:**
  - It adds the tables FlightStabilityPoints, FlightReviews and FlightReviewEntries.
  - It adds the AnalysisRevisions columns for the SNAPSHOT_RULES version and the engine assignments.
  - Migration `UseExclusionReasons` turns the reason code into the closed list.
  - Both are applied to `DotAirEctmV1` (owner rule: every phase is delivered ready).
- **Report, screen and route IDs:**
  - reports R-F1 (assembled), R-F5, R-F7, R-E6, R-E7, R-D9 and R-P04 (data)
  - screens S8 Stability points / review actions / History, and S9/S9a for SNAPSHOT_RULES
  - 9 new routes, 63 in total

## Basis of the rules
- **Seed:** the SNAPSHOT_RULES seed copies `policy-defaults-v1.json → snapshots` (CANDIDATE).
  - Its `decisions` register (P06-D01 … D12) gives every choice with its basis, alternative and the parts it governs. The validator checks those links.
- **Measurement:** every rule was measured on all 102 real files of `samples/` with the pack oracle (GAP-ANALYSIS §3).
  - Takeoff is available for both engines in 102/102 files.
  - Cruise availability, engine 1 / engine 2: STRICT 8/12, OPERATOR_300S 93/94, OPERATOR_60S 102/102.

## Independent scientific contract
See SCIENTIFIC-CONTRACT.md.

| Gate | Result |
|---|---|
| Regression: takeoff and cruise engine sections of the 8 goldens, field by field | PASS 8/8 |
| Independent: frozen `paired-joint-block-v1.json` | PASS 24/24 records |
| Independent: pack oracle over 102 real files = C# result | PASS 102/102 |
| Independent: SN-NEG-01, SN-NEG-02, SN-05, hand-calculated statistics | PASS |
| Approval | PENDING: thresholds CANDIDATE, powerplant sign-off PENDING |

## Reproducible E2E (what the owner clicks, docs/14 P06)
1. Apply the migration and publish the SNAPSHOT_RULES seed (registry).
2. Import the 8 files, or recompute them if they are already imported.
3. `GET flights/{6951}/stability-points`, selector OPERATOR_300S:
   - both engines AVAILABLE, paired AVAILABLE with 60 matched samples
   - each window shows its gate values and its rejected neighbours
4. `GET flights/{6950}/stability-points`, selector OPERATOR_300S:
   - engine 1 NO_ELIGIBLE_BLOCK, engine 2 AVAILABLE
   - paired MISSING_ENGINE_WINDOW
5. `GET engines/{E1}/period-performance?kind=CRUISE&selector=OPERATOR_60S` gives 0 contributors: nothing is accepted yet.
6. Accept 7 flights, then hold 1, release it and accept it:
   - Hold flight 25.
   - `POST flights/bulk-review` ACCEPT for the other 7. D-01 refusals are listed per flight.
   - Accept the refused flights one by one.
   - Release flight 25 and accept it.
   - Each step is recorded with the reviewer; the results become FINAL.
7. R-E6 now has 8 contributors.
   - R-E7 shows 8 flights ACCEPTED.
   - R-P04 counts 8 ACCEPT actions.
   - `engines/{E1}/recompute` gives 8 accepted flights, all UNCHANGED.

Tests: `Flows/ImportWalkthroughTests` (J3 extended with the E2E above).

**Negative paths tested:**
- SN-NEG-01/02
- a stale revision in a review → 409 80325
- EXCLUDE without a reason, or with a free-text reason → 400
- ACCEPT before identity → 80314
- bulk ACCEPT of accepted flights → NOT_PENDING_REVIEW
- 9 broken SNAPSHOT_RULES documents, each naming its path
- no phase result → NO_PHASE_RESULT

**Other:**
- 401/403: NOT DELIVERED (security deferred by the owner).
- Tests: conformance 347/347, acceptance 81/81 (`evidence/`).

## Delta inventory
| Kind | Added in P06 |
|---|---|
| Configuration | SNAPSHOT_RULES authorable: seed, validator, decisions register, controlContext section |
| Domain | FlightStabilityPoints aggregate with one class per step (TakeoffSearch, TakeoffN1PeakFinder, TakeoffEgtPeakFinder, CruiseWindowGates, CruiseBlockFinder, PairedComparer, PhaseMetricsCalculator, ControlContextCalculator, CompensatedSum, SampleStatistics); FlightReview aggregate with entries and BulkAcceptance; the flight state machine now has ACCEPTED/EXCLUDED; shared RuleDocumentCheck, RuleDecisions, RuleDocumentHeader |
| Contracts | ReviewState, SnapshotKind, SnapshotAvailability, SnapshotReason, TakeoffSearchBasis, SnapshotContextStatus, CruiseGate, MetricGroupKind, RecomputeOutcome, BulkReviewOutcome; FlightState ACCEPTED/EXCLUDED; RevisionTrigger IDENTITY_CHANGE |
| Tables | FlightStabilityPoints, FlightReviews, FlightReviewEntries; AnalysisRevisions.SnapshotRules*, EngineAssignments |
| Endpoints | GET `flights/{id}/stability-points`, `phase-metrics`, `control-context`; POST `flights/bulk-review`, `engines/{id}/recompute`; GET `engines/{id}/period-performance`, `engines/{id}/data-coverage`, `engine-models/{id}/period-performance`, `reviews`; the review route takes `reasonCode` and `analysisRevisionId`; revisions include the review |
| Tests | 34 new conformance tests; J3 extended with the P06 E2E; route inventory 63 |

## Deviations
- **Carried over:** Consumers kept, Framework envelope, Ordering query pattern, no security.
- **New in P06:**
  - The paired rule follows the independent fixture where the oracle is defective.
  - The paired report window is centred differently from the engine window (P06-D10).
  - Error 80315 (accept/exclude delivered later) is removed. New errors: 80324, 80325, 80326.

## Stage closure
- **`SOFTWARE_VERIFIED`:** PASS (428/428 tests). Security remains deferred.
- **`ENGINEERING_APPROVED`:** PENDING.
- **`OPERATIONAL_STATUS`:** available on `DotAirEctmV1`. Every migration up to `UseExclusionReasons` is applied, SNAPSHOT_RULES v1 is published (`evidence/config-kinds-after-publish.json`) and the owner's import of `2026-10-02_EP-DOA_6951.csv` created its flight once the host ran this build.
- **Open owner decisions and inputs:**
  1. Powerplant review of P06-D04 (AIR/GROUND lag in the takeoff context) and P06-D10 (centring).
  2. Carried over from P04/P05.
- **Owner signature and gate to start P07:**
