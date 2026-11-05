"""Runs the pack oracle's snapshots on the 8 golden files (parity check) and on every real CSV in samples/.

The oracle's paired E2-E1 value is a documented defect (docs/17 section 3); its paired section is kept only for reference.

Usage: python -I snapshot_corpus.py <repo-root> <output-json>
"""
import importlib.util
import json
import sys
from collections import Counter
from pathlib import Path

root = Path(sys.argv[1])
pack = root / "docs/ECTM-v3.3-Product-Pack/ECTM-v3.3-Product-Pack"
spec = importlib.util.spec_from_file_location("ectm_oracle", pack / "reference/ectm_oracle.py")
oracle = importlib.util.module_from_spec(spec)
spec.loader.exec_module(oracle)
policy = oracle.load_policy(pack / "contracts/policy-defaults-v1.json")


def analyse(path):
    flight = oracle.Flight(path)
    quality = oracle.quality(flight, policy)
    phase = oracle.phase(flight, policy, quality)
    return flight, oracle.snapshots(flight, policy, quality, phase)


parity = {}
for golden in sorted((root / "tests/fixtures/golden").glob("*.oracle.json")):
    expected = json.loads(golden.read_text(encoding="utf-8"))["snapshots"]
    _, actual = analyse(root / "tests/fixtures/real" / golden.name.replace(".oracle.json", ".csv"))
    parity[golden.name] = json.loads(json.dumps(actual)) == expected
print("oracle reproduces golden snapshots:", all(parity.values()), f"({sum(parity.values())}/{len(parity)})", flush=True)

flights = []
takeoff = Counter()
cruise = Counter()
for number, path in enumerate(sorted((root / "samples").rglob("*.csv")), start=1):
    flight, snapshots = analyse(path)
    for position, record in snapshots["takeoff"].items():
        takeoff[(position, record["availability"], record.get("reason") or record.get("searchBasis"), record.get("contextStatus"))] += 1
    for selector, record in snapshots["cruise"].items():
        for position, engine in record["engines"].items():
            cruise[(selector, position, engine["availability"], engine.get("reason"))] += 1
    flights.append(dict(file=path.name, n=flight.n, snapshots=json.loads(json.dumps(snapshots))))
    if number % 20 == 0:
        print(f"{number} files done", flush=True)

summary = dict(
    files=len(flights),
    takeoff={" ".join(str(part) for part in key): count for key, count in sorted(takeoff.items(), key=str)},
    cruise={" ".join(str(part) for part in key): count for key, count in sorted(cruise.items(), key=str)},
)
Path(sys.argv[2]).write_text(json.dumps(dict(summary=summary, flights=flights), indent=1), encoding="utf-8")
print(json.dumps(summary, indent=1))
