"""Runs the pack oracle's phase detection on the 8 golden files (parity check) and on every real CSV in samples/.

Usage: python -I phase_corpus.py <repo-root> <output-json>
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
    return flight, oracle.phase(flight, policy, quality)


parity = {}
for golden in sorted((root / "tests/fixtures/golden").glob("*.oracle.json")):
    expected = json.loads(golden.read_text(encoding="utf-8"))["phase"]
    _, actual = analyse(root / "tests/fixtures/real" / golden.name.replace(".oracle.json", ".csv"))
    parity[golden.name] = expected == actual
    if expected != actual:
        print("PARITY DIFF", golden.name, [key for key in expected if expected.get(key) != actual.get(key)], flush=True)
print("oracle reproduces goldens:", all(parity.values()), f"({sum(parity.values())}/{len(parity)})", flush=True)

flights = []
for number, path in enumerate(sorted((root / "samples").rglob("*.csv")), start=1):
    flight, phase = analyse(path)
    last120 = sum(segment["endIndexInclusive"] - max(segment["startIndex"], flight.n - 120) + 1
                  for segment in phase["segments"]
                  if segment["phaseCode"] == "CLIMB" and segment["endIndexInclusive"] >= flight.n - 120)
    flights.append(dict(
        file=path.name,
        n=flight.n,
        liftoffBasis=phase["liftoffBasis"],
        touchdownBasis=phase["touchdownBasis"],
        unknownPct=100.0 * phase["coverage"].get("UNKNOWN", 0) / flight.n,
        cruise=phase["coverage"].get("CRUISE", 0) > 0,
        landingRoll=phase["coverage"].get("LANDING_ROLL", 0) > 0,
        climbInLast120=last120,
        phase=phase))
    if number % 20 == 0:
        print(f"{number} files done", flush=True)

summary = dict(
    files=len(flights),
    liftoffBasis=dict(Counter(flight["liftoffBasis"] for flight in flights)),
    touchdownBasis=dict(Counter(flight["touchdownBasis"] for flight in flights)),
    withCruise=sum(flight["cruise"] for flight in flights),
    withLandingRoll=sum(flight["landingRoll"] for flight in flights),
    climbInLast120=[flight["file"] for flight in flights if flight["climbInLast120"]],
    unknownPct=dict(min=round(min(flight["unknownPct"] for flight in flights), 2), max=round(max(flight["unknownPct"] for flight in flights), 2)),
    highestUnknown=sorted(((round(flight["unknownPct"], 1), flight["file"]) for flight in flights), reverse=True)[:5],
)
Path(sys.argv[2]).write_text(json.dumps(dict(summary=summary, flights=flights), indent=1), encoding="utf-8")
print(json.dumps(summary, indent=1))
