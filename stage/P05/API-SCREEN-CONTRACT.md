# P05 — API and screen contract

Conventions as in P00–P04: Framework envelope, long ids as strings, enums as `{value, name, title}`. Security is deferred by the owner. 54 business routes (48 − 1 + 7).

## S9/S9a PHASE_RULESET
The generic registry routes accept `PHASE_RULESET`: drafts, validate (JSON path errors), publish, retire, diff. Publishing needs an active PARAMETER_CATALOG. The seed draft is created at startup.

| Event | Route | Response | Errors |
|---|---|---|---|
| dry-run a draft (or any version) on a flight | POST `config/PHASE_RULESET/documents/{id}/dry-run/phases` `{flightId}` | phase dry-run (below) | 404 80101 document; 404 80311 flight; 422 80111 another kind; 422 80108 invalid document |

## S8 Phases tab, R-F3
| Event | Route | Response | Errors |
|---|---|---|---|
| ribbon, segments, anchors, coverage | GET `flights/{id}/phases` | `id`, `flightId`, `analysisRevisionId`, `revisionNumber`, `phaseRuleSetDocumentId`/`Version`, `ruleSet{code, title, description, calibrationStatus, engineeringSignOff}`, `reference{algorithm, title, match, changes[{path, change, before, after}]}`, `status`, `notEvaluableReason`, `sampleCount`, `anchors[]`, `coverage[{phase, samples, percent}]`, `segments[{phase, startIndex, endIndexInclusive, startElapsed, endElapsed, seconds, confidence, evidenceCodes, flags, unknownReasons}]`, `transitions[{sampleIndex, elapsed, fromPhase, toPhase, firedRuleId, ruleLabel, unknownReason, changedBy, when}]`, `evaluatedUtc` | 404 80311; 404 80319 no phases in the current revision |
| why this sample | GET `flights/{id}/phases/explain?sampleIndex=` | `phase`, `unknownReason`, `firedRuleId`, `ruleLabel`, `changedBy` (post-processing step), `segment`, `rules[{ruleId, phase, matched, fired, when}]`, `inputs[{channel, number, text}]`, `derived[{code, operator, value, sourceIndices, nullReason}]`, `masks[{code, startIndex, endIndexInclusive, jumpIndex, cause}]`, `anchors[]` | 404 80311; 404 80319; 422 80320 sample outside the flight |

**Anchor:** `code`, `operator`, `index`, `elapsed`, `basis`, `channel`, `numberBefore`/`numberAt` or `textBefore`/`textAt`, `at`.

**Predicate trace** (`when`, `at`):
- `operator`, `holds`, `channel`, `op`, `threshold`, `observed`, `nullAllowed`, `expected`, `observedText`, `reference`, `of[]`
- `condition{code, operator, contains, predicate, runStart, runEnd, runSeconds, requiredSec, anchor, anchorIndex, windowStart, windowEnd, gate}`

## S8 Events tab, R-F4; R-D6
| Event | Route | Response | Errors |
|---|---|---|---|
| event spans of a flight | GET `flights/{id}/events` | `spans[{code, parameterKey, position, startIndex, endIndexInclusive, startElapsed, endElapsed, seconds, phaseAtStart, phaseAtEnd, values[{parameterKey, atStart, atEnd, peak}]}]`, `coverage[{code, parameterKey, position, evaluability, valuedSamples, activeSamples, spanCount, note}]` | 404 80311; 404 80319 |
| event rates of an aircraft (descriptive) | GET `events/aircraft/{aircraftId}?from&to` | `flights`, `flightsWithPhases`, `landings`, `events[{code, evaluatedFlights, evaluatedLandings, flightsWithEvent, spanCount, perHundredFlights, perHundredLandings, unverifiedFlights, missingFlights}]` | 404 80200; 400 `to` before `from` |

## S8 History, re-run dialog
| Event | Route | Response | Errors |
|---|---|---|---|
| analysis revisions | GET `flights/{id}/revisions` | newest first: `id`, `number`, `trigger` (IMPORT / RECOMPUTE), `status` (PENDING / CURRENT / SUPERSEDED / FAILED), `sourceSha256`, profile, catalog, quality rules and phase rule set ids and versions, `processingJobId`, `failureCode`, `createdUtc`, `completedUtc`, `supersededUtc` | 404 80311 |
| re-run, preview only | POST `flights/{id}/recompute` `{qualityRulesDocumentId?, phaseRuleSetDocumentId?, dryRun: true}` | `{flightId, dryRun: true, preview}` | 404 80311; 422 80323 not a published document of that kind; 422 80116 no active QUALITY_RULES |
| re-run for real | the same with `dryRun` false or absent | `{flightId, dryRun: false, revision (PENDING), processingJobId}`; the job makes the revision CURRENT and supersedes the old one with its results | 422 80321 the current revision already uses these versions; 409 80322 a recompute is pending; as above |

Omitted ids mean the active version. `GET jobs/{id}` follows the job.

**Phase dry-run** (both dry-runs):
- `flightId`
- current: `currentRevisionId`, `currentRevisionNumber`, `currentPhaseRuleSetDocumentId`/`Version`, `currentGrade`
- proposed: `proposedQualityRulesDocumentId`/`Version`, `proposedPhaseRuleSetDocumentId`/`Version`/`Status`, `proposedGrade`
- `changedSamples`, `changedPercent`
- `changes[{startIndex, endIndexInclusive, before, after}]`, `anchorChanges[{code, beforeIndex, afterIndex, beforeBasis, afterBasis}]`
- `currentCoverage`, `proposedCoverage`, `proposedSegments`, `proposedAnchors`, `proposedReference`

## What the user reads (no bare codes)
| Field | Meaning shown to the user |
|---|---|
| `ruleSet.title`, `ruleSet.description` | display text of the rule set, written in the document; never evaluated |
| `ruleSet.calibrationStatus` | Candidate (not calibrated) / Calibrated |
| `ruleSet.engineeringSignOff` | Engineering sign-off pending / Approved by the flight engineer |
| `reference.algorithm`, `reference.title` | the pack reference the rules are compared with: phase-v1.0.3, the phase algorithm of the pack v3.3 reference |
| `reference.match` | computed by the system, never typed by hand: Matches the pack reference / Modified from the pack reference |
| `reference.changes` | every changed rule against the reference, with its path and old and new value. Title, description, notes, decisions and status do not count. |

## Changed from P04
- `POST flights/{id}/quality` is removed; `POST flights/{id}/recompute` replaces it (docs/13).
- Every FlightQuality belongs to an analysis revision. Import records revision 1 with quality and, if a PHASE_RULESET is active, phases.
