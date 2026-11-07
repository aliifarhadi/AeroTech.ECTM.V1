# P07 — API contract for the frontend agent

**Envelope:** prefix `/Backoffice/v1`, the Framework `ApiResult {data, errors[{code,title,detail,metadata}]}`.

**Wire format:**
- Every `long` id is a decimal string.
- Enums are sent as UPPERCASE strings and returned as `{value, name, title}`.
- Swagger on the running host is the field-level authority.

**Screens:** Engine › Baselines tab, Engine › baseline trend chart, Flight › Baseline comparison.

## Routes (13)
| Method | Route | Body / query | Returns | Notes |
|---|---|---|---|---|
| POST | `engines/{engineId}/baselines/prefill-file` | multipart: `file` (one .csv or .xlsx), `enginePosition` (1 or 2), `shopVisitEventId?` | `BaselinePrefill` | Stores nothing. 400 80406 means the file is rejected, with the P03 reason. |
| GET | `engines/{engineId}/baselines` | | `EngineBaselines` | The list, each parameter's `isPrimary`, and the latest evaluation job |
| POST | `engines/{engineId}/baselines` | `BaselineDraft` | 201 `Baseline` | Manual or prefilled; same body |
| GET | `baselines/{id}` | | `Baseline` | |
| PUT | `baselines/{id}` | `BaselineDraft` + `expectedDraftRevision` | `Baseline` | Replaces the whole draft. 409 80401 if not a draft, 409 80403 if stale. |
| POST | `baselines/{id}/preview` | `{flightId?}` | `BaselinePreview` | Pure. Without `flightId` it uses the latest accepted flight of the engine. |
| POST | `baselines/{id}/publish` | `{expectedDraftRevision, confirmedRawReference: true}` | `{baseline, evaluation: Job}` | 422 80404 lists the issues. 400 when the confirmation is missing. |
| POST | `baselines/{id}/clone` | | 201 `Baseline` | A new draft of the same lineage, with `revisionNumber`+1 |
| POST | `baselines/{id}/retire` | `{reason}` | `Baseline` | Removes the baseline's primaries |
| PUT | `engines/{engineId}/baselines/primary` | `{baselineParameterId?, snapshotKind?, selector?, parameterCode?, expectedCurrentPrimaryId?}` | `EngineBaselines` | Set it by `baselineParameterId`. Clear it with the kind, selector and code. 409 80408 if `expectedCurrentPrimaryId` is stale. |
| POST | `baselines/{id}/reconcile` | | 202 `Job` | Repairs a failed or missed evaluation. Track it with `GET jobs/{jobId}`. |
| GET | `flights/{flightId}/baseline-comparisons` | `baselineIds` (repeat), `snapshotKind`, `selector`, `parameterCode` | `FlightBaselineComparisons` | One row per baseline parameter and engine |
| GET | `engines/{engineId}/baseline-trend` | the same filters plus `from`, `to`, `pageNumber`, `pageSize` | `PaginatedList<Evaluation>` | Ordered by flight date, then flight id |

## Shapes

**`BaselineDraft`:**
- Fields: `name`, `declaredEffectiveDate?` (yyyy-MM-dd), `shopVisitEventId?`, `firstPostShopDeclared`, `sourceFileName?`, `sourceFileSha256?`, `notes?`, `parameters[]`.
- **Do not send** unit, statistic or value origin; the server derives them.
- Each item of `parameters[]` has:
  - the measurement: `snapshotKind` (TAKEOFF_N1_PEAK_CONTEXT / TAKEOFF_EGT_PEAK_OBSERVED / CRUISE), `selector` (CRUISE only: STRICT / OPERATOR_300S / OPERATOR_60S), `parameterCode` (one of the 10 engine parameters of SNAPSHOT_RULES)
  - the values: `rawValue?` (null is allowed in a draft), `suggestedRawValue?` (the prefill number)
  - the source: `sourceEnginePosition?`, `sourceStartIndex?`, `sourceEndIndexInclusive?`, `sourcePeakIndex?`
  - the reference: `referenceConditions?[{parameterCode, value}]`, `referenceConfiguration?`, `notes?`
- **Prefill to draft:** copy `snapshot.source` into the `source*` fields, and `snapshot.referenceConditions` and `referenceConfiguration` unchanged.

**`BaselinePrefill`:**
- Fields: `engineId`, `engineSerialNumber`, `enginePosition`, `sourceFileName`, `sourceFileSha256`, `fileArchived=false`, `firstPostShopVerified=false`, `sourceQuality=PROVISIONAL`, `notEvaluableReason?`, the catalog and SNAPSHOT_RULES ids and versions, and `snapshots[]`.
- Each item of `snapshots[]` has: `snapshotKind`, `selector`, `statistic`, `availability`, `reason?`, `contextStatus?`, `source{position,startIndex,endIndexInclusive,peakIndex}`, `referenceConditions[]`, `referenceConfiguration`, `values[{parameterCode, unit, suggestedRawValue?, sampleCount}]`.
- **A null suggestion stays an empty editable field. Never show it as 0.**

