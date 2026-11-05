# P03 — Gap analysis

## 1. Source lock
Legacy `AeroTech.ECTM@0ad5f93`, unchanged. V1 baseline: P00–P02 committed by the owner (`7b64401`). The P02 gate is signed ("A").

## 2. Legacy code read, and the decision for each item
| Legacy path | Decision | Reason |
|---|---|---|
| `Application/ImportBatchAggregate/Services/UploadExpander.cs`, `RarArchiveReader.cs`, `SourceFileTypes.cs`, `SourceMember.cs` | PORT and extend | Adds ZIP (J3 "RAR/ZIP") through SharpCompress `ArchiveFactory`. Limits come from the measurement of `samples/` in §5. Encrypted, empty and over-limit archives are refused. |
| `Domain/ImportBatchAggregate/ImportLimits.cs` | RE-IMPLEMENT as `ImportOptions` (configuration section `Import`) | A size bound is a security limit, not a domain constant (docs/20). The defaults are the §5 values; operations may change them. |
| `Application/ImportItemAggregate/Services/XlsxRows.cs`, `SourceRows.cs` | PORT | Moved to `Domain/_Shared/Recording`; first worksheet, shared and inline strings, row cap. |
| `Domain/AnalysisRunAggregate/Computation/M4Reducer.cs` | PORT | Applied per run of valued samples, so that a null or an elapsed gap is never bridged. |
| `Application/ProcessingJobAggregate/Services/ProcessingJobRunner.cs`, `IProcessingJobHandler.cs`, `Domain/ProcessingJobAggregate/ProcessingJob.cs`, `Consumers/Jobs/ProcessingJobPoller.cs` | PORT | DB lease, progress, per-unit retry on a concurrent change, and unit abandonment. The poller lives in the Consumers project the owner kept. |
| `ParsedPayloadAggregate/Telemetry/TelemetryParser.cs`, `TelemetryBulkWriter.cs`, `SampleExtraCodec` | RE-IMPLEMENT | Channels are read through the DATA_PROFILE, with no fixed properties. Storage is one compressed column per mapped channel (`FlightChannels`) instead of a wide row table, so bulk copy is not needed. |
| legacy `DatasetRevision` / `ApprovedDataset` / `FlightRecord` identity chain | IGNORE | Replaced by the Flight state machine (docs/16 §3). |

## 3. Decisions taken for small gaps
- **Preview:**
  - Synchronous. It stores each source file once by SHA-256 (bytes immutable) and an ImportBatch with one item per file.
  - It pins the active DATA_PROFILE and PARAMETER_CATALOG versions, so that commit uses the same versions.
  - No active profile or catalog → 422 80116.
- **Commit:** asynchronous, as a processing job of kind IMPORT_COMMIT (S2 "Jobs/{id} polling"). It re-reads each file with the pinned versions and recomputes identity at commit time, so a fleet change between preview and commit is respected. Idempotency rules:
  - same key → same batch
  - different key on a committed batch → 409
- **Item outcomes:**
  - NEW
  - DUPLICATE: same bytes, linked to the existing flight; nothing is parsed again
  - REJECTED, with one of these reasons:
    - signature mismatch
    - unsupported file
    - no data rows
    - synthetic source outside a test environment
- **Identity at upload:**
  - The file name `YYYY-MM-DD_REG_FLIGHTNO.csv|xlsx` gives the date, registration and flight number. Defaults fill what the name does not give; the file name wins.
  - The `FLIGHT_NUMBER`, `DAY` and `MONTH` columns must agree with the name.
  - The aircraft is looked up by registration.
  - Engines come from the installations on that position covering the whole flight date. With none, the engine is POSITION_ONLY. When the installation changes during that day, the engine needs confirmation.
  - Same identity with different bytes → the flight is flagged POSSIBLE_DUPLICATE_IDENTITY and needs the operator to confirm it is a distinct flight.
- **Identity status:**
  - UNRESOLVED: no registration, no aircraft or no date.
  - NEEDS_CONFIRMATION: any mismatch, POSITION_ONLY, ambiguity or possible duplicate.
  - RESOLVED: otherwise.
- **Flight state:** PENDING_REVIEW when identity is RESOLVED, otherwise NEEDS_IDENTITY. HELD is reachable from PENDING_REVIEW, and RELEASE returns to the earlier state. ACCEPT and EXCLUDE are P06.
- **Import validation in P03 (docs/02 §4):** only rules without a threshold:
  - signature: BLOCKING, so the file is rejected
  - duplicate or decreasing elapsed: BLOCKING
  - elapsed gaps: WARNING, with count and seconds
  - ragged rows: WARNING
  - Required-channel parse coverage and plausibility counts are reported as INFO measurements. Their thresholds (95 %, 50 %, 5 %) are in `policy-defaults → dataQuality`, which is the QUALITY_RULES seed (P04). Grading them is P04's job.
