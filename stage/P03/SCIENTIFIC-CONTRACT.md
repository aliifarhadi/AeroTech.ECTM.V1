# P03 — Scientific contract (parsing and identity, no engineering computation)

**Observation:** recorder CSV/XLSX cells, mapped by the active DATA_PROFILE (ordinal + header occurrence) to catalog parameters.

**Method:**
- Values are parsed with the invariant culture after trimming. A missing token means no value; an unparseable cell means no value plus a count.
- Units come from the profile, with the IDENTITY conversion only.
- The time axis is `ELAPSED_S`. Neither UTC clock is used as the flight time.

**Identity:**
- File name, column values and the installation history.
- No engine is ever guessed: when there is no installation, the engine is POSITION_ONLY.

**Three gates:**
- **Regression gate:** row counts, first and last elapsed, and the flight number, day and month of all 8 files equal `fixtures/golden/*.oracle.json → identity`. SHA-256 of all 8 equal the goldens.
- **Correctness gate:** independent values read with Python's stdlib `csv` module (not the product parser) at samples 101 and 121 of flight 25:

| Sample | ELAPSED_S | N1_PCT[1] | N1_PCT[2] | N2_PCT[1] | N2_PCT[2] | EGT_C[1] | EGT_C[2] | FUEL_FLOW_PPH[1] | AIR_GROUND (raw) |
|---|---|---|---|---|---|---|---|---|---|
| 101 | 114960 | 92.52 | 92.71 | 97.85 | 97.87 | 848 | 873 | 6241 | " GND" |
| 121 | 114980 | 92.06 | 90.68 | 97.97 | 97.33 | 861 | 872 | 6009 | " AIR" |

- **Corpus gate:** every CSV in `samples/` (102 files) parses with the seed profile to the measured row count, with no BLOCKING issue. Exactly one elapsed-gap WARNING occurs in the corpus (`2026-10-09_EP-DOA_47`, after sample 3942, 126669 → 126673). Identity from the name agrees with the columns in every file (GAP-ANALYSIS §5).
- **Approval gate:** not applicable. Parsing makes no engineering claim.

No quality, phase or engineering result is produced in P03.
