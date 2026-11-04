# P02 — API and screen contract

Conventions as in P00 and P01. Security is deferred by the owner; the intended policies are Viewer for reads, Engineer for drafts and Approver for publish/retire.

## S9 Configuration home and S9a Document editor
| Screen event | Route | Request | Response | Errors |
|---|---|---|---|---|
| home cards | GET `config/kinds` | — | per kind: `kind`, `deliveryPhase`, `authorable`, `activeVersion` (with publishedUtc, publishedBy, approvalNote) or null + `NO_ACTIVE_VERSION`, `draftCount`, `publishedCount` | — |
| readiness banner (R-P05 data) | GET `meta/readiness` | — | `parserReady`, `missingForParser[]`, per kind `{kind, deliveryPhase, requiredForParser, activeDocumentId, activeVersionNumber}` | — |
| new version from active | POST `config/{kind}/documents` | `basedOnDocumentId` (clones its content) or `content` (JSON object) | DocumentDetail, 201 | 422 80105 kind not authorable (names its phase); 422 80110 base of another kind; 422 80103/80104 content |
| edit draft | PUT `config/{kind}/documents/{id}` | `content` | DocumentDetail | 409 80106 not a draft; 404 80100/80101 |
| validate | POST `config/{kind}/documents/{id}/validate` | — | `valid`, `issues[{path, code, message}]` | 404 |
| publish | POST `config/{kind}/documents/{id}/publish` | `approvalNote` (required, ≤2000) | DocumentDetail (PUBLISHED, publishedUtc/By, note) | 400 without note; 422 80108 has issues; 409 80106 |
| retire | POST `config/{kind}/documents/{id}/retire` | `note?` | DocumentDetail (RETIRED) | 409 80107 not published |
| compare (R-P06 data) | GET `config/{kind}/documents/{fromId}/diff/{toId}` | — | `from`, `to` summaries, `changes[{path, change ADDED/REMOVED/CHANGED, before, after}]` | 404 |
| profile preview | POST `config/DATA_PROFILE/documents/{id}/dry-run` | multipart `file` (CSV ≤ 50 MB) | `signatureMatches`, `expectedColumnCount`, `headerColumnCount`, `headerMismatches[{ordinal, expectedHeader, actualHeader}]`, `dataRowCount`, `raggedRowCount`, `firstRows`/`lastRows` (10 each; cells `{ordinal, parameterKey, raw, number, status OK/MISSING/UNPARSEABLE/TEXT}`), `parseSummary[{ordinal, parameterKey, missingCount, unparseableCount}]`, `engines[{position, parameterKeys}]`, `fileSha256` | 422 80111 for other kinds; 422 80112 unreadable; 422 80113 too large; 422 80116 no active catalog; 422 80108 invalid profile |
| typed-definition kinds | GET `config/typed-definitions/kinds` | — | `{code, fields, availableFrom, evaluable}` × 10 | — |
| test-evaluate box | POST `config/typed-definitions/evaluate` | `catalogDocumentId?` (default: the active catalog), `definition` (typed JSON), `position?` (1, 2), `inputs{ref: {value, unit}}` | `code`, `kind`, `value` or null, `unit`, `status` (AVAILABLE / POSITION_REQUIRED / MISSING_INPUT / UNIT_MISMATCH / NULL_INPUT / DENOMINATOR_ZERO / NON_FINITE), `reasons[]`, `inputs[]`, `constants{}`, `formula`, `catalogDocumentId`, `catalogVersionNumber` | 422 80115 invalid definition, with the first issue (path, code, message); 422 80116 no active catalog |

## Validation issue codes
- **Catalog:**
  - `CATALOG_CODE_REQUIRED`, `PARAMETERS_REQUIRED`, `CODE_INVALID`, `CODE_DUPLICATE`
  - `ROLE_UNKNOWN`, `UNIT_UNKNOWN`, `VALUE_KIND_UNKNOWN`
  - `POSITIONS_REQUIRED`, `POSITION_INVALID`, `PLAUSIBILITY_INVALID`, `OEM_CITATION_REQUIRED`
  - `KIND_UNKNOWN`, `KIND_NOT_AVAILABLE`, `FIELD_NOT_ALLOWED`, `FIELD_REQUIRED`, `STATUS_UNKNOWN`
  - `REF_INVALID`, `REF_UNKNOWN`, `REF_POSITION_INVALID`, `REF_NOT_NUMERIC`
  - `UNITS_DO_NOT_RECONCILE`, `CONSTANT_INVALID`, `EXPONENT_REQUIRES_CORRECTION_MODEL`
  - `ACTIVE_PROFILE_INCOMPATIBLE`
- **Profile:**
  - `PROFILE_CODE_REQUIRED`, `FILE_TYPE_UNSUPPORTED`, `DELIMITER_INVALID`, `ENCODING_UNSUPPORTED`
  - `COLUMN_COUNT_MISMATCH`, `ORDINAL_OUT_OF_RANGE`, `ORDINAL_DUPLICATE`, `ORDINAL_MISSING`
  - `HEADER_REQUIRED`, `OCCURRENCE_MISMATCH`
  - `ACTIVE_CATALOG_MISSING`, `PARAMETER_UNKNOWN`, `PARAMETER_IS_DERIVED`, `POSITION_INVALID`, `COLUMN_COLLISION`
  - `UNIT_UNKNOWN`, `UNIT_MISMATCH`, `CONVERSION_UNSUPPORTED`, `MISSING_TOKENS_REQUIRED`
  - `TIME_CHANNEL_MISSING`, `REQUIRED_CHANNEL_MISSING`
- **Both:** `CONTENT_SHAPE_INVALID`, meaning a field outside the schema or a wrong JSON type.
