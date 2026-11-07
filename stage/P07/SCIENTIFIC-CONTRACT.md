# P07 — Scientific contract

## What each number means
| Number | Meaning |
|---|---|
| `baselineRawValue` | The raw reference the engineer confirmed, in the catalog unit. It may be the P06 value of the declared first flight after shop, edited, or typed in. It is **not** environment-neutral and **not** a proof of a healthy engine. |
| `rawObserved` | The P06 statistic of an accepted (FINAL) flight, for the same physical ESN. |
| `rawDelta` | `rawObserved − baselineRawValue` when the measurement matches: same ESN, snapshot kind, selector, parameter and unit. Otherwise it is null, with the reason. It is DESCRIPTIVE_RAW: environmental effects are **not** removed. |
| `corrected*` | Always null in P07, with NO_APPROVED_CORRECTION_MODEL. The four owner AMM documents describe ground tests, not an in-flight correction (pack v2.0 docs/10). |

## Normalization
- Normalization is P08. It needs a source-backed, engine-specific method, and the textbook θ/δ is not one.
- Until then, P07 shows each condition difference as a number (altitude, Mach, TAT, CAS, GS, and the same engine's N1 when BASELINE_RULES names it).
- With BASELINE_RULES published, a difference above the CANDIDATE tolerance raises a warning only.
- Example: 28 °C against 30 °C is compared, but the 2 °C remains inside the delta.

## Takeoff and vibration
- **TAKEOFF_N1_PEAK_CONTEXT:**
  - The P06-D04 masks are unchanged.
  - CONTEXT_UNVERIFIED_CHANNEL_COHERENCE is shown as a warning and never hides the value.
- **TAKEOFF_EGT_PEAK_OBSERVED:**
  - It has no context of its own, so it gets CONTEXT_UNKNOWN.
  - It is raw only.
- **Vibration:**
  - It stays in the recorder's native scale, with DESCRIPTIVE_RAW_UNIT_UNVERIFIED.
  - No AMM ground vibration-survey limit is applied.

## Independent check
- **Frozen fixture:** `tests/AeroTech.ECTM.Domain.ConformanceTests/Baselines/independent-p07-v2.json`.
  - It is an unchanged copy of the pack fixture; its SHA-256 `c1ecfc2b…e126` equals the pack `MANIFEST.sha256`.
  - Its 8 cases are covered: +6, +7, +11, +0.03 (vibration), and the 4 null cases.
- **Golden data:** the real-file walkthrough checks the prefill suggestion against the P06 golden.
  - 9072 E1 OPERATOR_60S EGT = 660.15.
  - The E2 value is checked too.
  - The comparison delta equals the golden value minus the reference.

## Approval
- ENGINEERING_APPROVED: PENDING.
- The tolerances are CANDIDATE.
- The first flight after shop is the engineer's declaration, not a verified chronology.
