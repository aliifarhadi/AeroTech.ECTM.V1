# P05 — Scientific contract (explainable phases, no engineering result)

**Observation:** the stored channels of a flight (P03) and the quality result of the same revision (P04), read through the PHASE_RULESET document.

**Method:** the typed document of the seed equals the pack oracle `phase-v1.0.3`. Steps, in document order:
1. **Derived channels.**
   - `ALT_RATE_FPM` = (alt[after] − alt[before]) · 60 / (t[after] − t[before]). The samples are the nearest to t ± 15 s within ± 2 s, in the same block, with a value; ties go to the earlier sample.
   - `ALT_RATE_SMOOTH_FPM` = the median of the rates at i − 2 … i + 2 in the same block, with at least 3 values. It is null inside ALTITUDE_RATE_UNTRUSTED.
2. **Anchors.**
   - LIFTOFF: the first upward crossing of RH 50 ft, with CAS ≥ 100 kt at the crossing, staying above 50 ft for 10 s in one block. Otherwise PARTIAL_START when the quality result says the file starts airborne; otherwise NOT_FOUND. The index is never invented.
   - TOUCHDOWN: the first AIR→GND change after lift-off that holds 5 s in one block (basis AIRGROUND_DISCRETE_LAGGED); otherwise NOT_FOUND.
3. **Conditions** (named sample sets):
   - **GROUND:** GND, GS < 35, CAS < 60, |rate| < 200, sustained 10 s.
   - **AIRBORNE:** see P05-D04.
   - **CLIMB, DESCENT, LEVEL:** ≥ 300, ≤ −300 and |rate| ≤ 200 fpm while airborne, each sustained 20 s.
   - **CRUISE:** LEVEL with Mach ≥ 0.35, sustained 180 s.
   - **TAKEOFF:** lift-off to +120 s, not ground.
   - **TAKEOFF_ROLL:** see P05-D05.
   - **LANDING_ROLL:** from touchdown, if GS ≥ 80 there, while GS ≥ 35.
   - A run never crosses a gap; seconds = t[end] − t[start] + 1.
4. **Label rules in precedence order:**
   - GROUND (TAXI_OUT before lift-off, TAXI_IN after touchdown) > LANDING_ROLL > TAKEOFF_ROLL > TAKEOFF > UNKNOWN(ALTITUDE_RATE_UNTRUSTED) > CRUISE > CLIMB > DESCENT > LEVEL.
   - Fallback UNKNOWN(INSUFFICIENT_EVIDENCE).
5. **Post-processing, in order:**
   1. Both neighbours of a gap become UNKNOWN(RECORDING_GAP).
   2. Runs under 10 s become UNKNOWN(SHORT_ISLAND), except UNKNOWN, TAKEOFF, TAKEOFF_ROLL and LANDING_ROLL.
   3. A CLIMB of ≤ 60 s between two DESCENTs becomes UNKNOWN(UNCORROBORATED_CLIMB_IN_DESCENT), unless TOGA is Engaged in it (go-around).
6. **Segments.**
   - Runs break at a label change or a gap.
   - Confidence LOW for UNKNOWN, for overlap with AIR/GROUND or engine lag, and for a non-ground segment at a file edge. MEDIUM for TAKEOFF, TAKEOFF_ROLL, LANDING_ROLL and LEVEL. HIGH otherwise.
   - Flags EDGE_WINDOW and CHANNEL_COHERENCE_SUSPECT; unknown reasons sorted.
7. **Events:** runs of the active literal per channel and position, with the phase at start and end, GS (or CAS, RH, altitude) at start and end, and peak N1. Evaluability per channel: EVALUATED / UNVERIFIED_ENUM / MISSING.
8. **Explanation:** for any sample, the explanation recomputes the same run with the revision's documents and stored quality result. It returns:
   - every label rule in precedence order, with matched / fired and the full predicate tree with observed values and thresholds
   - the run seconds against the required seconds
   - the derived values with their source indices
   - the quality masks with their jump and cause
   - the anchors with their operands
   - the post-processing step that changed the sample

**Units:** feet, ft/min (derived), knots, Mach, seconds, as in the catalog. No conversion.

**Three gates:**
| Gate | Evidence |
|---|---|
| Regression: phase sections of the 8 goldens (anchors, bases, coverage, every segment field) | `PhaseGoldenTests.The_phase_section_equals_the_golden` 8/8 |
| Independent: the pack oracle (Python) over all 102 real files equals the C# result field by field; adverse vectors (PH-04a–f) built in test code; go-around with and without TOGA; explanation values re-checked against each operator's definition at every transition | `PhaseGoldenTests`, `PhaseNegativeTests`, `PhaseExplanationTests` |
| Approval | thresholds CANDIDATE; engineering sign-off **PENDING** (P05-D13) |

All results are PROVISIONAL. P05 picks no stable window, computes no delta and raises no alert.
