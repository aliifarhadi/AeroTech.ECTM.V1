# P06 — Test plan

| Pack requirement | Test | Project |
|---|---|---|
| goldens for the engine sections 8/8 | takeoff and cruise per engine per selector, field by field | Conformance `SnapshotGoldenTests` |
| paired against the frozen independent fixture | 24 records: status, reason, joint block, report window, matched count, sum, mean | Conformance `SnapshotGoldenTests` |
| independent: oracle over the 102 real files | takeoff and cruise sections equal on every file | Conformance `SnapshotGoldenTests` |
| N1 peak ≠ EGT peak | 16 engine-takeoffs, at least one non-zero offset | Conformance |
| 6951 / 6950 (docs/14 E2E) | 6951 OPERATOR_300S both AVAILABLE, paired 60; 6950 E1 NO_ELIGIBLE_BLOCK, E2 AVAILABLE, paired MISSING_ENGINE_WINDOW | Conformance + Acceptance (J3) |
| SN-NEG-01 | no EGT in the E1 takeoff window → NOT_EVALUABLE(NO_EGT_IN_SELECTED_WINDOW), E2 identical | Conformance `SnapshotNegativeTests` |
| SN-NEG-02 | holes in E2 N1 leave every E1 result identical | Conformance |
| SN-05 | flight 24 OPERATOR_300S: joint 1718–2263, report 1961–2020, 60 matched | Conformance |
| gate trace | report-window gates pass with their limits; nearest rejected neighbours; rejected counts per gate | Conformance |
| phase metrics | hand-calculated mean, median, p90/p95, sd; compensated summation; flight 24 groups, counts, masked samples | Conformance `PhaseMetricsTests` |
| SNAPSHOT_RULES validation | seed valid; 9 broken parts name their path; unknown field refused | Conformance `SnapshotRulesValidatorTests` |
| review state machine | accept, exclude, hold of an accepted flight, release, identity required | Conformance `FlightStateTests` |
| unaccepted flight cannot enter a report or baseline | R-E6 before acceptance: 0 contributors; after: 8 | Acceptance (J3) |
| accept 7, hold 1, release, accept (docs/14 E2E) | bulk ACCEPT with D-01 refusals, individual accept of the refused, stale revision 80325, EXCLUDE without reason 400, review entries HOLD/RELEASE/ACCEPT, results FINAL, R-E7, R-D9, R-P04, bulk again refused NOT_PENDING_REVIEW, engine recompute UNCHANGED | Acceptance `ImportWalkthroughTests` |
| route inventory | 63 routes | Acceptance |
