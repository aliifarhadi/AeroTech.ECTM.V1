# P03 — API and screen contract

Conventions as in P00–P02. Security is deferred by the owner; the intended policies are Viewer for reads and Engineer for writes. Sample index = 0-based data row of the source file.

## S2 Imports and S3 Import batch
| Event | Route | Request | Response | Errors |
|---|---|---|---|---|
| upload and preview | POST `imports/preview` (multipart) | `files[]` (CSV, XLSX, ZIP, RAR), `defaultRegistration?`, `defaultYear?` | Batch with items. Each item: `fileName`, `sha256`, `sizeBytes`, `outcome` (NEW / DUPLICATE / REJECTED), `rejectionCode?`, `existingFlightId?`, identity preview (`registration`, `flightDate`, `flightNumber` with sources, `aircraftId`/`aircraftRegistration`, positions 1 and 2 with `assignment` (ENGINE / POSITION_ONLY / NEEDS_CONFIRMATION), `engineId`/`engineSerialNumber`, `identityStatus`, `reasons[]`), `sampleCount`, `firstElapsed`, `lastElapsed`, validation `{severity, code, message, parameterKey?, count?, value?}` with B/W/I counts, and the first and last 10 rows of mapped values. The batch also carries the pinned `dataProfileDocumentId`/version and `parameterCatalogDocumentId`/version. | 422 80116 no active profile or catalog; 422 80301 unsupported type; 422 80302 too large; 422 80303–80306 archive unreadable, encrypted, too many members, expands beyond the limit; 400 no file |
| commit | POST `imports/commit` | `batchId`, `idempotencyKey` | Batch (state COMMITTING) with `jobId` | 404 80300 batch; 409 80307 committed with another key |
| progress | GET `jobs/{jobId}` | — | `{id, kind, state, processed, total, errorCode, createdUtc, completedUtc}` | 404 80310 |
| batch list | GET `imports?pageNumber&pageSize` | — | page of batch summaries (counts per outcome and state) | 400 |
| batch page | GET `imports/{batchId}` | — | batch and items. After commit each item has `flightId`, `flightState` and the flight's identity (ESN1, ESN2). | 404 80300 |

## S9a Catalog dry-run on a flight (owner decision 2026-11-05)
| Event | Route | Request | Response | Errors |
|---|---|---|---|---|
| dry-run a catalog version (draft or published) on an imported flight | POST `config/PARAMETER_CATALOG/documents/{documentId}/dry-run/flight` | `flightId`, `budget?` (4–20,000, default 2,000) | The flight's source file is imported again with its pinned DATA_PROFILE and this catalog, and nothing is stored. The response holds `configurationIssues` (profile incompatible with this catalog), `sampleCount`, the pinned and dry-run versions, the active catalog id and version, `findings[]` (each finding against the flight's stored one: `change` ADDED / REMOVED / CHANGED / UNCHANGED, stored and dry-run severity, count and value), `derivedParameters[]` (per key and position: `againstActive` ADDED / UNCHANGED / CHANGED, `availableCount`, `statusCounts`, `minimum`, `maximum`, the first available formula, and M4 `points` and `gaps` for the raw viewer) and `removedFromActive[]` | 404 80101 document; 404 80311 flight; 422 80108 invalid catalog; 422 80111 for another kind (the profile keeps its file dry-run); 400 budget |

## S7 Flights list and S8 Workbench Raw
| Event | Route | Request | Response | Errors |
|---|---|---|---|---|
| list | GET `flights?state&aircraftId&engineId&from&to&batchId&identityStatus&pageNumber&pageSize` | — | page of flight summaries (date, registration, flight number, ESN1, ESN2, state, identity status, sample count, B/W/I counts) | 400 |
| detail | GET `flights/{flightId}` | — | identity with sources and reasons, engine assignments (installation id, ESN), state, hold info, source file (name, sha256, size, sourceType), versions (profile and catalog document ids and versions), validation issues, channels (parameterKey, unit, valueKind, profileOrdinal (provenance: the DATA_PROFILE column; docs/16 allows ordinals only in the parser and provenance), missing and unparseable counts) | 404 80311 |
| identity dialog (S3a, J4) | POST `flights/{flightId}/identity` | `aircraftId`, `flightDate`, `flightNumber`, `engines[{position, engineId?, positionOnly}]`, `confirmDistinctFlight`, `note?` | detail (identity source USER, RESOLVED, PENDING_REVIEW) | 404; 422 80312 engine not installed on that aircraft and position at that date; 422 80313 a possible duplicate needs confirmation; 409 80314 state does not allow it |
| review | POST `flights/{flightId}/review` | `action` (HOLD / RELEASE / REQUEST_IDENTITY; ACCEPT and EXCLUDE come in P06), `note?` | detail | 409 80314 `REVIEW_NOT_ALLOWED(state)`; 422 80315 action delivered in P06 |
| raw chart | GET `flights/{flightId}/series?parameters=EGT_C[1],N1_PCT[1]&sampling=M4\|RAW&from&to&budget` | — | per parameter: `parameterKey`, `unit`, `sampling`, `sampleCountInRange`, `points[{sampleIndex, elapsed, value}]` with a null point at every gap or missing value (never bridged), `gaps[{afterSampleIndex, elapsedFrom, elapsedTo}]` | 404; 422 80316 unknown or non-numeric parameter; 422 80317 RAW range above 20,000 samples; 400 range, budget (4–20,000, default 2,000) or more than 16 parameters |
| raw rows drawer | GET `flights/{flightId}/samples?from&to` (at most 500 rows) | — | `header[85]` (exact text), `rows[{sampleIndex, cells[85]}]` raw text from the source bytes | 404; 400 |
