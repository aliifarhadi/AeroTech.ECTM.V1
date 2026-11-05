# P05 — Decisions and difference dossier

The source of truth is the `decisions` register in the PHASE_RULESET seed (`src/AeroTech.ECTM.Domain/ConfigurationDocumentAggregate/Seeds/phase-ruleset-ep-doa-seed.json`). This page is the same list for reading. Status of every decision: **CANDIDATE, engineering sign-off PENDING**.

## How to change a decision
1. Create a draft of PHASE_RULESET from the active version.
2. Edit the part named under "How to change" (and the decision text).
3. Validate: errors name the JSON path.
4. Dry-run the draft on flights: `POST config/PHASE_RULESET/documents/{id}/dry-run/phases`. It shows the changed samples, the changed intervals and the anchors.
5. Publish with an approval note.
6. Recompute the flights: `POST flights/{id}/recompute` creates a new revision.

No code changes, unless a new operator is needed.

## Difference dossier: pack runtime AST vs goldens (docs/04 §7)
| # | Part | Pack AST | Goldens / oracle (chosen) | Effect on the goldens if the AST were used |
|---|---|---|---|---|
| D02 | touchdown | LAST sustained AIR→GND | FIRST after lift-off | none on the 102 real files (one change each) |
| D03 | ground | no AIR_GROUND test; null rate allowed | AIR_GROUND = GND; valid rate required | ground before lift-off and after touchdown would change |
| D04 | airborne | speeds without the discrete | speeds only while AIR | samples on the runway would count as airborne |
| D05 | takeoff roll | from the last GS < 40 kt to lift-off | walk back while GS ≥ 40, RH ≤ 50, not ground, ≤ 120 s | roll start differs |
| D06 | takeoff | whole window | ground samples excluded | |
| D08 | partial start | AIR or RH > 20 ft | quality result: RH > 20 ft at the first sample | |

## Register
| Id | Topic | Choice | Basis | Alternative | How to change |
|---|---|---|---|---|---|
| P05-D01 | Reference algorithm | the oracle `phase()` of phase-v1.0.3, exactly | reproduces the 8 goldens; the AST is unverified | the AST as written | edit the parts of D02–D08 |
| P05-D02 | Touchdown | FIRST AIR→GND that holds 5 s, after lift-off | goldens; one such change in every real file | LAST | `anchors` TOUCHDOWN `pick` = LAST |
| P05-D03 | Ground evidence | AIR_GROUND = GND and a valid smoothed rate | goldens | no discrete test; null rate allowed | `conditions` GROUND |
| P05-D04 | Airborne evidence | speeds count only while AIR; RH > 50 ft with CAS ≥ 90 kt alone | goldens | speeds without the discrete | `conditions` AIRBORNE |
| P05-D05 | Takeoff roll | walk back while GS ≥ 40, RH ≤ 50, not ground, ≤ 120 s | goldens | from the last GS < 40 kt | `conditions` TAKEOFF_ROLL |
| P05-D06 | Takeoff | lift-off to +120 s without ground samples | goldens | whole window | `conditions` TAKEOFF `where` |
| P05-D07 | Late landing roll | only if GS ≥ 80 kt at touchdown; no fallback | goldens; 9 of 102 real flights reach GND at 18–37 kt | a fallback touchdown after the engineer's review | `conditions` LANDING_ROLL `startWhen`, or a new anchor |
| P05-D08 | Partial start | from the quality result (RH > 20 ft at the first sample) | goldens; 20 of 102 start airborne | AIR or RH > 20 ft | QUALITY_RULES `partial`, or `fallbackBases` |
| P05-D09 | Thresholds | policy-defaults-v1.json → phase; `minValid` 3 | pack values, CANDIDATE until P13 | engineer's values | edit the number |
| P05-D10 | Invalid GS at power-up | no GS plausibility gate; such samples stay UNKNOWN | flight 14200: 335–365 kt on the ground for 868 s | a GS-vs-CAS test after review | add a predicate to GROUND |
| P05-D11 | Events | observed active literals; fire, stick shaker and left A/T warning not evaluated; AT_ENGAGED is a state | literal shares on 102 real files | enable when the recorder documentation names the literal | `enabled`, `activeValue` |
| P05-D12 | Evidence codes | the oracle's codes, which quote default thresholds | goldens | codes without numbers | `segments.evidenceCodes` with the threshold |
| P05-D13 | Engineering sign-off | PENDING | never inferred automatically | APPROVED after review | `engineeringSignOff` in a new version |

## Decisions in code (not engineering rules)
| Decision | Where |
|---|---|
| A recompute with the versions already used is refused (80321); one pending recompute per flight (80322) | `RecomputeFlightService` |
| A revision always holds quality; phases only when a PHASE_RULESET was active or chosen | `AnalysisRecorder` |
| R-D6 counts a landing when the TOUCHDOWN anchor is found; rates = flights with the event per 100 evaluated flights and per 100 evaluated landings, with the denominators shown | `GetAircraftEventRatesService` |
| No hand-set algorithm label. The system compares the rules with the pack reference (the seed, proven equal to the oracle) and reports the match and every changed rule; display text and status are ignored in the comparison. | `PhaseReference` (owner request 2026-11-05: "a new user must understand it") |
| `engines/{id}/recompute` waits for flight acceptance (P06); identity changes do not create a revision before P06 | OUT-OF-SCOPE.md |
