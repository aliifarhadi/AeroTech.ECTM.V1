# P01 — Out of scope

| Item | Where it goes |
|---|---|
| report R-D5 and the S12 library, viewer and export | P11 (docs/14 P01 "NO S12 report stub"; see GAP-ANALYSIS §2) |
| maintenance "reset flag" (J1, S6), meaning opening a new baseline period | P07; docs/01 and `policy-defaults` say maintenance is a marker only and never resets anything automatically |
| computed life counters (source COMPUTED, from accepted flights) | after P06, when flights are accepted |
| aircraft flights table, position-only trends, engine tabs Overview, Stability points, Baselines, Trends, Alerts | P03–P14 |
| engine-model tabs Correction models, Fleet reference, Pattern table | P08, P12, P14 |
| security (policies, 401/403) | deferred by the owner |
| frontend screens S4–S6 | frontend repository |
| real fleet data (I-01) | owner input; the demo uses test-labelled ESNs until it is supplied |
