# P04 — Test plan

| Pack requirement | Test | Project |
|---|---|---|
| golden parity of the quality sections 8/8 | every field of `quality` in each golden equals the C# result | Conformance `QualityGoldenTests` |
| independent parity on the real data | the pack oracle's output over all 102 files of `samples/` equals the C# result | Conformance `QualityGoldenTests` |
| flight 24 | jump at 3887 CONFIRMED, mask 3867–3917 with actual 663 ft/s and threshold 300, lag 57 s, clock counters, grade C with reason | Conformance + Acceptance (J3) |
| CLOCK-01 | clocks that disagree → UNTRUSTED, counted, offset histogram | Conformance `ClockTrustTests` |
| CLOCK-02 | invalid values (24 h, 60 min, one-digit seconds, two fields, missing) → UNTRUSTED, counted; a one-digit hour is valid by owner ruling and invalid in the oracle format | Conformance |
| CLOCK-03 | clocks that agree and step with elapsed are TRUSTED across midnight and an elapsed gap; a clock that does not step is counted; the tolerance applies | Conformance |
| stuck sensor | EGT identical while N1 moves 11.8 % → stuck with a note; identical in steady cruise → not stuck; below live N1 → not stuck | Conformance `StuckSensorTests` |
| seed rules on the real data | 102 files graded A 70, B 1, C 31; no stuck span; no invalid clock row with one-digit hours accepted | Conformance `QualityGoldenTests` |
| grade rules | A, B (mask, gap, coverage below 99 %), C (coverage below 95 %, partial start and end), NOT_USABLE (blocking, coverage below 50 %) | Conformance `QualityGradeTests` |
| missing channel / reordered time | named reason and null, never zero; NOT_USABLE | Conformance |
| QUALITY_RULES validation | seed valid; paths named for bad thresholds, channel kinds, unknown channels and span codes, missing sections, unknown fields | Conformance `QualityRulesValidatorTests` |
| quality on commit and on demand | J3: every batch row has a grade; same rules version returns the same result; R-A3 summary of the 8 flights | Acceptance `ImportWalkthroughTests` |