- **Storage:**
  - `SourceFileContents`: raw bytes, separate from `SourceFiles`.
  - `FlightChannels`: one row per mapped column. Numeric values are stored as float64 with NaN meaning no value; text and enum values as UTF-8 JSON. Both are GZip-compressed and have a SHA-256.
  - Raw rows (the samples drawer) are read from the source bytes, so the original text is always shown.
  - Both stores are excluded from the audit log: immutable bulk data whose creation the Flight and SourceFile CREATED entries already show. `byte[]` values are never written into audit snapshots.
- **Sample index:** the 0-based data row of the source file (docs/06 §6 "immutable 0-based sample indices").
- **Synthetic sources:** a file name starting with `SYNTHETIC_TEST_ONLY` is rejected unless `Import:SyntheticSourcesAllowed` is true. It is false by default; only a Test environment may set it (docs/17 §5).
- **R-P01 / R-P02:** the data is delivered through `imports` and `flights`; the report screens come in P11.
- **Error codes:** 80300–80399 are imports and flights.

## 4. Delta inventory planned
| Kind | Items |
|---|---|
| Domain concepts | SourceFile (+content), ImportBatch (+ImportItem), Flight (+EngineAssignment, FlightValidationIssue, FlightChannel), ProcessingJob |
| Tables | SourceFiles, SourceFileContents, ImportBatches, ImportItems, Flights, FlightEngineAssignments, FlightValidationIssues, FlightChannels, ProcessingJobs |
| Endpoints | 11: imports preview/commit/list/detail; flights list/detail/identity/review/series/samples; jobs/{id} |
| UI pages | none here (S2, S3, S3a, S7, S8 Raw in the frontend repository) |
| Tests | parser parity on 8 files, spot values at samples 101 and 121 of flight 25, the whole `samples/` corpus (§5), duplicate, synthetic, identity paths, column swap, archive, job, series and samples |

## 5. Measurement of the real data (`samples/`)
Owner ruling (2026-11-05): limits and rules are based on all the real data in `samples/`. The 8 files named in the pack are a regression subset only. Measured on 2026-11-05 with a stdlib-only script (`evidence/measure_samples.py`). The per-file results stay in `reports/P03/` (private).

| Fact | Measured value |
|---|---|
| files | 102 CSV in 2 delivery folders (5 and 97 files); 4 PDF manuals (not imported) |
| size per file | 1,448,777 – 4,365,021 bytes, median 2,023,781 |
| rows per file | 2,855 – 8,573, median 4,001.5, total 409,837 |
| widest data row | 524 bytes |
| largest delivery | 97 files, 196,691,614 bytes raw, 21,705,945 bytes as ZIP |
| flights per day | at most 8 |
| registrations | EP-DOA only |
| header | all 102 equal the seed profile's 85 exact headers |
| encoding | strict UTF-8 without BOM, CRLF, `,` delimiter |
| ragged rows | none |
| file name | all 102 match `YYYY-MM-DD_REG_FLIGHTNO.csv`; flight numbers have 2–5 digits |
| identity columns | in every file `FLIGHT_NUMBER`, `DAY` and `MONTH` are constant and agree with the name |
| duplicates | none by SHA-256, none by (date, registration, flight number) |
| elapsed | strictly increasing everywhere; 409,734 steps of 1 s and one step of 4 s (`2026-10-09_EP-DOA_47`, after sample 3942: 126669 → 126673) |
| UTC clocks | cross midnight within a flight (flight 47: 22:54:18 → 0:59:56); stall (one value repeated 3,666 times); reset (11:14:59 → 0:00:01 at the flight-47 gap) |
| always empty | `AT_N1_LIMIT_MODE`, `FCC_N1_LIMIT_MODE`, `DAA_N1_LIMIT_MODE` in all 102 files |
| unparseable numeric cells | none |
| out-of-range values | for example negative N1, N2 and fuel flow at engine stop; `LPT_IMBALANCE_ANGLE_DEG[2]` up to 501.9° |
| type note | `ENGINE_ASYMMETRY` is ENUM in the pack but holds numbers (0 to 55.2). Owner ruling 2026-11-05: NUMERIC. Unit `NONE` until the recorder manual (I-02) gives one |

**Limits derived from it** (all in `ImportOptions`, the `Import` configuration section):

