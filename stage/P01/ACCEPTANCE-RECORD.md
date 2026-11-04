# STAGE ACCEPTANCE — P01 / Fleet registry (backend)

## Source lock
- **Backend:** `AeroTech.ECTM.V1`, uncommitted on top of `cf126e3`. It contains P00 and P01.
- **Frontend:** `AeroTech.ECTM.Backoffice.V1`, not part of this delivery.
- **Legacy references read:** see GAP-ANALYSIS §3 and `docs/legacy-notes/fleet-registry.md`.
- **Migration:**
  - `AddFleetRegistry` adds 7 tables, the `RegistryVersions` sequence, 3 check constraints and filtered unique indexes. It only adds; nothing is altered or dropped.
  - It is applied and verified on every throwaway test database: no pending migration, no pending model change.
  - **It is not yet applied to Development `DotAirEctmV1`.** The owner's host (`dotnet run`, port 4748) locks the ServiceHost build output. It can be applied once that host is stopped.
- **Report, screen and route IDs:** R-D5 data, S4, S5 and S6 (Installations and Maintenance tabs), and the 17 docs/13 Fleet operations.

## Independent scientific contract
NOT_APPLICABLE (SCIENTIFIC-CONTRACT.md). P01 has no computed engineering value.

## Reproducible E2E (J1, API)
- **Dataset:** the owner's facts (I-01, partial):
  - EP-DOA, `Eng1S1` on position 1 and `Eng2S2` on position 2, installed 2026-09-24.
  - The engine model is **not supplied**, so it is test-labelled `TEST-MODEL-UNVERIFIED` and no model is guessed.
  - A third ESN is labelled `TEST-ESN-SPARE-NOT-REAL`.
- **Owner path:** `Flows/FleetSetupWalkthroughTests`:
  1. Create aircraft, then model, then engines.
  2. Install both engines.
  3. Try to install the spare on position 1 → **409 80211**: "…overlaps installation …: engine Eng1S1 on aircraft EP-DOA position 1 from 2026-09-24T00:00:00Z to open".
  4. Record a WASH marker.
  5. The aircraft shows position 1 = Eng1S1 and position 2 = Eng2S2.
- **Swap in a dated slot:** `InstallationTests` closes ESN-1 at T and installs ESN-2 at T. The history holds both periods, each with INSTALLED / REMOVED RegistryChanges and an increasing RegistryVersion.
- **Negative paths, all tested:**
  - overlap on the same position
  - the same engine on two aircraft
  - removal ≤ installation
  - closing twice
  - position 3
  - duplicate registration, model code and serial
  - variant without evidence
  - negative counters
  - a maintenance event naming both an engine and a position, or neither
  - negative hours
- **Audit:** setting life counters writes an UPDATED `Engine` entry, with before and after counters, and a CREATED `LifeCounterAdjustment` entry.
- **401/403:** NOT DELIVERED (security deferred by the owner).
- **Tests:**
  - From a clean clone they need only a connection string.
  - Logs: `evidence/conformance-tests.txt` (45/45) and `evidence/acceptance-tests.txt` (50/50).
  - The throwaway databases were dropped; only `DotAirEctm` (legacy, untouched) and `DotAirEctmV1` remain.

## Delta inventory
| Kind | Added in P01 |
|---|---|
| Domain concepts | Aircraft, EngineModel, Engine + LifeCounterAdjustment, EngineInstallation + RegistryChange, MaintenanceEvent |
| Enums | AircraftStatus, EngineStatus, LifeCounterSource, RegistryChangeType, MaintenanceEventType, DateReliability, IdentityKind |
| Tables | Aircraft, EngineModels, Engines, EngineLifeCounterAdjustments, EngineInstallations, RegistryChanges, MaintenanceEvents (+ sequence RegistryVersions) |
| Endpoints | 17 (route inventory test: 4 P00 + 17 P01) |
| UI pages | none here |
| Tests | 28 new domain tests, 27 new acceptance tests |

## Deviations
- **Carried over from P00:** Consumers kept, Framework `ApiResult` envelope, Ordering query pattern, no security.
- **New in P01:** none. R-D5 and S12 are left out per docs/14 (GAP-ANALYSIS §2). The maintenance "reset flag" is deferred to P07 per docs/01.

## Stage closure
- **`SOFTWARE_VERIFIED`:** PASS for the fleet registry; build clean, 95/95 tests. The security part remains deferred.
- **`ENGINEERING_APPROVED`:** NOT_APPLICABLE.
- **`OPERATIONAL_STATUS`:** AVAILABLE for registry entry. The real fleet is incomplete until the engine model is supplied.
- **Open owner decisions and inputs:**
  1. **Engine model of Eng1S1 and Eng2S2:** code and manufacturer (I-01), plus variant and rating with evidence, if known (I-13).
  2. **Applying `AddFleetRegistry` to `DotAirEctmV1`:** stop the running host, or let me stop it.
  3. **Loading the real EP-DOA fleet into `DotAirEctmV1`:** once (1) arrives.
  4. **Still open from P00:** fixture publication; package vulnerability NU1902/NU1903.
- **Owner signature and gate to start P02:** A
