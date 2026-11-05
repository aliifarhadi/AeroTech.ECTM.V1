# P06 — Scientific contract (stability points and phase metrics, no engineering result)

**Observation:** the stored channels of a flight, its quality result (P04) and its phase result (P05) of the same revision, read through SNAPSHOT_RULES.

**Method, per engine position, never reading the other engine's N1:**
1. **Takeoff search window.**
   - [lift-off − 30 s, lift-off + 120 s] when the lift-off anchor exists (LIFTOFF_ANCHORED).
   - Otherwise, for a partial start, from the first sample to 120 s after the engine's own first N1 ≥ 85 % (PARTIAL_START_FIRST_HIGH_N1).
   - Otherwise NOT_EVALUABLE.
2. **TAKEOFF_N1_PEAK_CONTEXT.**
   - The engine's first N1 maximum in the search window, in the block of its start.
   - Report window [peak − 5 s, peak + 4 s] in the peak's block, with ≥ 8 valid N1 samples and ≥ 1 EGT value.
   - It reports means, minimums, maximums and counts of the 10 engine parameters, the condition means, the TOGA literals and the configuration literals.
   - Context is VERIFIED unless an untrusted or coherence mask overlaps the window.
3. **TAKEOFF_EGT_PEAK_OBSERVED:** the engine's first EGT maximum in the same search window, its values at that sample, and its offset in seconds from the N1 peak.
4. **Cruise, per selector.**
   1. A window of `windowSec` samples ending at i is eligible when all of these hold:
      - it is contiguous (`t[i] − t[start] = windowSec − 1`, one block)
      - every sample is CRUISE and outside the untrusted mask, the gap neighbours and the coherence masks
      - N1 is complete with max |N1 − mean| ≤ tolerance
      - altitude and Mach are complete and their ranges are within limits
      - the 7 configuration literals are the same throughout
   2. The block is the longest run of eligible window ends covering ≥ `minBlockSec` (earliest on a tie).
   3. The report window is the central `reportSec` samples: start = centre − R/2.
   4. Every window-end that is rejected records its first failing gate. The report window records each gate with its actual value and limit, and its nearest rejected neighbours.
5. **Paired E2 − E1, per selector.**
   - The joint block of the two engine blocks; report window start = jointStart + (length − R)/2.
   - It uses only samples where both EGT values exist; ≥ 20 such samples are required.
6. **Phase metrics.**
   - Groups: the whole flight, each phase, each segment and each steady cruise block.
   - For each engine and condition parameter: samples, valued, missing, gap starts, masked samples, minimum, maximum, mean, median, nearest-rank p95 and sample standard deviation.
7. **Control context (R-F5):** summaries of N1, target, climb limit, bug and TLA, and the A/T mode literals over takeoff roll, takeoff and climb. It is labelled UNVERIFIED (I-02).

Means and sums use compensated summation.

**Units:** as in the catalog, with no conversion. Nothing here is an EGT margin, a correction or a delta.

**Three gates:**
| Gate | Evidence |
|---|---|
| Regression: takeoff and cruise engine sections of the 8 goldens, field by field (availability, reasons, windows, peaks, 10 means, conditions, literals, context status) | `SnapshotGoldenTests` 8/8 |
| Independent: frozen `paired-joint-block-v1.json` (24 records); oracle over all 102 real files; SN-NEG-01/02, SN-05; hand-calculated statistics | `SnapshotGoldenTests`, `SnapshotNegativeTests`, `PhaseMetricsTests` |
| Approval | thresholds CANDIDATE; powerplant sign-off **PENDING** |

Results are PROVISIONAL until the single review accepts the revision; only then are they FINAL and usable by reports (and later baselines and trends).
