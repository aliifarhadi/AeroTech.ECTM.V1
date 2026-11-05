"""Runs the pack oracle's quality pre-pass on every real CSV in samples/ and summarises it.

Usage: python -I quality_corpus.py <repo-root> <output-json>
"""
import importlib.util, json, statistics, sys
from collections import Counter
from pathlib import Path

root = Path(sys.argv[1])
pack = root / "docs/ECTM-v3.3-Product-Pack/ECTM-v3.3-Product-Pack"
spec = importlib.util.spec_from_file_location("ectm_oracle", pack / "reference/ectm_oracle.py")
oracle = importlib.util.module_from_spec(spec)
spec.loader.exec_module(oracle)
policy = oracle.load_policy(pack / "contracts/policy-defaults-v1.json")

results = []
for path in sorted((root / "samples").rglob("*.csv")):
    flight = oracle.Flight(path)
    quality = oracle.quality(flight, policy)
    quality["file"] = path.name
    results.append(quality)

def summary(values):
    values = [v for v in values if v is not None]
    return {"n": len(values), "min": min(values), "median": statistics.median(values), "max": max(values)} if values else None

codes = Counter(span["code"] for q in results for span in q["spans"])
disc = Counter(d["code"] for q in results for d in q["discontinuities"])
report = {
    "files": len(results),
    "partialStart": sum(q["partialStart"] for q in results),
    "partialEnd": sum(q["partialEnd"] for q in results),
    "airGroundLagSec": summary([q["airGroundLagSec"] for q in results]),
    "discontinuities": dict(disc),
    "filesWithoutConfirmedJump": [q["file"] for q in results if not any(d["code"] == "ALTITUDE_DISCONTINUITY_CONFIRMED" for d in q["discontinuities"])],
    "spans": dict(codes),
    "timeTrust": dict(Counter(q["clock"]["timeTrust"] for q in results)),
    "invalidOrMissingRowsAll": sum(q["clock"]["invalidOrMissingRows"] == q["sampleCount"] for q in results),
    "elapsedGaps": {q["file"]: q["elapsedGaps"] for q in results if q["elapsedGaps"]},
    "jumpSizes": summary([d["dAltFtPerSec"] for q in results for d in q["discontinuities"]]),
}
Path(sys.argv[2]).write_text(json.dumps({"summary": report, "flights": results}, indent=1), encoding="utf-8")
print(json.dumps(report, indent=1))
