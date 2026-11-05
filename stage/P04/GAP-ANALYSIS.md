# P04 — Gap analysis

## 1. Source lock
- Legacy `AeroTech.ECTM@0ad5f93`, unchanged.
- V1 baseline: P00–P02 committed (`7b64401`); P03 uncommitted on top. The owner started P04 on 2026-11-05 ("start the next phase"), which is taken as the P03 gate.

## 2. Sources read, and the decision for each
| Source | Decision | Reason |
|---|---|---|
| `reference/ectm_oracle.py` `quality()` (quality-v1.0.1) | PORT exactly | The pack says the C# implementation must reproduce the oracle on every file. Ported step by step into `FlightQualityAggregate/Computation/QualityEvaluator`. Ordinals are replaced by parameter codes from the QUALITY_RULES document. |
| docs/17 §3 "second adjustment" | APPLY | Time trust ignores elapsed gaps (reported separately) and uses `clockStepToleranceSec`. |
| `contracts/policy-defaults-v1.json` `quality`, `dataQuality` | SEED | They become the QUALITY_RULES seed, `calibrationStatus = CANDIDATE`. |
| legacy `FlightQualityAssessmentAggregate` (assessment command, coverage report) | IGNORE | The pack oracle is the executable definition. The legacy shape (assessment command) is replaced by evaluation on commit and on demand. Its coverage report is covered by R-A3 data (`quality/aircraft/{id}`). |

## 3. Decisions taken for small gaps
- **QUALITY_RULES document:**
  - Typed, strict JSON, validated against the active catalog.
  - Every rule names its channels by parameter code. Ordinals appear only as provenance (`profileOrdinals`) in the output.
  - Thresholds and the downstream uses each span affects (`affects`) are configuration, not code.
- **One result per flight and rules version:**
  - `FlightQualities`, PROVISIONAL.
  - Evaluating again with the same version returns the stored result. A new version supersedes the old one.
  - The full result is JSON; grade, time trust, partial flags and counts are columns for lists and R-A3.
- **When it runs:**
  - inside the commit unit, in the same save as the flight, if a QUALITY_RULES version is active
  - on demand: `POST flights/{id}/quality`
- **Grade (policy text, as rules):**

  | Grade | Condition |
  |---|---|
  | NOT_USABLE | blocking validation, or a required channel below 50 % |
  | C | a required channel below 95 %, or partial start and partial end |
  | B | any mask other than the AIR/GROUND lag, the engine lag and the confirmed recorder altitude jump; an elapsed gap; or a required channel below 99 % |
  | A | otherwise |

  Every grade below A carries its reasons.
- **No fake zeros:**
  - A rule input that is not recorded gives `QUALITY_INPUT_MISSING` and a null result (for example partial start), never false or 0.
  - A time axis that is not strictly increasing gives NOT_USABLE with `TIME_AXIS_NOT_STRICTLY_INCREASING` and no numbers.
- **Coverage and plausibility:** the P03 INFO measurements are graded now: coverage below 95 % is WARNING and below 50 % BLOCKING; out-of-range values above 5 % of valued samples are WARNING.
- **Stuck sensor (agent decision, owner asked for a resolution-aware rule):**
  - A run of identical values while that engine's N1 is at least 40 %, per configured channel.
  - Each channel has a minimum duration and, optionally, a minimum N1 movement during the run.
  - A coarse channel may legitimately hold its value in steady cruise but must respond when the engine's power changes; the N1 movement is that test.
  - Candidates from §4: N1 16 s; N2 22 s; EGT 30 s with N1 moving at least 4 %; oil pressure 30 s with N1 moving at least 15 %.
  - No rule for fuel flow, oil temperature, vibration, imbalance angles, TLA and the N1 target/bug/limit: they hold their value while N1 moves over its whole range.
- **Clock format (owner ruling 2026-11-05):**
  - `H:MM:SS` is a valid clock value, set by `clock.acceptOneDigitHour` in QUALITY_RULES.
  - The oracle accepts only `HH:MM:SS`, so the parity tests run with the setting off and the goldens stay unchanged; no regeneration is needed.
  - With the seed, no real file has an invalid clock row; time trust stays UNTRUSTED on all 102 (the two clocks disagree or step apart).
- **Grade A and the recorder altitude jump (agent decision, owner left it to the agent):**
  - The jump confirmed by both altitude channels is a known recorder fact (docs/06 §2). It is masked so that P05 never reads a false climb.
  - It is treated like the two lags already allowed for A. A single-channel jump still gives B, because it may mean a faulty channel.
  - Configured as `grade.masksAllowedForA[{code, cause}]`, so it can be reversed by publishing a new version.
- **Error code:** 80318 = no quality result yet.

## 4. Measurement of the real data (`samples/`, 102 files, 2026-11-05)
Scripts: `evidence/quality_corpus.py` (the pack oracle over samples/) and `evidence/stuck_corpus.py`. Results stay in `reports/P04/` (private).

| Fact | 8 pack files | 102 real files |
|---|---|---|
| confirmed altitude jump | 8/8 | 82 (jump 445–1112 ft/s, median 638) |
| AIR/GROUND lags radio height | 8/8 | 102 (41–60 s, median 58) |
| partial start | 4/8 | 36 |
| partial end | 8/8 | 83 |
| time trust | UNTRUSTED 8/8 | UNTRUSTED 102 |
| every clock row invalid (one-digit hour) | — | 37 |
| elapsed gap | 0 | 1 (flight 47 of 2026-10-09) |
| required channels below 100 % | — | none |

**Longest identical run while the engine runs (N1 ≥ 40 %), maximum over 102 files:**

| Channel | Seconds |
|---|---|
| N1 | 8 |
| N2 | 11 |
| fuel flow | 853 |
| EGT | 289 |
| oil temperature | 464 |
| oil pressure | 1155 |
| vibration | up to 736 |
| TLA | 1705 |

**How much N1 moved while a channel stayed identical, runs of 30 s or more, maximum over 102 files** (`evidence/stuck_response_corpus.py`):

| Channel | N1 movement |
|---|---|
| EGT | 1.93 % |
| oil pressure | 7.42 % |
| N2 | never identical for 30 s |
| fuel flow | 27.8 % |
| oil temperature, vibration, imbalance angles, TLA, N1 target/bug/limit | 15–56 % |

- **Stuck-sensor candidates:** 2 × the measured values (§3). None fires on the 102 real files.
- **Grades on the 102 files:**

  | Rules | A | B | C | NOT_USABLE |
  |---|---|---|---|---|
  | seed as first written (jump blocks A) | 20 | 51 | 31 | 0 |
  | seed as published (confirmed jump allowed for A) | 70 | 1 | 31 | 0 |

  The one B is the elapsed gap of flight 47. The 31 C are flights with a partial start and a partial end.

## 5. Delta inventory planned
| Kind | Items |
|---|---|
| Configuration | QUALITY_RULES kind authorable, seed, validator |
| Domain | FlightQuality aggregate; quality evaluator, timeline, grader, clock parser |
| Tables | FlightQualities |
| Endpoints | 3: `GET`/`POST flights/{id}/quality`, `GET quality/aircraft/{aircraftId}` |
| Tests | golden parity 8/8, oracle parity on 102 real files, CLOCK-01..03, grades, validator, E2E in J3 |
