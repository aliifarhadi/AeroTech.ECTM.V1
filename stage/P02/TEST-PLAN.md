# P02 — Test plan

**Three gates:** see SCIENTIFIC-CONTRACT.md.
- **Correctness gate:** hand-calculated vectors.
- **Regression gate:** not applicable.
- **Approval gate:** pending.

| Pack requirement | Test | Project |
|---|---|---|
| 85 ordinals covered once | `The_profile_covers_each_of_the_85_ordinals_exactly_once`; negatives: duplicated ordinal, missing column | Conformance |
| roles valid | catalog seed valid; unknown role, unit and value kind refused | Conformance |
| units reconcile | difference of different units; ratio of equal units not RATIO; profile unit mismatch; unknown unit; conversion other than IDENTITY | Conformance |
| duplicate-header binding | the two UTC columns at ordinals 1 and 5 bind by occurrence; swapped occurrences refused | Conformance |
| column swap mismatch | swapped header columns → signature mismatch, nothing parsed (domain and HTTP) | both |
| collisions blocked | two columns on N1_PCT[1] → `COLUMN_COLLISION` naming the first ordinal | Conformance |
| derived-parameter vectors | 46 vectors over DIFFERENCE, RATIO, LINEAR_COMBINATION and POWER_RATIO, plus explanation and bit-determinism | Conformance |
| no free-form expression | unknown kind, field outside the kind (`formula`), and kinds of later phases refused | Conformance + Acceptance |
| real data | profile headers equal the real CSV header byte-for-byte; preview of flight 24 shows 3987 rows, elapsed 99095…103081, columns 64/77/78 missing not zero | Conformance + Acceptance |
| any edit creates a new version | PUT on a published document → 409 80106; new version from active → v n+1 | Acceptance |
| both kinds active | J2 walkthrough: catalog and profile active → `parserReady`; add EGT_OVER_N2 → validate → evaluate 600/96 = 6.25 → publish → diff shows one ADDED → readiness still ready | Acceptance |
| publish gates | missing note 400; invalid draft 422 80108 with issues; catalog breaking the active profile → `ACTIVE_PROFILE_INCOMPATIBLE`; retire a draft 409; clone from another kind 422 | Acceptance |
| audit | publish writes UPDATED with status and note only, no content | Acceptance |
| route inventory | 33 operations exactly | Acceptance |
