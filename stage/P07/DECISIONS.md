# P07 — Decisions

**Authority:** `docs/ECTM-P07-Baseline-Pack-v2.0`. It supersedes Redesign-Pack v1.0 and the v3.3 docs/07 for P07.

**Owner rulings of 2026-11-07:**
- Excel/CSV is a prefill helper only. There is no minimum number of flights and no averaging.
- The raw delta is always shown, together with warnings.
- The P06 takeoff masks stay unchanged.
- Vibration is raw only, labelled with its unverified unit.
- The comparison is stored and written by one job (`BASELINE_EVALUATION`) whose subject is the engine.
- Condition warnings come from an optional BASELINE_RULES document.
- Backend only: the frontend agent builds the UI.

## Register of the BASELINE_RULES seed (`baseline-rules-ep-doa-seed.json`, CANDIDATE, sign-off PENDING)
| Id | Topic | Choice | How to change |
|---|---|---|---|
| P07-D01 | Warning, not blocking | A condition outside its tolerance adds `CONDITION_OUT_OF_TOLERANCE`, and the raw delta stays | Edit or remove a condition. Retiring the document shows the numbers only. |
| P07-D02 | Tolerance units | Pressure altitude ±2000 ft, Mach ±0.03, TAT ±8 °C, N1 of the same engine ±1.5 points | Edit the tolerance |
| P07-D03 | Thresholds and sign-off | CANDIDATE / PENDING until P13 | A new version after calibration |

## Decisions in code
| Decision | Where |
|---|---|
| One `Baseline` row per revision. A clone is a new root in the same lineage (`LineageId`, `RevisionNumber`, `PreviousBaselineId`), so no history is loaded to edit. | `Baseline` |
| A published or retired baseline is immutable (409 80401). Every draft edit names `expectedDraftRevision` (409 80403). | `Baseline.Revise/Publish` |
| Publishing needs at least one parameter, every row with a finite value, and an explicit confirmation. No evidence, envelope, shop event or flight count is required. | `Baseline.PublicationIssues`, `PublishBaselineValidator` |
| The server derives the statistic, the unit and the value origin. The client sends none of them. The statistic follows the snapshot kind: MEAN, or AT_PEAK_SAMPLE for the EGT peak. The unit comes from the active catalog. The origin is MANUAL when there is no suggestion, FILE_SUGGESTED when the value equals it, and FILE_EDITED otherwise. | `MeasurementStatistic`, `BaselineDraftBuilder`, `BaselineParameter` |
| Each parameter pins the active catalog and SNAPSHOT_RULES versions at draft save. | `BaselineDraftBuilder` |
| A measurement is snapshot kind + selector + parameter, unique within a baseline. The same parameter at another snapshot is a separate measurement. | DB unique index, `BaselineParameterCheck` |
| The reference context holds the snapshot's aircraft conditions (altitude, Mach, TAT, CAS, GS), its engine values and its configuration. Condition checks list the aircraft conditions, plus the per-engine conditions of BASELINE_RULES. | `MeasurementContext`, `ComparisonDetail` |
| The one evaluator gives `rawDelta = observed − reference` for a matching measurement. It returns null with the cause (DIFFERENT_ESN, SNAPSHOT_KIND/SELECTOR/PARAMETER/UNIT_MISMATCH, MISSING_OBSERVATION) and never zero. Every result carries NO_APPROVED_CORRECTION_MODEL. | `BaselineComparisonEvaluator` |
| Warnings never block. They are ENVIRONMENT_NOT_REMOVED (always), CONTEXT_UNKNOWN, CONTEXT_UNVERIFIED_CHANNEL_COHERENCE, CONDITION_OUT_OF_TOLERANCE, CONFIGURATION_DIFFERS, SNAPSHOT_RULESET_CHANGED, REFERENCE_SOURCE_POINT, RETROSPECTIVE and TEMPORAL_ORDER_UNVERIFIED. | same |
| Chronology: a flight before the declared date is RETROSPECTIVE. The same day, or a missing date, is TEMPORAL_ORDER_UNVERIFIED. | same |
| A unit ending in `_UNVERIFIED` (the vibration channels) gives DESCRIPTIVE_RAW_UNIT_UNVERIFIED. | same |
| REFERENCE_SOURCE_POINT: the flight's source-file SHA-256 equals the baseline's prefill file hash. The edited value is still compared normally, so the delta is not forced to zero. | same |
| The corrected values are not stored in P07. The DTO returns them as null. P08/P09 add the columns to the same table. | `BackofficeBaselineEvaluationDto` |
| **Job: one kind `BASELINE_EVALUATION`, subject = EngineId, one unit.** The job reconciles every published or retired baseline of the engine against every current ACCEPTED+FINAL flight. | `BaselineEvaluationJobHandler`, `EngineBaselineReconciler` |
| Idempotency: a current row is kept while its fingerprint is unchanged. The fingerprint covers calculation version, parameter, engine, flight, revision, stability points, position, date, file hash and BASELINE_RULES document. Otherwise the row is withdrawn and a new one recorded. Withdrawals are saved before the inserts because of the unique "one current row per parameter and flight" index. | same |
| Triggers: publish; any review action (`FlightReviewer`); a completed analysis revision on import or recompute (`AnalysisRecorder`); and `POST baselines/{id}/reconcile`. A queued job of the same engine is reused, including one not yet saved. Engines without a published baseline get no job. | `BaselineEvaluationScheduler` |
| Read overlay: a stored row is shown as NOT_FINAL when the flight is not ACCEPTED, and STALE_REVISION when it is not current. This holds even before the job runs. A GET never writes. | `BaselineParameterFlight.StatusFor` |
| Primary: zero or one per (engine, kind, selector, parameter), always set explicitly. A stale `expectedCurrentPrimaryId` gives 409 80408. Retiring a baseline removes its primaries. Publishing never sets or moves a primary. | `SetPrimaryBaselineService`, `RetireBaselineService` |
| Prefill runs the P03–P06 pure chain unchanged and stores nothing. The file name pattern is not required. The SHA-256 is returned lower-case, and the file is not kept (`fileArchived=false`). | `PrefillBaselineFromFileService` |
| Draft parameters are replaced as a whole on update, with cascade delete of the old rows. | `BaselineConfiguration` |
| Engine- and flight-scoped routes sit on their own controllers, as `period-performance` does. | RestApi |
| Error codes 80400–80410. The next baseline code is 80411. | `ExceptionFactory` |
