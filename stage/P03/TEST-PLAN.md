# P03 — Test plan

| Pack requirement | Test | Project |
|---|---|---|
| parser parity on 8 files | each real file parses to the golden row count, first and last elapsed, flight number, day and month; the SHA equals the golden | Conformance (parser) + Acceptance (commit) |
| real-data corpus (owner ruling) | every CSV in `samples/` parses to its measured row count with no BLOCKING issue; the only gap WARNING is flight 47 of 2026-10-09 after sample 3942; name and columns agree | Conformance |
| samples 101 and 121 of flight 25 | the stored channel values and the raw drawer equal the independent stdlib-csv values (SCIENTIFIC-CONTRACT) | Conformance + Acceptance |
| duplicate | the same bytes again → DUPLICATE, linked to the existing flight; nothing is parsed again; a second commit adds no flight | Acceptance |
| synthetic rejection | a `SYNTHETIC_TEST_ONLY_…` file → REJECTED `SYNTHETIC_SOURCE_NOT_ALLOWED` | Acceptance |
| identity paths | RESOLVED (both ESNs from installations); unknown registration → UNRESOLVED / NEEDS_IDENTITY → identity dialog resolves; no installation → POSITION_ONLY; column/file-name mismatch → NEEDS_CONFIRMATION; possible duplicate identity → confirm distinct; an engine not installed → 422 | Conformance (resolver) + Acceptance |
| column swap | swapped header → REJECTED `PROFILE_SIGNATURE_MISMATCH`, no flight | Acceptance |
| archive | the 8 CSVs in one ZIP → 8 items | Acceptance |
| E2E J3 / J4 | 8 files in one archive → commit job → 8/8 PENDING_REVIEW with both ESNs; renamed copy with unknown registration → NEEDS_IDENTITY → J4 → PENDING_REVIEW | Acceptance |
| idempotent commit | same key → same batch and job; other key → 409 | Acceptance |
| state machine | HOLD → HELD → RELEASE → PENDING_REVIEW; ACCEPT → 422 (P06) | Conformance + Acceptance |
| series | M4 within budget; a gap or missing value gives a null point, never bridged; RAW returns every sample in range | Conformance |
| validation without thresholds | duplicate elapsed → BLOCKING; elapsed gap → WARNING with count and seconds; coverage and plausibility → INFO measurements | Conformance |
| channel codec | float64 and text round-trip exactly, NaN means null | Conformance |
| catalog dry-run on a flight | derived values at every sample (difference, chained TAT_K → THETA, per-position ratio, missing input); finding comparison; end to end: EGT_OVER_N2[1] ADDED with 848 / 97.85 at sample 101 of flight 25, EGT_DIFF_E2_E1_C UNCHANGED, a narrowed N1 range gives an ADDED plausibility finding; other kind → 422 80111; unknown flight → 404 | Conformance + Acceptance |
| job runner | claims, runs units, reports progress, completes; a failed unit is abandoned without failing the batch | Acceptance |
