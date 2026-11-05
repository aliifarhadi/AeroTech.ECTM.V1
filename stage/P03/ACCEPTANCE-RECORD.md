# STAGE ACCEPTANCE — P03 / Import, identity at upload, raw workbench (backend)

## Source lock
- **Backend:** `AeroTech.ECTM.V1`, uncommitted on top of `7b64401` (P02).
- **Legacy references read:** GAP-ANALYSIS §2 and `docs/legacy-notes/import-and-flights.md`.
- **Migration:** `AddImportsAndFlights` adds 10 tables and alters nothing:
  - SourceFiles, SourceFileContents
  - ImportBatches, ImportItems
  - Flights, FlightEngineAssignments, FlightValidationIssues, FlightChannels, FlightChannelPayloads
  - ProcessingJobs

  It is applied to `DotAirEctmV1` (4 migrations) and verified on every throwaway test database. No model change is pending.
- **Report, screen and route IDs:** R-P01 and R-P02 data, S2, S3, S3a, S7, S8 Raw and S9a (catalog dry-run on a flight), and 12 new operations (45 in total).

## Basis of limits and rules (owner ruling)
All 102 real files in `samples/` were measured before any limit or rule was set (GAP-ANALYSIS §5; script `evidence/measure_samples.py`; per-file results in `reports/P03/`, private).
- **Limits:**
  - 50,000,000 bytes per file
  - 200 files per batch
  - 1 GiB per batch and per request
- **Rules confirmed:**
  - the elapsed gap is a step larger than the file's most common step; no threshold is needed
  - UTC is kept as raw text only
  - enumerated domains come from the observed literals
  - `ENGINE_ASYMMETRY` is numeric, by owner ruling
- **Real-volume run:** the largest real delivery (97 files) previews in 25–30 s. The commit job takes about 45 s. The result is 97/97 PENDING_REVIEW with both ESNs.

## Independent scientific contract
See SCIENTIFIC-CONTRACT.md. P03 makes no engineering claim.

| Gate | Result |
|---|---|
| Regression: 8 goldens (rows, first/last elapsed, flight number, day, month, SHA-256) | PASS (`RecordingReaderTests`) |
| Independent values: samples 101 and 121 of flight 25, read with stdlib `csv` | PASS in the domain (`RecordingReaderTests`), end to end (RAW series and the raw-rows drawer, `" GND"` and `" AIR"` exact) and in the catalog dry-run (`848 / 97.85`, `873 − 848`) |
| Corpus: 102 real files | PASS: all parse, no BLOCKING, identity agrees, exactly one gap WARNING (`2026-10-09_EP-DOA_47`, after sample 3942, 3 s) (`SampleCorpusTests`) |
| Approval | not applicable |

## Reproducible E2E (what the owner clicks)
- **J3 — `Flows/ImportWalkthroughTests`** (fresh database):
  1. EP-DOA with Eng1S1 on position 1 and Eng2S2 on position 2, installed 2026-09-24; catalog and profile seeds published.
  2. `POST imports/preview` with the 8 real CSVs in one ZIP → 8 NEW, each RESOLVED with both ESNs, golden sample counts, 10 first and 10 last rows.
  3. `POST imports/commit` (key `j3-commit`) → 202 COMMITTING with a job. The same key returns the same job; another key → 409 80307.
  4. `GET jobs/{id}` → COMPLETED 8/8. `GET imports/{batchId}` → **8/8 PENDING_REVIEW, identity RESOLVED, Eng1S1 and Eng2S2 on every row** (exit criterion).
  5. The same bytes again → DUPLICATE linked to the existing flight; after commit SKIPPED; still 8 flights.
  6. `GET flights/{24}/series?parameters=N1_PCT[1],N1_PCT[2],EGT_C[1],EGT_C[2]&budget=400` → M4 within budget for both engines.
  7. RAW from 101 to 121 of flight 25 → N1 92.52 / 92.06, EGT[2] 873 / 872. `samples` gives the exact source text.
  8. `review`: HOLD → HELD, RELEASE → PENDING_REVIEW, ACCEPT → 422 80315, RELEASE again → 409 80314.
  9. A new catalog draft from the active one adds `EGT_OVER_N2` and narrows the N1 range. `POST config/PARAMETER_CATALOG/documents/{draft}/dry-run/flight` on flight 25 gives:
     - EGT_OVER_N2[1] ADDED, available at all 4313 samples, 848 / 97.85 at sample 101
     - EGT_DIFF_E2_E1_C UNCHANGED against the active catalog
     - the N1 plausibility finding ADDED; nothing stored
     - a profile on this route → 422 80111; an unknown flight → 404 80311
