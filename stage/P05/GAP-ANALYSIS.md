# P05 — Gap analysis

## 1. Source lock
- Legacy `AeroTech.ECTM@0ad5f93`, unchanged and read only.
- V1 baseline: P00–P02 committed (`7b64401`); P03 and P04 uncommitted on top.
- Owner instruction for P05 (2026-11-05): choose the options that are most accurate by the pack and the benchmark, easiest to change after consulting the flight engineer, and transparent enough in code to be explained or changed on request; then implement.
- The development database was cleared by the owner during P05 ("دیتابیس پاک شد"); all migrations apply from scratch.

## 2. Sources read, and the decision for each
| Source | Decision | Reason |
|---|---|---|
| `reference/ectm_oracle.py` `phase()` (phase-v1.0.3) | EXPRESS EXACTLY as the PHASE_RULESET seed | It reproduces the 8 goldens. The C# evaluator runs the typed document and equals the oracle on all 102 real files. |
| `contracts/phase-ruleset-runtime-v1.json` (typed AST candidate) | USE AS THE SHAPE, NOT AS THE CONTENT | docs/28 §1 says it is unverified. It differs from the goldens in 6 points, listed in DECISIONS.md. Its operator names are kept where they fit. |
| `contracts/phase-ruleset-default-v1.json` (prose) | IGNORE | It is documentation; docs/04 §7 forbids evaluating its English strings. |
| `contracts/policy-defaults-v1.json → phase` | SEED VALUES | Every number of the seed comes from it (CANDIDATE). `minValid 3` is the oracle's fixed value. |
| docs/04, docs/28, docs/01 (AnalysisRevision, FlightEvents), docs/13 routes | IMPLEMENT | See API-SCREEN-CONTRACT.md. |
| legacy `FlightPhaseAssessmentAggregate` / `AnalysisRunAggregate.PhaseDetector` | IGNORE | Two generations with constants in code; the pack asks for a rule-set evaluator. See `docs/legacy-notes/phases.md`. |

## 3. Measured on all 102 real files of samples/ (pack oracle, `evidence/phase_corpus.py`)
| Fact | Value | Consequence |
|---|---|---|
| oracle reproduces the goldens | 8/8 | the oracle is the regression reference |
| lift-off basis | RADIO_HEIGHT_CROSSING 82, PARTIAL_START 20 | the PARTIAL_START fallback is needed |
| touchdown by the discrete | 102/102; exactly one AIR→GND change that holds 5 s per flight | FIRST and LAST give the same sample (P05-D02) |
| cruise found | 102/102 | |
| landing roll found | 93/102; in the other 9 the discrete reaches GND only at GS 18–37 kt | no landing roll is claimed there (P05-D07) |
| climb in the last 120 s | 0 flights | docs/14 P05 expectation holds on all files |
| UNKNOWN share | 1.23 %–18.56 % | the maximum is flight 2026-10-05_EP-DOA_14200: GS reads 335–365 kt on the ground for 868 s (P05-D10) |
| cruises shorter than 240 s | 4 segments in 3 flights; in the goldens only 9072 (213 s and 184 s) | the owner's draft (cruise 240 s) shows a diff on 9072, none on flight 24 (cruise 1402 s) |
| event literals (share of all samples) | TR Deployed 0.4 % per engine in 102 flights; TOGA Engaged 2.5 % in 102; A/T Disconnect 0.1 % in 100; A/T right Warning 0.8 % in 102; A/T left Warning 50.4 % in 102; Cutoff in 6 / 7 flights; fire and stick shaker only the inactive literal | P05-D11 |

## 4. Decisions taken
Every choice is written in the seed's `decisions` register (P05-D01 … P05-D13), with its basis, the alternative and how to change it, and linked to the parts it governs. The validator rejects a link to a part that does not exist. DECISIONS.md is the same list for reading.

Structure decisions (not engineering rules):
- **Closed operator library, one class per operator.** Each class holds its data, its meaning, its explanation trace and its validation. Adding an operator is code with tests and a version change; editing a rule is a document edit.
- **Order is the algorithm.** derived channels → anchors → conditions → label rules (order = precedence) → fallback → post-processing (in order) → segments → events.
- **AnalysisRevision now.** Quality and phases belong to one revision. Import creates revision 1. Recompute creates the next revision as a job; it becomes CURRENT when its job finishes, and the old revision and its results become SUPERSEDED.
- **Recompute is explicit** (docs/04 §5, docs/06): `dryRun` shows the result without moving the pointer; a real recompute is the engineer's confirmation. Recomputing with the versions already used is refused (80321).
- **`POST flights/{id}/quality` of P04 is removed** and replaced by `POST flights/{id}/recompute` (docs/13).
- **Events live in the PHASE_RULESET** because their spans need the phase at occurrence; they are stored with the phase result of the same revision.
