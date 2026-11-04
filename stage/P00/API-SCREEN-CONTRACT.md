# P00 — API and screen contract

Base path: `Backoffice/v1/`.
- **Envelope:** Framework `ApiResult` `{data, errors[{code, title, detail, metadata}]}` (owner ruling).
- **Correlation id:** header `X-Correlation-Id`, present on every response, including errors.
- **Ids:** strings.
- **Enums:** `EnumValueDto {value, name, title}`.
- **Lists:** `PaginatedList {results, pageNumber, pageSize, totalCount}`, page size 1..100 (default 25).

## Permissions
Deferred by the owner on 2026-11-05: there is no security code for now. Every route below is open, and the "Permission" column shows the intended policy for when security is resumed.

## Screen S9 — Configuration home (frontend repository)
| Screen event | Route | Method | Request | Response fields | Permission (when resumed) | Errors | Empty state | Owner demo click |
|---|---|---|---|---|---|---|---|---|
| open Configuration home | `config/kinds` | GET | — | `kind` (EnumValueDto), `activeVersion` (null, or the same document summary as the list below), `activeVersionReason` (`NO_ACTIVE_VERSION` when null), `draftCount`, `publishedCount` | Viewer | — | 9 kinds, each "no active version" | Sign in → Configuration |
| open one kind | `config/{kind}/documents?pageNumber&pageSize` | GET | `kind` = UPPER_SNAKE code | page of `{id, kind, versionNumber, status, basedOnDocumentId, contentSha256, lastUpdateTime}` | Viewer | 404 code 80100 (kind not found: only the exact UPPER_SNAKE code is accepted), 400 page out of range | "no documents for this kind" | Configuration → DATA_PROFILE |
| open one document | `config/{kind}/documents/{documentId}` | GET | — | same fields plus `content` (JSON object) | Viewer | 404 code 80101 (document not found, also returned when the document belongs to another kind); 404 code 80100 | — | (no documents in P00) |

## Screen S13 — Audit (frontend repository; first used in J12)
| Screen event | Route | Method | Request | Response fields | Permission (when resumed) | Errors | Empty state |
|---|---|---|---|---|---|---|---|
| open audit log | `audit?entityType&entityId&actor&from&to&pageNumber&pageSize` | GET | `from` ≤ `to` (ISO 8601) | page of `{id, occurredAt, actorSubject, actorId, entityType, entityId, action (EnumValueDto), before, after (JSON or null), correlationId}`, newest first | Viewer | 400 when `from` > `to` or page out of range | "no audit entries" |

## Error code catalogue (P00)
| Code | HTTP | Meaning |
|---|---|---|
| 80100 | 404 | configuration kind not found |
| 80101 | 404 | configuration document not found |
| 80102 | 422 | document version below 1 |
| 80103 | 422 | document content is empty |
| 80104 | 422 | document content is not one JSON object |

80102–80104 are domain rules. No P00 route can trigger them yet; they first apply when drafts are created in P02.

## Other endpoints
- `GET /health`, `/health/live`, `/health/ready`: Framework Kubernetes probes.
- Swagger UI and JSON: only in Development and Staging.
