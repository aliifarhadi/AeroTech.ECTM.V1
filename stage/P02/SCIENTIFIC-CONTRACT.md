# P02 — Scientific contract (typed derived parameters)

## Inputs and units
Every input is a catalog parameter at one position (`CODE`, `CODE[1]`, `CODE[2]` or `CODE[pos]`), in its catalog unit. No recorder ordinal is read; ordinals exist only in DATA_PROFILE.

## Methods (version: evaluator P02)
| Kind | Result | Rules |
|---|---|---|
| DIFFERENCE | a − b | a and b share one unit, which is also the result unit |
| RATIO | numerator / denominator | a zero denominator gives null with `DENOMINATOR_ZERO`; equal input units give RATIO |
| LINEAR_COMBINATION | Σ coefficientᵢ·xᵢ + offset | all terms share one unit; coefficients and the offset are visible constants |
| POWER_RATIO | x / referenceConstant | the result is RATIO. An exponent is refused (`EXPONENT_REQUIRES_CORRECTION_MODEL`): it can only come from an approved CORRECTION_MODEL (P08). |

Common rules for all kinds:
- A missing, null or wrong-unit input gives null with a status and a reason.
- A non-finite result gives null with `NON_FINITE`.
- No value is ever replaced by zero.
- Rounding happens only at display; the arithmetic is IEEE-754 double precision, so it is deterministic down to the bits.

## Seed definitions and their status
| Code | Definition | Status | Open assumption |
|---|---|---|---|
| EGT_DIFF_E2_E1_C | EGT_C[2] − EGT_C[1], same sample | CANDIDATE | — |
| TAT_K | 1·TAT_C + 273.15 | CANDIDATE | — |
| THETA | TAT_K / 288.15 | CANDIDATE | total-temperature form; the owner confirms the OEM reference temperature definition |
| DELTA | STATIC_PRESSURE_INHG / 29.92126 | CANDIDATE | the owner confirms whether the OEM uses total pressure |

288.15 K and 29.92126 inHg are ISA standard-day constants, not OEM values. They are stored in the catalog document, not in code.

## Three gates
- **Regression gate:** not applicable. No golden covers derived parameters.
- **Correctness gate:**
  - 46 independent vectors with hand-calculated expected values (`DerivedParameterEvaluatorTests`), covering normal, negative, zero, null, missing, wrong-unit, position, overflow and determinism cases for each evaluable kind.
  - Profile headers are compared byte-for-byte with the real recording.
- **Approval gate:** PENDING. A powerplant or flight-data engineer reviews:
  - the catalog roles and units
  - THETA and DELTA
  - the recorder vibration channels (unit UNVERIFIED until I-02)

## Not used scientifically yet
No derived parameter is applied to flight data until P03+. THETA and DELTA feed no correction: there is no approved model (`NO_APPROVED_MODEL`, P08).
