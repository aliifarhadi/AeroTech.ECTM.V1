# P05 — Out of scope

| Item | Where it goes |
|---|---|
| stability windows, snapshots, phase metrics | P06 |
| `POST engines/{id}/recompute` (all accepted flights of an engine) | P06, when flight acceptance exists |
| flight review and its invalidation on a new revision; identity change creating a revision; engine assignments in the revision | P06 (review state machine) |
| calibration of the phase thresholds (CANDIDATE) | P13 |
| engineering sign-off of PHASE_RULESET | the flight engineer; recorded as PENDING (P05-D13) |
| fallback touchdown for late discretes (9 of 102 flights without a landing roll) | the flight engineer (P05-D07) |
| GS plausibility on the ground (flight 14200) | the flight engineer (P05-D10) |
| fire, stick shaker, left A/T warning events | when the recorder documentation names the active literal (P05-D11) |
| event rates with SOP context | P16 (R-D6 is descriptive now) |
| human sentences generated from the AST in the viewer | frontend S9 (the explanation already carries every operand) |
| R-F3, R-F4, R-D6 report screens and exports | P11 (the data routes exist now) |
| security | deferred by the owner |
| frontend S8 Phases / Events / History tabs, S9a for PHASE_RULESET | frontend repository |
