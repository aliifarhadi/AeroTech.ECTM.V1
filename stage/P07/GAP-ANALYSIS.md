# P07 — Gap analysis

**Source lock:**
- Backend `AeroTech.ECTM.V1`, `3ef5fd9` (P06), clean tree before P07.
- Frontend `AeroTech.ECTM.Backoffice.V1`, `d4178af`. It was not touched; the owner's uncommitted change to `flights/[flightId]/page.tsx` is preserved.

**History:** the first P07 build follows v3.3 docs/07 (five flights, averaging, blocking envelope). The owner rejected it, and it was reverted to `3ef5fd9` together with its database migration.

## P06 → P07 mapping (verified in code)
| P06 source | P07 use |
|---|---|
| `RecordingInspector.Inspect`, `RecordingSetLoader`, `AnalysisRulesLoader.FindActiveAsync`, `QualityInput.From`, `QualityEvaluator`, `PhaseEvaluator`, `StabilityPointsEvaluator` | The prefill chain, unchanged. It is the same chain as `AnalysisRecorder.CompleteAsync`, so a prefill number equals the number the flight gets once imported. |
| `StabilityPointsResult` (takeoff N1, EGT peak, cruise per selector) | `SnapshotReading`: one additive projection. It keeps window, peak, context and context status. `SnapshotObservation` stays unchanged. |
| `FlightStabilityPoints.Status` FINAL + `Flight.State` ACCEPTED | Eligibility for a current evaluation |
| `Flight.Engines[].EngineId` (historic assignment) | Physical ESN; the position is provenance only |
| `SourceFile.Sha256` | REFERENCE_SOURCE_POINT |
| Flight's pinned catalog | Observed unit |
| `ProcessingJobRunner` + `IProcessingJobHandler` | The `BASELINE_EVALUATION` handler; no new queue |
| `FlightReviewer.ApplyAsync`, `AnalysisRecorder.CompleteAsync` | The two scheduling hooks; no change to the review rules |
| `ConfigurationKind.BASELINE_RULES` (already in the enum) | Validator and seed; optional for comparisons |

## Gaps that remain open (recorded, not hidden)
1. **Clean clone:** the real and golden fixtures, `samples/` and `reports/` are not in git (owner rule). The P06 golden tests and the P07 real-file walkthrough need them locally.
2. **Security:** no 401/403 (owner deferral). The write routes are not protected.
3. **Chronology:** `FlightDate` is day-only. Order within a day is never claimed.
4. **Vibration:** the recorder scale is unverified (I-02).
5. **Browser E2E:** this is the frontend agent's step.
