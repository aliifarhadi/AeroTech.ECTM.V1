# STAGE ACCEPTANCE — P04 / Data quality (backend)

## Source lock
- **Backend:** `AeroTech.ECTM.V1`, uncommitted on top of `7b64401` (P02). It contains P03 and P04.
- **References read:** GAP-ANALYSIS §2 and `docs/legacy-notes/data-quality.md`.
- **Migration:** `AddFlightQuality` adds the table FlightQualities and alters nothing. It is applied to `DotAirEctmV1` (5 migrations). No model change is pending.
- **Report, screen and route IDs:** R-F2 and R-A3 data, S8 Quality and S9/S9a for QUALITY_RULES, and 3 new operations (48 in total).

## Basis of thresholds and rules (owner ruling)
- **Seed:** the QUALITY_RULES seed copies `policy-defaults-v1.json` (CANDIDATE).
- **Measurement:** every rule was measured on all 102 real files in `samples/` with the pack oracle (GAP-ANALYSIS §4).
- **Grades on the 102 files with the published seed:** A 70, B 1 (the elapsed gap), C 31 (partial start and end), NOT_USABLE 0.
- **Owner rulings in the reply to the first P04 report (2026-11-05):**
  1. Publish the QUALITY_RULES seed: done.
  2. Accept `H:MM:SS` clocks: `clock.acceptOneDigitHour = true`. No real file now has an invalid clock row. The goldens are unchanged, because the parity runs in the oracle format.
  3. Grade A and the altitude jump, left to the agent: the jump confirmed by both channels is allowed for A; a single-channel jump still gives B.
  4. Stuck sensor, left to the agent with a preference for a resolution-aware rule:
     - each channel has a duration and, where needed, a minimum N1 movement during the run
     - N1 16 s; N2 22 s; EGT 30 s with N1 moving 4 %; oil pressure 30 s with N1 moving 15 %
     - all values are 2 × the measured ones, and none fires on real data
     - channels that do not follow N1 get no rule

## Independent scientific contract
See SCIENTIFIC-CONTRACT.md.

| Gate | Result |
|---|---|
| Regression: quality sections of the 8 goldens, field by field | PASS 8/8 |
| Independent: pack oracle over 102 real files = C# result, field by field | PASS 102/102 |
| Approval | PENDING: thresholds are CANDIDATE (P13) |

## Reproducible E2E (what the owner clicks)
1. Publish the QUALITY_RULES seed draft (registry, after catalog and profile).
2. Import, as in J3.
3. Every batch row shows a provisional grade.
4. `GET flights/{24}/quality` shows:
   - `ALTITUDE_DISCONTINUITY_CONFIRMED` at 3887 and the mask 3867–3917, with 663 ft/s against 300 ft/s and the downstream use `PHASE_ALTITUDE_RATE`
   - AIR/GROUND lag 57 s
   - clock counters with no invalid row (one-digit hours accepted), time UNTRUSTED
   - grade C, reason `PARTIAL_START_AND_END`
5. `POST flights/{24}/quality` again → the same result (same rules version).
6. `GET quality/aircraft/{EP-DOA}?from=2026-09-24&to=2026-10-01` → 8 flights, 8 evaluated, 8 untrusted, 8 with a discontinuity.

Test: `Flows/ImportWalkthroughTests` (J3 extended).

**Negative paths, all tested:**
- a reordered time axis → NOT_USABLE with `TIME_AXIS_NOT_STRICTLY_INCREASING`, no numbers
- a missing radio height → `QUALITY_INPUT_MISSING`, coverage BLOCKING, partial start null, NOT_USABLE
- CLOCK-01..03
- a broken QUALITY_RULES document → named JSON paths
- a later-phase kind (PHASE_RULESET) still → 422 80105

**Development database:**
- The migration is applied.
- **QUALITY_RULES v1 is published (owner instruction):**
  - The draft your host had created from the earlier seed was replaced with the current seed through the API, validated with 0 issues, and published with the approval note "Owner instruction 2026-11-05 (reply to the P04 report) …".
  - This used a separate host on port 5199, stopped afterwards. `evidence/config-kinds-after-publish.json` holds the result.
- No flight was evaluated on your behalf. `POST flights/{id}/quality` evaluates the already imported flight; new imports are evaluated on commit.
- The running host must be restarted on this build.

**Other:**
- 401/403: NOT DELIVERED (security deferred by the owner).
- Tests: `evidence/conformance-tests.txt` 240/240 and `evidence/acceptance-tests.txt` 80/80.

## Delta inventory
| Kind | Added in P04 |
|---|---|
| Configuration | QUALITY_RULES authorable: seed, typed content, validator (catalog-checked) |
| Domain | FlightQuality aggregate (PROVISIONAL, superseded by a new rules version); evaluator, timeline, grader, clock parser |
| Contracts | 3 enums: QualityGrade, TimeTrust, ResultStatus |
| Tables | FlightQualities |
| Endpoints | 3: `GET`/`POST flights/{id}/quality`, `GET quality/aircraft/{aircraftId}`; grade added to the flight list, detail and batch rows |
| Tests | 41 new conformance tests; acceptance J3 extended; 2 P02 tests now use PHASE_RULESET as the later-phase example |

## Deviations
- **Carried over:** Consumers kept, Framework envelope, Ordering query pattern, no security.
- **New in P04:**
  - Time trust without the gap term, as docs/17 §3 orders.
  - The stuck sensor is per channel with an N1-response test (measured), not one N.
  - One-digit clock hours are valid (owner ruling); the oracle format is kept for the parity tests.
  - The confirmed recorder altitude jump does not block grade A (agent decision, configurable).
  - Results are stored as JSON with key columns.

## Stage closure
- **`SOFTWARE_VERIFIED`:** PASS (320/320 tests). Security remains deferred.
- **`ENGINEERING_APPROVED`:** PENDING. Thresholds are CANDIDATE.
- **`OPERATIONAL_STATUS`:** quality AVAILABLE in `DotAirEctmV1` (QUALITY_RULES v1 active) once the host runs this build.
- **Open owner decisions and inputs:**
  1. **Still open:** preview as a job; publish gate; I-01, I-02; THETA and DELTA; fixture publication; package vulnerability.
- **Owner signature and gate to start P05:**
