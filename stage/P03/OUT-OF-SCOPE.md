# P03 — Out of scope

| Item | Where it goes |
|---|---|
| quality spans, grade, WARNING/BLOCKING classification of coverage and plausibility | P04 QUALITY_RULES (thresholds live in policy-defaults → dataQuality) |
| provisional pipeline on commit (quality → phases → snapshots → …) | grows from P04 |
| ACCEPT / EXCLUDE, bulk review, analysis revisions | P06 |
| "supersede the earlier flight" in duplicate resolution, `flights/{id}/replace-source`, `flights/{id}/revisions` | P06, which brings analysis revisions; P03 offers "confirm distinct flight" |
| phase ribbon, masks, anchors and snapshot windows on the raw chart | P04–P06 |
| R-P01 / R-P02 report screens | P11 (the data routes exist now) |
| `jobs?subjectType=&subjectId=` list | not needed; the batch carries its job id |
| clean-up of uncommitted preview batches | not scheduled; batches stay PREVIEWED and auditable |
| preview as a background job (needed above about 200 files per batch) | owner decision; P03 keeps the synchronous preview of the pack contract |
| publish gate "a dry-run on at least one flight" (docs/03) | owner decision; the dry-run exists, publishing does not require it yet |
| derived parameters in `flights/{id}/series` after publication | not in PHASE-03; the dry-run already returns their points for the raw viewer (J11) |
| security | deferred by the owner |
| frontend S2, S3, S3a, S7, S8 Raw | frontend repository |