- **J4 — `Flows/IdentityWalkthroughTests`** (fresh database):
  1. `2026-10-01_EP-XYZ_24.csv` → UNRESOLVED (AIRCRAFT_NOT_FOUND) → after commit NEEDS_IDENTITY.
  2. The dialog with swapped engines → 422 80312; with one position missing → 422 80309; correct → PENDING_REVIEW, sources USER, both ESNs.
  3. Different bytes with the same identity → POSSIBLE_DUPLICATE_IDENTITY → without confirmation 422 80313 → with `confirmDistinctFlight` PENDING_REVIEW.
- **Negative paths, all tested:**
  - synthetic source → REJECTED SYNTHETIC_SOURCE_NOT_ALLOWED
  - swapped columns → REJECTED PROFILE_SIGNATURE_MISMATCH with 2 mismatches, no flight
  - unsupported archive member → REJECTED alone
  - PDF upload → 422 80301; broken archive → 422 80303; no file → 400
  - unknown batch, job, flight → 404 80300, 80310, 80311
  - repeated or decreasing elapsed → BLOCKING; ragged row and literal outside the domain → WARNING
  - unknown series parameter → 422 80316
  - job runner: a failed unit is abandoned and the job still completes with progress; a failure outside the units fails the job and the batch
- **Development database `DotAirEctmV1` (owner instruction 2026-11-05):**
  - Both seed drafts were replaced with the P03 seeds through the API: `enumValues`, and `ENGINE_ASYMMETRY` NUMERIC in the catalog and the profile. The profile had to follow because that column's unit changed.
  - Both drafts validated with 0 issues and were published: the catalog first, then the profile.
  - Approval note: "Owner instruction 2026-11-05 (reply to the P03 report): replace the seed draft with the P03 seed and publish."
  - Readiness now reports `parserReady=true` (`evidence/readiness-after-publish.json`, `evidence/config-kinds-after-publish.json`).
  - The audit holds a content UPDATED and a publish UPDATED entry per document.
  - The actor is empty because security is deferred.
  - The work used a separate host on port 5199, which was stopped afterwards. No flight was imported.
  - The owner's running ServiceHost (started before P03) must be restarted to serve the new routes.
- **401/403:** NOT DELIVERED (security deferred by the owner).
- **Tests:** `evidence/conformance-tests.txt` 199/199 and `evidence/acceptance-tests.txt` 80/80. The throwaway databases were dropped.

## Delta inventory
| Kind | Added in P03 |
|---|---|
| Domain concepts | SourceFile (+content), ImportBatch (+ImportItem), Flight (+engine assignments, validation issues, channels, channel payloads), ProcessingJob. Computation: recording reader and checks, time axis, file-name identity, identity claims and resolver, channel codec, M4 and series sampler, derived series evaluator, finding comparison. Readers: CSV, XLSX, source rows |
| Contracts | 13 enums |
| Seeds | catalog: `enumValues` for 25 ENUM parameters (measured on `samples/`); `ENGINE_ASYMMETRY` NUMERIC with unit `NONE` in the catalog and the profile |
| Tables | 10 (see source lock) |
| Endpoints | 12: imports preview, commit, list and detail; flights list, detail, identity, review, series and samples; jobs/{id}; catalog dry-run on a flight |
| Background work | `ProcessingJobPoller` in Consumers; job kind IMPORT_COMMIT |
| Tests | 55 new conformance tests, 13 new acceptance tests |

## Deviations
- **Carried over:** Consumers kept, Framework envelope, Ordering query pattern, no security.
- **New in P03:**
  - Files per batch 200 instead of the planned 500, because of the measured synchronous preview time.
  - The P02 header comparison now trims both sides (docs/02 §4.1).
  - The P02 file dry-run of a catalog now points to the flight dry-run.
  - The provenance ordinal is called `profileOrdinal`.
  - Error code 80317 added (RAW range too large).
  - `ENGINE_ASYMMETRY` departs from the pack type (ENUM → NUMERIC) by owner ruling.

## Stage closure
- **`SOFTWARE_VERIFIED`:** PASS for the delivered scope (279/279 tests). Security remains deferred.
- **`ENGINEERING_APPROVED`:** not applicable (no engineering result). The enumerated domains, limits and the `ENGINE_ASYMMETRY` unit await review.
- **`OPERATIONAL_STATUS`:** import AVAILABLE in `DotAirEctmV1` (parser ready) after restarting the host on the P03 build. Quality grading starts in P04.
- **Open owner decisions and inputs:**
  1. **Preview as a background job:** needed if one delivery can exceed about 200 files (a full month at 8 flights a day is 248).
  2. **Publish gate:** docs/03 asks for "a dry-run on at least one flight (where applicable)" before publishing. The dry-run exists; should publishing require one?
  3. **I-02:** the recorder frame manual: the unit and meaning of `ENGINE_ASYMMETRY`, and the domains of the three always-empty columns.
  4. **Still open:** engine model of Eng1S1 and Eng2S2 (I-01); THETA and DELTA; fixture publication; package vulnerability.
- **Owner signature and gate to start P04:**