**`Baseline`:**
- Fields: `id`, `engineId`, `engineSerialNumber`, `name`, `lineageId`, `revisionNumber`, `previousBaselineId?`, `state`, `draftRevision`, `inputProvenance` (MANUAL / FILE_PREFILLED / MIXED), `declaredEffectiveDate?`, `shopVisitEventId?`, `firstPostShopDeclared`, `sourceFileName?`, `sourceFileSha256?`, `notes?`, `publicationIssues[]` (NO_PARAMETER / VALUE_MISSING), the audit fields, and `parameters[]`.
- Each item of `parameters[]` has: `id`, `snapshotKind`, `selector`, `statistic`, `parameterCode`, `unit`, `rawValue?`, `suggestedRawValue?`, `valueOrigin` (FILE_SUGGESTED / FILE_EDITED / MANUAL), `isPrimary`, `source{…}`, `referenceConditions[]`, `referenceConfiguration?`, the pinned document ids and versions, `notes?`.

**`Evaluation`:**
- Identity: `id?`, the baseline id, name, revision and state, `baselineParameterId`, `isPrimary`.
- Flight: `flightId`, `flightNumber`, `flightDate`, `engineId`, `position`, `analysisRevisionId`, `flightStabilityPointsId`, the SNAPSHOT_RULES id and version.
- Measurement: `snapshotKind`, `selector`, `statistic`, `parameterCode`, `unit`.
- Numbers: `rawObserved?`, `baselineRawValue`, `rawDelta?`, `arithmetic` (for example `681 - 661 = +20 CELSIUS`).
- Corrected values: `correctedObserved`, `correctedBaseline` and `correctedDelta` are **always null in P07**.
- Status and warnings: `status`, `reasons[]`, `conditions[{parameterCode, perEngine, observed, reference, difference, tolerance, outsideTolerance}]`, `observedConfiguration`, `referenceConfiguration`.
- Trace: `source{position,startIndex,endIndexInclusive,peakIndex}`, `isCurrent`, `calculatedAt`.

**`status` values:**
- DESCRIPTIVE_RAW
- DESCRIPTIVE_RAW_UNIT_UNVERIFIED (vibration)
- NOT_COMPARABLE: `rawDelta` is null, and the reason is in `reasons[0]`
- NOT_FINAL: the flight is not accepted
- STALE_REVISION

`BaselinePreview` = `{baseline, canPublish, issues[], previewFlightId?, previewFlightNumber?, previewFlightDate?, evaluations[]}`.

`FlightBaselineComparisons` = `{flightId, flightNumber, flightDate, flightState, currentAnalysisRevisionId?, evaluations[]}`.

## Screen rules (pack v2.0 docs/07)
- **Create form:** one form with two ways in, "Upload Excel/CSV for autofill" or "Enter values manually".
  - Uploading needs the engine position.
  - A SHOP_VISIT link is optional. "First flight after shop" shows as *declared, not verified*.
- **Value grid:** group the rows by TAKEOFF_N1_PEAK_CONTEXT, TAKEOFF_EGT_PEAK_OBSERVED and CRUISE (with its three selectors). Columns: Parameter, Statistic, Suggested, Baseline value (editable), Unit, Origin, Context note, Source window link.
- **Publish:** needs a checkbox: "User-defined RAW reference — environmental effects not removed".
- **After publish:** follow the evaluation job with `GET jobs/{id}`. On FAILED, offer Reconcile.
- **Comparison and trend:** show `rawDelta` with the legend "Raw descriptive — environmental factors not removed". Show every warning as a badge.
  - Show `conditions[]` as observed / reference / difference / tolerance.
  - The corrected column reads "Not available: approved OEM model missing".
  - No slope, EWMA, health verdict or threshold colouring.
- **Badges:** RETROSPECTIVE, TEMPORAL_ORDER_UNVERIFIED (same day: order unknown), REFERENCE_SOURCE_POINT, and UNVERIFIED RECORDER SCALE for vibration.
- **Deep link:** `/flights/{flightId}?tab=stability` with the snapshot kind and selector.
- **Error codes:** 80400 not found, 80401 not a draft, 80402 not published, 80403 stale draft, 80404 cannot publish, 80405 invalid parameters, 80406 file rejected, 80407 parameter not found, 80408 primary changed, 80409 not a SHOP_VISIT of the engine, 80410 parameter of another engine.
