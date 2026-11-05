"""Longest run of identical values per numeric engine channel while that engine runs (N1 >= 40 %), over samples/.

Usage: python -I stuck_corpus.py <repo-root> <output-json>
"""
import csv, io, json, sys
from collections import defaultdict
from pathlib import Path

root = Path(sys.argv[1])
profile = json.loads((root / "src/AeroTech.ECTM.Domain/ConfigurationDocumentAggregate/Seeds/data-profile-ep-doa-seed.json").read_text(encoding="utf-8"))
catalog = json.loads((root / "src/AeroTech.ECTM.Domain/ConfigurationDocumentAggregate/Seeds/parameter-catalog-ep-doa-seed.json").read_text(encoding="utf-8"))
numeric = {p["code"] for p in catalog["parameters"] if p["valueKind"] in ("NUMERIC", "INTEGER")}
engine = [(c["ordinal"], f'{c["parameterCode"]}[{c["position"]}]', c["position"]) for c in profile["columns"] if c["position"] and c["parameterCode"] in numeric]
n1 = {c["position"]: c["ordinal"] for c in profile["columns"] if c["parameterCode"] == "N1_PCT"}
longest = defaultdict(list)
for path in sorted((root / "samples").rglob("*.csv")):
    rows = list(csv.reader(io.StringIO(path.read_text(encoding="utf-8"), newline="")))[1:]
    for ordinal, key, position in engine:
        best = run = 0
        previous = None
        for row in rows:
            try:
                running = float(row[n1[position]]) >= 40
            except ValueError:
                running = False
            value = row[ordinal].strip()
            if not running or value == "":
                run, previous = 0, None
                continue
            run = run + 1 if value == previous else 1
            previous = value
            best = max(best, run)
        longest[key].append((best, path.name))
out = {}
for key, values in sorted(longest.items()):
    runs = sorted(v for v, _ in values)
    out[key] = {"median": runs[len(runs) // 2], "p90": runs[int(len(runs) * 0.9)], "max": runs[-1], "maxFile": max(values)[1]}
    print(f"{key:28} median {out[key]['median']:5} p90 {out[key]['p90']:5} max {out[key]['max']:5}  {out[key]['maxFile']}")
Path(sys.argv[2]).write_text(json.dumps(out, indent=1), encoding="utf-8")
