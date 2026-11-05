# P06 — Decisions

The source of truth for the engineering rules is the `decisions` register of the SNAPSHOT_RULES seed (`src/AeroTech.ECTM.Domain/ConfigurationDocumentAggregate/Seeds/snapshot-rules-ep-doa-seed.json`). Status of every decision: **CANDIDATE, engineering sign-off PENDING**. A change follows the same path as in P05: draft → validate → publish → recompute (one flight, or `engines/{id}/recompute` for the accepted flights of an engine).

## Register (document)
| Id | Topic | Choice | Basis | How to change |
|---|---|---|---|---|
| P06-D01 | Reference | takeoff and cruise as the oracle; paired by the joint-block rule | goldens 8/8; docs/17 §3 and the independent fixture | code change with new independent vectors |
| P06-D02 | Windows in seconds | search and takeoff windows in elapsed seconds; the oracle counts samples | docs/05; identical on all 102 files | code change |
| P06-D03 | Two takeoff observations | N1-peak window and EGT peak, with the offset between them | docs/05 §1 | `takeoff` |
| P06-D04 | Takeoff context | values reported with CONTEXT_UNVERIFIED when a coherence mask overlaps | the oracle; 86 / 74 of 102 overlap the AIR/GROUND lag | remove the AIR/GROUND lag from `masks.coherence` |
| P06-D05 | Cruise selectors | all three computed, no default | availability STRICT 8/12, OPERATOR_300S 93/94, OPERATOR_60S 102/102; default at P13 | `cruise.selectors` |
| P06-D06 | Cruise eligibility | CRUISE, outside masks and gap neighbours, own N1, altitude, Mach, 7 configuration literals | docs/05 §2.1; oracle | `masks`, `configurationChannels`, `cruise` |
| P06-D07 | Phase metrics | samples, valued, missing, gap starts, masked, min, max, mean, median, nearest-rank p95, sample sd | docs/05 §3 | `phaseMetrics`, `conditionParameters` |
| P06-D08 | Thresholds | policy-defaults → snapshots | CANDIDATE until P13 | the number |
| P06-D09 | Sign-off | PENDING | never inferred | `engineeringSignOff` |
| P06-D10 | Centring | engine window centre − R/2; paired window start + (length − R)/2 | each frozen reference | code change |
| P06-D11 | Summation | compensated (Neumaier) | the reference's Python 3.14 `sum`; more accurate | code change |
| P06-D12 | Control context (R-F5) | N1, target, climb limit, bug, TLA per engine; A/T and N1-limit modes; over takeoff roll, takeoff, climb | docs/10 R-F5; I-02 UNVERIFIED | `controlContext` |

## Decisions in code (review and reports)
| Decision | Where |
|---|---|
| D-01 bulk acceptance: identity RESOLVED, grade A/B, no BLOCKING, both engines with TAKEOFF_N1_PEAK_CONTEXT, TAKEOFF_EGT_PEAK_OBSERVED and a cruise window in at least one selector. Every refusal is listed per flight. Individual acceptance is the engineer's own decision and is not gated. | `BulkAcceptance`, `BulkReviewFlightsService` |
| HOLD of an ACCEPTED flight withdraws the acceptance (results back to PROVISIONAL, review back to PENDING); RELEASE returns to PENDING_REVIEW (docs/11 state machine) | `Flight.Review`, `FlightReview.Record`, `FlightReviewer` |
| EXCLUDE needs a reason code and a note; EXCLUDED is final for its revision; only a new revision returns the flight to review | `ReviewFlightValidator`, `Flight` |
| Exclusion reasons are a closed list (owner ruling 2026-11-05): DATA_QUALITY_UNUSABLE, IDENTITY_UNCONFIRMED, DUPLICATE_RECORDING, INCOMPLETE_RECORDING, SENSOR_OR_RECORDER_FAULT, TEST_FERRY_OR_TRAINING_FLIGHT, ABNORMAL_OPERATION, ENGINE_EVENT_UNDER_INVESTIGATION, OTHER. The pack names no list; these cover its own cases (quality, identity, duplicate, events, recorder) and the usual engine-health exclusions. A reason is required with EXCLUDE and refused with any other action; the note stays required. Adding a reason is a small code change (enum `ExclusionReason`). | `ExclusionReason`, `ReviewFlightValidator` |
| An identity change creates an IDENTITY_CHANGE revision only when the engine assignment of the flight changes | `ConfirmFlightIdentityService` |
| `analysisRevisionId` in a review is optional; when given and not current, the review is refused (409 80325) | `FlightReviewer` |
| R-E6 and R-D9 use accepted (FINAL) stability points only; R-E7 counts every received flight | report services |
| `engines/{id}/recompute` recomputes the accepted flights of the engine (docs/13); each flight reports QUEUED, UNCHANGED or ALREADY_PENDING | `RecomputeEngineService` |
