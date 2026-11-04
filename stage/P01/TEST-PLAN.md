# P01 — Test plan

**Three gates:** not applicable. P01 has no computed values.

**How to run:** as in P00. A throwaway database is used per run. Distributed locks are replaced by an in-memory fake.

| Area | Positive | Negative | Project |
|---|---|---|---|
| Aircraft | create, read, list with search, edit status | duplicate registration → 409 80201; blank registration → 400; unknown id → 404 | Acceptance |
| Engine model | create with documents, edit, list | duplicate code → 409 80204; missing manufacturer → 400 | Acceptance |
| Engine | create with initial counters → one adjustment; configuration evidence stamped | duplicate serial for the same manufacturer → 409 80207; variant without evidence → 422 80213; unknown model → 404 | Acceptance + domain |
| Life counters | PUT appends an adjustment with reason; audit holds an UPDATED Engine entry with before/after counters and a CREATED adjustment entry | negative value → 422 80214; missing reason → 400 | Acceptance + domain |
| Installation | install ESN-1 on position 1; swap: close at T, install ESN-2 at T on the same slot; history lists both with RegistryChanges and increasing RegistryVersion | overlap on the same position → 409 80211, naming the conflicting id, ESN, registration, position and period; the same engine on two aircraft → 409 80211; removal ≤ installation → 422 80212; close twice → 409 80215; position 3 → 422 80209 | Acceptance + domain (half-open boundaries) |
| Maintenance | ENGINE event; POSITION_ONLY event (aircraft + position, no engine); list by engine and by position; edit | engine and position together → 422 80216; neither → 422 80216; negative hours → 422 80217 | Acceptance + domain |
| Read model | aircraft shows its current installations; engine shows its current installation; display labels (ESN, registration) next to ids | — | Acceptance |
| Route inventory | 4 P00 routes + 17 P01 operations exactly | — | Acceptance |
| Regression | P00 suites unchanged | — | both |

**E2E walkthrough (J1, API):** EP-DOA, test model, two test-labelled ESNs → install both → close ESN-1 and install a third ESN in the same slot → refused overlap → WASH marker → read history.
