# P05 — Test plan

| Pack requirement | Test | Project |
|---|---|---|
| goldens 8/8: anchors, bases, coverage, segments (every field) | `PhaseGoldenTests.The_phase_section_equals_the_golden` | Conformance |
| independent: oracle over all 102 real files = C# result | `PhaseGoldenTests.Every_real_file_in_samples_matches_the_oracle` (reports/P05/phase-corpus.json) | Conformance |
| every sample in exactly one segment | `PhaseGoldenTests.Every_sample_is_in_exactly_one_segment` | Conformance |
| TR spans 8/8 | spans per engine = golden `trDeployedRows` | Conformance + Acceptance (J3) |
| flight 24: CRUISE … LANDING_ROLL, no CLIMB in the last 120 s; the sample before the jump explains UNKNOWN(ALTITUDE_RATE_UNTRUSTED) | `PhaseGoldenTests` flight-24 facts; J3 explain at 3886 | Conformance + Acceptance |
| PH-04a one-row AIR bounce | bounce after touchdown: touchdown moves to the change that holds, one landing roll, no air label after touchdown | `PhaseNegativeTests` |
| PH-04b 3-s gap in cruise | both neighbours UNKNOWN(RECORDING_GAP), no segment across the gap, no rate across the gap (±13 samples UNKNOWN), cruise on both sides | `PhaseNegativeTests` |
| PH-04c missing radio height | lift-off NOT_FOUND, no takeoff labels, cruise and touchdown still found, full partition | `PhaseNegativeTests` |
| PH-04d frozen altitude | no CLIMB where altitude is frozen | `PhaseNegativeTests` |
| PH-04e/f short climb without / with TOGA | UNKNOWN(UNCORROBORATED_CLIMB_IN_DESCENT) / kept as go-around; a 61-s climb is kept | `PhaseNegativeTests` |
| touchdown pick editable (P05-D02) | FIRST → 20, LAST → 50 on a synthetic two-change discrete | `PhaseNegativeTests` |
| validation errors with JSON paths | 13 broken parts name their path and code; unknown operator, English instruction, missing operator, numeric enum, text threshold refused | `PhaseRuleSetValidatorTests` |
| explanation consistency at transitions | at every segment start of the 8 goldens: at most one fired rule, no matched rule above it, every operator's recorded result recomputed from its recorded values (comparisons, ALL/ANY/NOT, sustained seconds) | `PhaseExplanationTests` |
| dry-run diff (docs/14 E2E) | cruise 180 → 240 s: flight 24 no change; flight 9072 397 samples CRUISE → LEVEL, no anchor change | Conformance + Acceptance (J3 via `dry-run/phases`, not published) |
| determinism | same rules → same JSON; stored result reads back unchanged | `PhaseExplanationTests` |
| publish + reprocess → new revision | no phases → 80319; publish → recompute → revision 2 CURRENT, 1 SUPERSEDED, new quality row; pending → 80322; version 2 (cruise 240) → revision 3 loses 397 cruise samples; an older published version chosen → revision 4 equals revision 2; wrong kind → 80323 | Acceptance `RecomputeWalkthroughTests` |
| same versions refused; dry-run shows nothing changed | 80321; `changedSamples` 0 | Acceptance (J3) |
| R-F4 evaluability | fire channels UNVERIFIED_ENUM | Acceptance (J3) |
| R-D6 | 8 flights, 8 with phases, 8 landings; TR in 8 of 8 (100 per 100 landings) | Acceptance (J3) |
| route inventory | 54 routes | Acceptance `RouteInventoryTests` |
