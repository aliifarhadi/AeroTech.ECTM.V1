# P04 — API and screen contract

Conventions as in P00–P03. Security is deferred by the owner.

## S9/S9a QUALITY_RULES
The generic registry routes of P02 now accept `QUALITY_RULES`: drafts, validate, publish, retire and diff. Publishing needs an active PARAMETER_CATALOG (`ACTIVE_CATALOG_REQUIRED`). The seed draft is created at startup.

## S8 Quality tab and R-F2
| Event | Route | Response | Errors |
|---|---|---|---|
| read the current result | GET `flights/{flightId}/quality` | see "Quality result" below | 404 80311 flight; 404 80318 not evaluated yet |
| evaluate with the active rules | POST `flights/{flightId}/quality` | the same result. With the same rules version it returns the stored result; with a new version it adds a result and supersedes the old one | 404 80311; 422 80116 no active QUALITY_RULES |

**Quality result:**
- `id`, `flightId`, `qualityRulesDocumentId`/`Version`
- `status` (PROVISIONAL), `grade` (A / B / C / NOT_USABLE), `gradeReasons[]`
- `sampleCount`, `elapsedGaps[]`, `partialStart`, `partialEnd`, `airGroundLagSec`, `firstRadioHeightAirborneIndex`, `firstAirFlagIndex`
- `discontinuities[{index, dAltFtPerSec, dRhFtPerSec, radioHeightBeforeFt, code}]`
- `spans[{code, startIndex, endIndexInclusive, parameterKeys, profileOrdinals, jumpIndex, cause, actual, threshold, unit, affects[]}]`
- `clock{disagreeRows, invalidOrMissingRows, primaryNotInStepRows, secondaryNotInStepRows, offsetHistogram, timeTrust}`
- `coverage[{parameterKey, recorded, sampleCount, valuedCount, percent}]`
- `findings[{severity, code, message, parameterKey, count, value}]`
- `evaluatedUtc`

The flight list, flight detail and batch rows carry the provisional `grade`.

## R-A3 Aircraft data-quality feedback
| Event | Route | Response | Errors |
|---|---|---|---|
| period summary for one aircraft | GET `quality/aircraft/{aircraftId}?from&to` | `flights`, `evaluated`, `grades[{grade, flights}]`, `untrustedTime`, `clockDisagreement`, `withDiscontinuity`, `partialStart`, `partialEnd`, `channelShortfalls[{parameterKey, flightsNotRecorded, flightsBelowFullCoverage}]`, `rows[{flightId, flightDate, flightNumber, grade, timeTrust, discontinuityCount, partialStart, partialEnd}]` | 404 80200; 400 `to` before `from` |
