# P07 — Test plan and coverage of B07-01..22

| ID | Test | Result |
|---|---|---|
| B07-01 | Walkthrough: prefill with the 9072 CSV gives the golden E1 OPERATOR_60S EGT (660.15). The flight count stays 0 (nothing stored). | PASS |
| B07-02 | Walkthrough: an XLSX built from the same CSV gives identical snapshots. A changed header is refused through the same P03 inspector (80406 with the P03 code). | PASS (header case covered by the P03 tests) |
| B07-03 | Walkthrough: position 2 gives the golden E2 value | PASS |
| B07-04 | Walkthrough: takeoff N1, EGT peak and cruise suggestions are grouped, with the source window. Conformance: a missing observation gives NOT_COMPARABLE and a null delta. | PASS |
| B07-05 | Walkthrough: EGT edited 660.15 → 661 is FILE_EDITED and keeps the suggestion. Conformance origins test. | PASS |
| B07-06 | Walkthrough: a manual one-row baseline publishes without a file, evidence, envelope or shop event | PASS |
| B07-07 | Draft with an empty row → 422 80404. NaN/∞ → 80405. Duplicate → 80405. | PASS |
| B07-08 | Independent fixture: +6, +7, +11, +0.03 | PASS |
| B07-09 | Two published baselines coexist. Primary set explicitly. Stale expected primary → 409 80408. Unique index. | PASS |
| B07-10 | The job writes one record per flight × parameter. Delta = golden flight-26 EGT − reference. | PASS |
| B07-11 | Conformance: CONTEXT_UNVERIFIED keeps the value and adds the warning. P06 masks are unchanged. | PASS |
| B07-12 | Conformance: the EGT peak is raw only (CONTEXT_UNKNOWN). Corrected is null. | PASS |
| B07-13 | Fixture P07-MATH-04: vibration gives DESCRIPTIVE_RAW_UNIT_UNVERIFIED | PASS |
| B07-14 | Fixture NULL-02/03/04 and the conformance mismatch cases | PASS |
| B07-15 | Walkthrough: the imported 9072 flight is REFERENCE_SOURCE_POINT, with delta 660.15 − 661 (not zero) | PASS |
| B07-16 | Walkthrough: same-day TEMPORAL_ORDER_UNVERIFIED. Conformance: RETROSPECTIVE. | PASS |
| B07-17 | Walkthrough: publish queues the job. Reconcile and a re-run give no duplicate current rows. | PASS |
| B07-18 | Walkthrough: after HOLD the status is NOT_FINAL before the job, and the rows are withdrawn after it. RELEASE + ACCEPT gives new rows. | PASS |
| B07-19 | Walkthrough and conformance: a clone edit never changes the published source | PASS |
| B07-20 | Frontend with the real backend | NOT_RUN: frontend agent |
| B07-21 | The register is R-P08, read from `GET engines/{id}/baselines`. No report engine. | PASS (by design) |
| B07-22 | P06 regressions | See ACCEPTANCE-RECORD |

**Commands:**
- `dotnet test tests/AeroTech.ECTM.Domain.ConformanceTests -c Release`
- `dotnet test tests/AeroTech.ECTM.Application.AcceptanceTests -c Release` (throwaway databases)
