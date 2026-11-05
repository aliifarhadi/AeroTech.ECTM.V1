# P06 — API and screen contract

Conventions as in P00–P05. Security is deferred by the owner. 63 business routes (54 + 9).

## S9/S9a SNAPSHOT_RULES
The generic registry routes accept `SNAPSHOT_RULES`: drafts, validate (JSON paths), publish, retire and diff. The seed draft is created at startup. The document carries `title`, `description`, `calibrationStatus`, `engineeringSignOff` and the `decisions` register.

## S8 Stability points tab, R-F7, R-F5
| Event | Route | Response | Errors |
|---|---|---|---|
| stability points of the current revision | GET `flights/{id}/stability-points` | `ruleSet`, `status` (PROVISIONAL / FINAL), `positions`, `takeoffN1Peaks[]`, `takeoffEgtPeaks[]`, `cruise[{selector, availableEngineCount, engines[], paired}]` (below) | 404 80311; 404 80326 no stability points |
| phase metrics | GET `flights/{id}/phase-metrics?group=WHOLE_FLIGHT\|PHASE\|SEGMENT\|STEADY_CRUISE` | `groups[{kind, name, phase, segmentNumber, selector, position, startIndex, endIndexInclusive, parameters[{parameterKey, samples, valued, missing, gapStarts, masked, min, max, mean, median, percentile, standardDeviation}]}]` | as above |
| engine control context | GET `flights/{id}/control-context` | `verificationNote` (UNVERIFIED, I-02), `phases[{phase, samples, startIndex, endIndexInclusive, engineChannels[], modes[{parameterKey, value, firstIndex, samples}]}]` | as above |

**Takeoff N1 peak:** `position`, `availability`, `reason`, `searchBasis`, search window, `peakIndex`, `peakElapsed`, window, `validCount`, `validEgtCount`, `n1Peak`, `egtAtPeak`, `values[{parameterKey, count, mean, min, max}]`, `context{conditions[], toga[], configurations[]}`, `contextStatus`.

**Takeoff EGT peak:** `position`, `availability`, `reason`, `searchBasis`, `peakIndex`, `peakElapsed`, `egtPeak`, `offsetFromN1PeakSec`, `valuesAtPeak[]`.

**Cruise engine:**
- `position`, `availability`, `reason`, the block (start, end, seconds) and the report window
- `values[]`, `context`
- `gates{eligibleWindowEnds, rejectedWindowEnds[{gate, windowEnds}], reportWindowGates[{gate, actual, limit, passes}], nearestRejected[{windowEndIndex, failedGate}]}`

**Paired:** `minuendKey`, `subtrahendKey`, `availability`, `reason`, the joint block, the report window, `matchedSamples`, `meanDifference`.

## J5 single review
| Event | Route | Body / response | Errors |
|---|---|---|---|
| review one flight | POST `flights/{id}/review` | `{action: ACCEPT / EXCLUDE / HOLD / RELEASE / REQUEST_IDENTITY, reasonCode?, note?, analysisRevisionId?}` → flight detail. `reasonCode` is one of `ExclusionReason` (DATA_QUALITY_UNUSABLE, IDENTITY_UNCONFIRMED, DUPLICATE_RECORDING, INCOMPLETE_RECORDING, SENSOR_OR_RECORDER_FAULT, TEST_FERRY_OR_TRAINING_FLIGHT, ABNORMAL_OPERATION, ENGINE_EVENT_UNDER_INVESTIGATION, OTHER), required with EXCLUDE only | 400 EXCLUDE without a listed reason or a note, or a reason with another action; 409 80314 transition not allowed; 422 80324 no revision yet; 409 80325 stale revision |
| bulk review | POST `flights/bulk-review` | `{flightIds (≤ 200), action, reasonCode?, note?}` → `{action, applied, refused, items[{flightId, outcome: APPLIED / REFUSED / NOT_FOUND, flightState, errorCode, refusals[{code, kind, position}]}]}`. ACCEPT is checked against D-01 per flight. | 400 |
| history | GET `flights/{id}/revisions` | each revision now also has `snapshotRulesDocumentId/Version`, `engineAssignments` and `review{state, decidedBy, decidedUtc, reasonCode, note, entries[{action, actor, reasonCode, note, flightStateAfter, atUtc}]}` | 404 80311 |
| recompute an engine | POST `engines/{id}/recompute` | `{qualityRulesDocumentId?, phaseRuleSetDocumentId?, snapshotRulesDocumentId?}` → `{acceptedFlights, queued, flights[{flightId, outcome: QUEUED / UNCHANGED / ALREADY_PENDING, revision, processingJobId}]}` | 404 80206; 422 80323 |

`POST flights/{id}/recompute` also accepts `snapshotRulesDocumentId`.

## Reports (data)
| Report | Route | Response |
|---|---|---|
| R-E6 engine period performance | GET `engines/{id}/period-performance?kind=&selector=&from=&to=` | `flights`, `acceptedFlights`, `contributors`, `parameters[{parameter, n, mean, median, p10, p90, min, max}]`, `conditions[]`, `configurations{literal: count}`. Accepted flights only. |
| R-D9 fleet period performance | GET `engine-models/{id}/period-performance?kind=&selector=&from=&to=` | `engines[R-E6]`, `spread[{parameter, engines, lowestEngineMean, highestEngineMean, spread}]` |
| R-E7 engine data coverage | GET `engines/{id}/data-coverage?from=&to=` | `flightsReceived`, `flightStates[]`, `grades[]`, `availability[{kind, selector, available, notEvaluableReasons}]`, `missingChannels[{parameterKey, flightsNotRecorded, flightsBelowFullCoverage}]` |
| R-P04 review and audit | GET `reviews?from=&to=` | `byReviewer[{reviewer, action, count}]`, `entries[]`, `publications[{documentId, kind, versionNumber, publishedBy, publishedUtc, approvalNote}]` |

R-F1 (flight engine report) is assembled by the frontend from `flights/{id}`, `quality`, `phases`, `stability-points`, `events` and `revisions`.