| Limit | Default | Reason |
|---|---|---|
| bytes per source file | 50,000,000 | 24 h at 1 Hz at the widest measured row is 45,273,600 bytes. The measured maximum is 4,365,021. |
| files per batch (after expansion) | 200 | preview is synchronous and measured at about 0.3 s per real file (see "Real-volume run" below), so 200 files take about 60 s, a common proxy read timeout. 200 is 2 × the largest real delivery (97). A full month at the busiest measured rate (248) would need preview as a job; that is an open owner question. |
| bytes per batch (expanded) | 1,073,741,824 | 200 files at the largest measured size (4,365,021) are 873,004,200 bytes; 5.4 × the largest real delivery (196,691,614). |
| request body | 1,073,741,824 | a whole delivery may be sent as raw CSV files (196,691,614 bytes) as well as an archive. |

**Real-volume run** (2026-11-05, development machine, local SQL Server Express, throwaway database): the largest real delivery, `EP-DOA_Parameters (20261025)`, 97 files.

| Step | Result |
|---|---|
| upload as one ZIP (21,712,357 bytes) → preview | 201 in 24.7 – 30.4 s; 97 NEW |
| commit job | 38.5 – 48.7 s in most runs (one outlier at 117.5 s on the same machine); 97 COMMITTED, 97/97 PENDING_REVIEW with both ESNs |
| the same 97 files again as raw CSV (196,691,614 bytes) | 201 in 4.4 – 7.3 s; 97 DUPLICATE, nothing parsed |
| commit unit cost | parse 21.5 s, channel encoding 9.6 s, database saves 8.4 s, others 4.1 s (97 files) |

The first version of the preview took 77.9 s and timed out on one large save. It was corrected before the gate:
- source bytes are stored GZip-compressed and saved file by file;
- preview parses distinct files in parallel and keeps only the shown rows;
- preview cells are built only for the 20 rows shown;
- immutable bulk bytes are compared by reference in change tracking.

## 6. Decisions taken during implementation
- **Header check:** a header matches "exactly after trimming" (docs/02 §4.1). The P02 profile dry-run now uses the same comparison, so dry-run and import cannot disagree.
- **Enumerated literals (docs/02 §4.5):** the catalog gets an optional `enumValues` per ENUM parameter. The seed lists the literals observed in all 102 real files (25 parameters). The three always-empty columns get none. A literal outside the domain is a WARNING with the literal and its count.
- **`ENGINE_ASYMMETRY` (owner ruling 2026-11-05):** NUMERIC in the catalog and the profile. The unit is `NONE` and the note says that unit and meaning are unknown until I-02. No unit is guessed.
- **Provenance ordinal:** each stored channel keeps the DATA_PROFILE column it came from as `profileOrdinal`. docs/16 allows ordinals in the parser and in provenance. The P00 scan that forbids the identifier `SourceOrdinal` in `src` is unchanged.
- **Storage:** `SourceFileContents` holds the GZip of the original bytes; the SHA-256 is of the original bytes. Channel metadata (`FlightChannels`) and channel values (`FlightChannelPayloads`) are separate, so the flight page never loads values. All three are bulk data and are left out of the audit log.
- **Rejection order:** type, size and synthetic marker are checked before the duplicate check, so the outcome of such a file does not depend on what was imported earlier.
- **Duplicate inside one batch:** the second copy of the same bytes is DUPLICATE and is linked at commit to the flight the first copy creates.
- **Series:** sample-index ranges; RAW returns every sample up to 20,000 per request (the longest real flight has 8,573); M4 gives one null point per break, never a bridge.
- **Catalog dry-run on a flight (owner ruling 2026-11-05: now):**
  - It replays the import of the flight's source file with the pinned profile and the candidate catalog, the same code path as an import (docs/03 "dry-run equals pipeline for the same versions").
  - It evaluates every typed derived parameter at every sample in catalog order, so a derived input such as TAT_K feeds THETA.
  - It compares the result with the active catalog and with the flight's stored findings. Nothing is stored.
  - The file dry-run of a catalog now points to this route.
  - Publishing does not yet require a recorded dry-run (docs/03 "where applicable"); that gate is left for the owner.

**Rules confirmed by it:**
- Elapsed gaps are detected as a step larger than the file's most common step. That step is 1 s in every real file, so no gap threshold is needed. The one real gap gives the WARNING test.
- Duplicate or decreasing elapsed never occurs in real data. It stays BLOCKING because the time axis must be strictly increasing; a test edits a copy of a real file.
- UTC is stored as raw text and never used as time: real UTC values are not monotonic.
- The three always-empty columns give three INFO coverage measurements of 0 % on every flight. Grading them is P04's job.
- The QUALITY_RULES candidate thresholds for P04 are checked against this measurement before the owner publishes them.
