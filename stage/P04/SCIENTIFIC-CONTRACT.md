# P04 — Scientific contract (data quality, no engineering result)

**Observation:** the stored channels of a flight (P03), read through the QUALITY_RULES document.

**Method:** the pack oracle `quality-v1.0.1`, step by step:
1. **Elapsed gaps.** A step larger than the file's nominal step; the oracle uses 1 s, identical on all real data. Gaps split the recording into blocks.
2. **Altitude discontinuity.** The rate of pressure altitude and of radio height between neighbouring samples in one block.
   - Both above 300 ft/s with the same sign → CONFIRMED.
   - Only one above → SUSPECT_SINGLE_CHANNEL.
   - Each jump masks `ALTITUDE_RATE_UNTRUSTED` from 20 s before to 30 s after, inside its block.
3. **AIR/GROUND lag.** GND while radio height is above 100 ft for at least 10 s → `AIRGROUND_LAGS_RADIO_HEIGHT`. The lag is the time of the first AIR minus the time of the first radio height above 50 ft, in the first block.
4. **Engine lag.** Radio height above 200 ft, rising more than 100 ft over 10 samples in one block, while every N1 is below 40 % → `ENGINE_CHANNELS_LAG_KINEMATICS`.
5. **Partial start and end.** Radio height at the first sample above 20 ft; groundspeed at the last sample at least 35 kt.
6. **Clock trust.**
   - Each clock is parsed as `HH:MM:SS`; with `acceptOneDigitHour` (owner ruling) also `H:MM:SS`. The oracle parity runs with the oracle format.
   - Counted per row: the two clocks disagree, a value is invalid or missing, a clock step differs from the elapsed step by more than `clockStepToleranceSec` (modulo one day).
   - TRUSTED only if every counter is 0. Elapsed gaps do not count (docs/17 §3).
7. **Stuck sensor.** Identical values while the engine's N1 is at least 40 %, for at least the channel's duration and, where configured, while N1 moves at least the channel's percentage.
8. **Coverage, plausibility, grade.** See GAP-ANALYSIS §3.

**Units:** feet, ft/s, knots, percent, seconds, as in the catalog. No conversion.

**Three gates:**

| Gate | Evidence |
|---|---|
| Regression: the quality sections of the 8 goldens, field by field (gaps, partial flags, lag, first indices, discontinuities, spans with ordinals and evidence, clock counters and offset histogram, time trust) | `QualityGoldenTests` |
| Independent: the pack oracle (Python, independent of the C# code) over all 102 real files equals the C# result field by field | `QualityGoldenTests.Every_real_file_in_samples_matches_the_oracle` |
| Approval | thresholds are CANDIDATE; calibration is P13 |

All results are PROVISIONAL. P04 produces no phase, window, delta or alert.
