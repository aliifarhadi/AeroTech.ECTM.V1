"""For each engine channel: runs of identical values while that engine runs (N1 >= 40 %), and how much
that engine's N1 moved during the run. Over every real CSV in samples/.

Usage: python -I stuck_response_corpus.py <repo-root> <output-json>
"""
import csv, io, json, sys
from collections import defaultdict
from pathlib import Path

root = Path(sys.argv[1])
seeds = root / "src/AeroTech.ECTM.Domain/ConfigurationDocumentAggregate/Seeds"
profile = json.loads((seeds / "data-profile-ep-doa-seed.json").read_text(encoding="utf-8"))
catalog = json.loads((seeds / "parameter-catalog-ep-doa-seed.json").read_text(encoding="utf-8"))
numeric = {p["code"] for p in catalog["parameters"] if p["valueKind"] in ("NUMERIC", "INTEGER")}
engine = [(c["ordinal"], c["parameterCode"], c["position"]) for c in profile["columns"]
          if c["position"] and c["parameterCode"] in numeric and c["parameterCode"] != "N1_PCT"]
n1 = {c["position"]: c["ordinal"] for c in profile["columns"] if c["parameterCode"] == "N1_PCT"}

def value(row, ordinal):
    try:
        return float(row[ordinal])
    except ValueError:
        return None

DURATIONS = (10, 30, 60)
worst = defaultdict(lambda: {d: (0.0, "") for d in DURATIONS})
for path in sorted((root / "samples").rglob("*.csv")):
    rows = list(csv.reader(io.StringIO(path.read_text(encoding="utf-8"), newline="")))[1:]
    for ordinal, code, position in engine:
        run_value, low, high, length = None, None, None, 0
        def close():
            if run_value is None:
                return
            for duration in DURATIONS:
                if length >= duration and high - low > worst[code][duration][0]:
                    worst[code][duration] = (high - low, path.name)
        for row in rows:
            speed, cell = value(row, n1[position]), row[ordinal].strip()
            if speed is None or speed < 40 or cell == "":
                close(); run_value = None; continue
            if cell == run_value:
                low, high, length = min(low, speed), max(high, speed), length + 1
            else:
                close(); run_value, low, high, length = cell, speed, speed, 1
        close()
out = {}
for code, by_duration in sorted(worst.items()):
    out[code] = {f"runsAtLeast{d}s": {"maxN1RangePct": round(v[0], 2), "file": v[1]} for d, v in by_duration.items()}
    print(f"{code:26} " + "  ".join(f">={d:2}s: {v[0]:6.2f} %" for d, v in by_duration.items()))
Path(sys.argv[2]).write_text(json.dumps(out, indent=1), encoding="utf-8")
