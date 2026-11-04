# P01 — API and screen contract

Conventions:
- **Base path:** `Backoffice/v1/`. Envelope, ids-as-strings, `EnumValueDto` and paging are as in P00.
- **Timestamps:** `*Utc` fields are ISO 8601 and are stored in UTC.
- **Status codes:** 201 on create, 200 otherwise.
- **Security:** deferred by the owner, so every route is open. The intended policy is Viewer for GET and Engineer for writes.

## S4 — Aircraft
| Screen event | Route | Request | Response | Errors |
|---|---|---|---|---|
| list | GET `aircraft?search&status&pageNumber&pageSize` | — | page of Aircraft | 400 paging |
| create | POST `aircraft` | `registration` (≤16, required), `typeCode?`, `operatorCode?` (≤64) | Aircraft | 409 80201 registration exists; 400 validation |
| detail | GET `aircraft/{aircraftId}` | — | Aircraft | 404 80200 |
| edit | PUT `aircraft/{aircraftId}` | `typeCode?`, `operatorCode?`, `status` | Aircraft | 404 80200; 400 |

Aircraft = `{id, registration, typeCode, operatorCode, engineCount, status, currentInstallations[Installation]}`.

## S5 — Engine models
| Screen event | Route | Request | Response | Errors |
|---|---|---|---|---|
| list | GET `engine-models?search&pageNumber&pageSize` | — | page of EngineModel | 400 |
| create | POST `engine-models` | `code` (required, ≤64), `manufacturer` (required, ≤128), `family?`, `variantCode?`, `thrustRatingCode?` (≤64), `documentReferences[]` (≤256 each), `notes?` (≤2000) | EngineModel | 409 80204 code exists; 400 |
| detail | GET `engine-models/{engineModelId}` | — | EngineModel | 404 80203 |
| edit | PUT `engine-models/{engineModelId}` | all fields except `code`; `manufacturer` required | EngineModel | 404 80203; 400 |

## S6 — Engines (ESN)
| Screen event | Route | Request | Response | Errors |
|---|---|---|---|---|
| list | GET `engines?search&engineModelId&status&pageNumber&pageSize` | — | page of Engine | 400 |
| create | POST `engines` | `serialNumber` (required, ≤64), `engineModelId`, `status`, `variantCode?`, `thrustRatingCode?`, `configurationEvidenceReference?` (≤256), `lifeCounters?` {`hoursSinceNew?`, `cyclesSinceNew?`, `hoursSinceLastEvent?`, `cyclesSinceLastEvent?`, `basisUtc`} | EngineDetail | 404 80203 model; 409 80207 serial exists for the manufacturer; 422 80213 variant/rating without evidence; 422 80214 counters invalid |
| detail | GET `engines/{engineId}` | — | EngineDetail | 404 80206 |
| edit | PUT `engines/{engineId}` | `status`, `variantCode?`, `thrustRatingCode?`, `configurationEvidenceReference?` | EngineDetail | 404 80206; 422 80213 |
| set life counters | PUT `engines/{engineId}/life-counters` | counter set as above + `reason` (required, ≤256) | EngineDetail | 404 80206; 422 80214; 400 |

- **Engine:** `{id, serialNumber, manufacturer, engineModelId, engineModelCode, status, variantCode, thrustRatingCode, configurationEvidence {reference, documentedUtc, documentedBy} | null, lifeCounters {hoursSinceNew, cyclesSinceNew, hoursSinceLastEvent, cyclesSinceLastEvent, basisUtc, source} | null, currentInstallation | null}`.
- **EngineDetail:** Engine plus `lifeCounterAdjustments[{id, counters…, source, reason, adjustedUtc, adjustedBy}]`, oldest first.

## S6 › Installations
| Screen event | Route | Request | Response | Errors |
|---|---|---|---|---|
| timeline | GET `installations?engineId&aircraftId&position&openOnly&pageNumber&pageSize` | — | page of Installation, newest first | 400 |
| install | POST `installations` | `engineId`, `aircraftId`, `position` (1 or 2), `installedUtc`, `reason` (required, ≤256), `evidence?` (≤256) | Installation | 404 80206 / 80200; 422 80209 position; **409 80211 overlap, naming the conflicting installation (id, ESN, registration, position, period)** |
| close | POST `installations/{installationId}/close` | `removedUtc`, `reason` (required) | Installation | 404 80210; 422 80212 removal not after installation; 409 80215 already closed |

Installation = `{id, engineId, engineSerialNumber, aircraftId, aircraftRegistration, position, installedUtc, removedUtc, reason, evidence, changes[{changeType, reason, changedUtc, registryVersion}]}`.

## S6 › Maintenance
| Screen event | Route | Request | Response | Errors |
|---|---|---|---|---|
| list | GET `maintenance-events?engineId` or `?aircraftId&position`, `&pageNumber&pageSize` | — | page of MaintenanceEvent, newest first | 400 |
| create | POST `maintenance-events` | either `engineId`, or `aircraftId` + `position` (POSITION_ONLY); `eventType`, `eventUtc`, `dateReliability`, `hoursAtEvent?`, `cyclesAtEvent?`, `workOrderReference?` (≤128), `notes?` (≤2000) | MaintenanceEvent | 404 80206 / 80200; 422 80216 identity invalid; 422 80217 counters invalid |
| edit | PUT `maintenance-events/{maintenanceEventId}` | the same fields except identity | MaintenanceEvent | 404 80218; 422 80217 |

MaintenanceEvent = `{id, identityKind, engineId, engineSerialNumber, aircraftId, aircraftRegistration, position, eventType, eventUtc, dateReliability, hoursAtEvent, cyclesAtEvent, workOrderReference, notes}`.

## Shared errors
- **409 80000:** another operation on the same resource is in progress.
- **409 80001:** the resource changed concurrently.
