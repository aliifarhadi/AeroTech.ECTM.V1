# P06 — Gap analysis

## 1. Source lock
- Legacy `AeroTech.ECTM@0ad5f93`, unchanged and read only.
- V1 baseline: P00–P02 committed (`7b64401`); P03–P05 uncommitted on top.
- The owner started P06 on 2026-11-05: "start; keep every earlier rule; clean, separated and human-readable code matter most". This is taken as the P05 gate.

## 2. Sources read, and the decision for each
| Source | Decision | Reason |
|---|---|---|
| `reference/ectm_oracle.py` `snapshots()` takeoff and cruise | PORT exactly | It reproduces the 8 goldens; the C# result equals it on all 102 real files. |
| oracle paired value | REPLACE with the docs/05 §2.5 joint-block rule | docs/17 §3 names it a defect; the frozen `fixtures/independent/paired-joint-block-v1.json` is the reference (24/24 records). |
| `contracts/policy-defaults-v1.json → snapshots` | SEED | It becomes the SNAPSHOT_RULES seed, CANDIDATE. |
| docs/05 §3 phase metrics | IMPLEMENT | It is descriptive and stored with the stability points of the revision. |
| docs/06 §4, docs/11 J5, docs/13 review routes | IMPLEMENT | One FlightReview per revision; bulk acceptance under D-01. |
| docs/10 R-F1, R-F5, R-F7, R-E6, R-E7, R-D9, R-P04 | DATA ROUTES | The report screens are P11; R-F1 is assembled from the flight routes. |
| docs/05 §3a GROUND_IDLE / GROUND_MPA | NOT IN P06 | docs/14 places the ground observations with the OEM evidence (P08). |
| legacy snapshot and review code | IGNORE | See `docs/legacy-notes/stability-points-and-review.md`. |

## 3. Measured on all 102 real files of samples/ (pack oracle, `evidence/snapshot_corpus.py`)
| Fact | Value | Consequence |
|---|---|---|
| oracle reproduces the snapshot goldens | 8/8 | the oracle is the regression reference |
| takeoff available | 102/102 for both engines; 82 anchored on lift-off, 20 (engine 1) partial start | |
| takeoff context unverified | engine 1: 86, engine 2: 74, all only from the AIR/GROUND lag mask | P06-D04 |
| cruise availability, engine 1 / engine 2 | STRICT 8 / 12; OPERATOR_300S 93 / 94; OPERATOR_60S 102 / 102 | P06-D05: all three are kept, no default |
| unavailable reason | always NO_ELIGIBLE_BLOCK | |
| Python summation | the oracle runs on Python 3.14, whose `sum` is compensated | P06-D11 |
| centring of report windows | engine windows: centre − R/2 (goldens); paired window: start + (length − R)/2 (independent fixture) | P06-D10 |

## 4. Decisions taken
- The engineering choices are in the SNAPSHOT_RULES `decisions` register (P06-D01 … P06-D12), validated like PHASE_RULESET. DECISIONS.md is the same list for reading, plus the application-level decisions.
- **Structure:**
  - **One small computation class per step:** TakeoffSearch, TakeoffN1PeakFinder, TakeoffEgtPeakFinder, CruiseWindowGates, CruiseBlockFinder, PairedComparer, PhaseMetricsCalculator, ControlContextCalculator, StabilityPointsEvaluator.
  - **One frame** (`SnapshotFrame`) gives every step its data, masks and phases.
  - **The validation helper is shared** by the rule documents (`RuleDocumentCheck`, renamed from PhaseRuleSetCheck), as are the decisions register (`RuleDecisions`) and the document header (`RuleDocumentHeader`).
- **Revision:** it now also holds the SNAPSHOT_RULES version and the engine assignments. A recompute with the same rules and the same engines is refused (80321).
- **Review:** FlightReview per revision with an entry per action; ACCEPT makes the revision's results FINAL; leaving ACCEPTED makes them PROVISIONAL again; a new revision supersedes the review and returns the flight to PENDING_REVIEW.
