# P01 — Gap analysis

## 1. Source lock
- **Legacy:** `AeroTech.ECTM@0ad5f93` and `Aerotech.ECTM.Backoffice@fd01cb5`, both unchanged since P00 and clean.
- **V1 baseline:** P00 (uncommitted, on top of `cf126e3`).

## 2. Pack conflict, resolved by the pack's own authority order
- **The conflict:**
  - `agent-prompts/PHASE-01.md` asks for "report R-D5 and the S12 viewer/export skeleton" and "R-D5 exported".
  - docs/14 P01 says "R-D5 data available to P11 (**NO S12 report stub**)".
  - docs/26 F12 and docs/30 §F/§G forbid dummy reports and blank stubs. docs/30 lists R-D5 among the four frozen reports of P11.
- **Resolution:** the authority order (00-START-HERE: numbered docs before agent prompts) makes docs/14 win. P01 delivers the data R-D5 needs (aircraft, ESN locations, dated installations and removals, engine status, life counters) through the fleet routes. The report itself and S12 are P11.

## 3. Legacy code read, and the decision for each item
| Legacy path | Decision | Reason |
|---|---|---|
| `Domain/AircraftAggregate/Aircraft.cs`, `Contracts/IAircraftRepository.cs` | RE-IMPLEMENT | adds type, engine count and status (docs/01) |
| `Domain/EngineModelAggregate/EngineModel.cs` | RE-IMPLEMENT | adds variant, thrust rating, document references and notes (docs/01) |
| `Domain/EngineAggregate/Engine.cs` | RE-IMPLEMENT | keeps manufacturer + serial identity and the documented configuration with evidence; adds status, life counters and the adjustment history |
| `Domain/EngineInstallationAggregate/EngineInstallation.cs`, `Entities/RegistryChange.cs` | PORT logic | half-open interval overlap per engine and per aircraft position, close with a REMOVED change, RegistryVersion sequence. Changes: the reason is required, evidence is added, and the error names the conflict. |
| `Persistence/EngineInstallationAggregate/*Configuration.cs` | PORT | position and interval check constraints, filtered unique "one open installation per engine / per position" indexes |
| `Domain/EngineMaintenanceEventAggregate/EngineMaintenanceEvent.cs` | RE-IMPLEMENT | uses the pack field set (`eventUtc`, hours/cycles at event, work order, notes). Legacy's `EventDate` and `SourceReference` are dropped. ENGINE / POSITION_ONLY identity is kept. |
| `Domain/_Shared/EnginePositions.cs` | PORT | positions 1 and 2 |
| `Application/AircraftAggregate/Commands/CreateAircraft/*`, `Services/AircraftLock.cs`, `EngineInstallationAggregate/Commands/*`, `Services/EngineInstallationLock.cs`, `_Shared/Concurrency/ResourceLockOptions.cs` | RE-IMPLEMENT (pattern) | command → handler → service; per-aggregate distributed locks around uniqueness and overlap checks |
| `Persistence/EctmDbContext.cs` unique and concurrency mapping | PORT | the first mutations arrive in P01 |
| `Contracts/AeroTech.Messages/Ectm/Enums/{MaintenanceEventType,DateReliability,IdentityKind,RegistryChangeType}.cs` | PORT names | the same concepts are in docs/01 |
| `Application/_Shared/Labels/ReferenceLabelLoader` | IGNORE | V1 services load display labels directly by id |
| `EngineAggregate` "Configuration" route `PUT Engines/{id}/Configuration` | IGNORE | docs/13 puts variant, rating and evidence in `PUT engines/{id}` |

## 4. Decisions taken for small gaps
- **Statuses:** Aircraft status ACTIVE / STORED / RETIRED. Engine status ACTIVE / STORED / LEASED / RETIRED (docs/10 R-D5 "engines in storage/lease"). Each status is declared by the owner; installation is shown separately.
- **Immutable identity fields:** registration, model code, engine serial number and engine model cannot be changed by PUT. They are identity keys for flight resolution (P03).
- **Engine manufacturer:** taken from the engine model when the engine is registered. Serial numbers are unique per manufacturer.
- **Configuration evidence:** a variant or thrust rating requires an evidence reference. Evidence is stamped with the actor and the clock when it changes.
- **Life counters:**
  - The counters are hours and cycles since new and since the last event, plus basis UTC and source.
  - Values are optional and must be ≥ 0. A basis UTC is required whenever counters are set.
  - Every setting, including the initial one at registration, appends one adjustment holding the full counter set, a reason, the actor and the time.
  - The source is always MANUAL; COMPUTED arrives with accepted flights.
- **Installation:**
  - The interval is half-open `[installedUtc, removedUtc)`, so a swap at the same instant is allowed.
  - Install and close both need a reason. Evidence is optional.
  - Overlap returns 409 and names the conflicting installation: id, engine serial, aircraft registration, position and period.
- **Maintenance event:**
  - ENGINE events carry an engine only. POSITION_ONLY events carry an aircraft and a position, and no engine.
  - `eventUtc` is required, and DAY_ONLY or APPROXIMATE qualify it.
  - Hours and cycles must be ≥ 0. No automatic effect on any counter or baseline.
- **Audit:** every changed entity is now audited, child entities included (life counter adjustments, registry changes), not only aggregate roots.
- **Error codes:** 80000–80001 are shared (operation in progress, changed concurrently). 80200–80299 are the fleet registry.

## 5. Delta inventory planned
| Kind | Items |
|---|---|
| Domain concepts | Aircraft, EngineModel, Engine (+ LifeCounterAdjustment), EngineInstallation (+ RegistryChange), MaintenanceEvent |
| Tables | Aircraft, EngineModels, Engines, EngineLifeCounterAdjustments, EngineInstallations, RegistryChanges, MaintenanceEvents; sequence RegistryVersions |
| Endpoints | the 19 docs/13 Fleet operations |
| UI pages | none here (frontend repository: S4–S6 without analytic tabs) |
| Tests | domain overlap and identity rules; acceptance for every route, overlap naming, swap, POSITION_ONLY event, counter adjustment audit |
