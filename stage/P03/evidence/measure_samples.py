"""Measures every real CSV under samples/ against the EP-DOA seed profile.

Usage: python -I measure_samples.py <repo-root> <output-json>
Stdlib only. Writes a JSON report and prints a summary.
"""
import csv
import hashlib
import io
import json
import math
import re
import statistics
import sys
from collections import Counter, defaultdict
from pathlib import Path

root = Path(sys.argv[1])
output = Path(sys.argv[2])
profile = json.loads((root / "src/AeroTech.ECTM.Domain/ConfigurationDocumentAggregate/Seeds/data-profile-ep-doa-seed.json").read_text(encoding="utf-8"))
columns = profile["columns"]
expected_header = [column["exactHeader"] for column in columns]
text_units = {"ENUM", "HHMMSS", "TEXT", "DATE", "IDENTIFIER"}
name_pattern = re.compile(r"^(\d{4})-(\d{2})-(\d{2})_([A-Z0-9-]+)_([A-Z0-9]+)\.csv$", re.IGNORECASE)

files = sorted((root / "samples").rglob("*.csv"))
report = {"fileCount": len(files), "files": [], "units": sorted({c["sourceUnit"] for c in columns})}
per_column = defaultdict(lambda: {"min": math.inf, "max": -math.inf, "missing": 0, "unparseable": 0, "values": 0, "tokens": Counter()})
sha_seen = defaultdict(list)
identity_seen = defaultdict(list)
step_counter = Counter()

for path in files:
    content = path.read_bytes()
    sha = hashlib.sha256(content).hexdigest()
    sha_seen[sha].append(path.name)
    entry = {"name": path.name, "folder": path.parent.name, "sizeBytes": len(content), "sha256": sha}
    entry["bom"] = content.startswith(b"\xef\xbb\xbf")
    try:
        text = content.decode("utf-8-sig" if entry["bom"] else "utf-8")
        entry["utf8"] = True
    except UnicodeDecodeError as error:
        entry["utf8"] = False
        entry["utf8Error"] = str(error)
        report["files"].append(entry)
        continue
    entry["crlf"] = "\r\n" in text
    rows = list(csv.reader(io.StringIO(text, newline="")))
    header = rows[0]
    data = rows[1:]
    trailing_empty = 0
    while data and all(cell == "" for cell in data[-1]):
        data.pop()
        trailing_empty += 1
    entry["trailingEmptyRows"] = trailing_empty
    entry["headerMatches"] = header == expected_header
    if not entry["headerMatches"]:
        entry["headerLength"] = len(header)
        entry["headerDiff"] = [(i, expected_header[i] if i < len(expected_header) else None, header[i] if i < len(header) else None) for i in range(max(len(header), len(expected_header))) if (expected_header[i] if i < len(expected_header) else None) != (header[i] if i < len(header) else None)][:10]
    entry["rowCount"] = len(data)
    lengths = Counter(len(row) for row in data)
    entry["rowLengths"] = dict(lengths)
    match = name_pattern.match(path.name)
    entry["nameMatches"] = bool(match)
    if match:
        entry["nameDate"] = f"{match.group(1)}-{match.group(2)}-{match.group(3)}"
        entry["nameRegistration"] = match.group(4)
        entry["nameFlightNumber"] = match.group(5)
        identity_seen[(entry["nameDate"], entry["nameRegistration"], entry["nameFlightNumber"])].append(path.name)

    by_code = {}
    for index, column in enumerate(columns):
        key = column["parameterCode"] + (f"[{column['position']}]" if column["position"] else "")
        by_code[key] = index

    elapsed_index = by_code["ELAPSED_S"]
    elapsed = []
    elapsed_unparseable = 0
    for row in data:
        cell = row[elapsed_index].strip() if elapsed_index < len(row) else ""
        try:
            elapsed.append(float(cell))
        except ValueError:
            elapsed.append(None)
            elapsed_unparseable += 1
    valued = [value for value in elapsed if value is not None]
    entry["elapsedFirst"] = valued[0] if valued else None
    entry["elapsedLast"] = valued[-1] if valued else None
    entry["elapsedUnparseable"] = elapsed_unparseable
    steps = [round(b - a, 6) for a, b in zip(valued, valued[1:])]
    file_steps = Counter(steps)
    step_counter.update(file_steps)
    entry["elapsedSteps"] = {str(k): v for k, v in file_steps.most_common(8)}
    entry["elapsedDuplicate"] = sum(1 for s in steps if s == 0)
    entry["elapsedDecreasing"] = sum(1 for s in steps if s < 0)
    entry["elapsedMaxStep"] = max(steps) if steps else None
    entry["durationSeconds"] = (valued[-1] - valued[0]) if valued else None

    for text_key in ("FLIGHT_NUMBER", "DAY", "MONTH", "UTC_PRIMARY", "AIR_GROUND"):
        if text_key in by_code:
            index = by_code[text_key]
            values = Counter(row[index].strip() for row in data if index < len(row))
            entry[f"{text_key}Values"] = dict(values.most_common(6))

    for column_index, column in enumerate(columns):
        key = column["parameterCode"] + (f"[{column['position']}]" if column["position"] else "")
        stats = per_column[key]
        stats["unit"] = column["sourceUnit"]
        for row in data:
            raw = row[column_index] if column_index < len(row) else ""
            cell = raw.strip()
            if cell in ("",) or raw in column["missingTokens"]:
                stats["missing"] += 1
                continue
            if column["sourceUnit"] in text_units:
                stats["tokens"][cell] += 1
                stats["values"] += 1
                continue
            try:
                number = float(cell)
            except ValueError:
                stats["unparseable"] += 1
                stats["tokens"][cell] += 1
                continue
            if not math.isfinite(number):
                stats["unparseable"] += 1
                continue
            stats["values"] += 1
            stats["min"] = min(stats["min"], number)
            stats["max"] = max(stats["max"], number)
    report["files"].append(entry)

report["duplicateSha"] = {sha: names for sha, names in sha_seen.items() if len(names) > 1}
report["duplicateIdentity"] = {"|".join(key): names for key, names in identity_seen.items() if len(names) > 1}
report["elapsedStepsAll"] = {str(k): v for k, v in step_counter.most_common(15)}
report["columns"] = {key: {"unit": s["unit"], "min": None if s["min"] == math.inf else s["min"], "max": None if s["max"] == -math.inf else s["max"], "missing": s["missing"], "unparseable": s["unparseable"], "values": s["values"], "topTokens": dict(s["tokens"].most_common(5))} for key, s in per_column.items()}
sizes = [f["sizeBytes"] for f in report["files"]]
rows = [f["rowCount"] for f in report["files"] if "rowCount" in f]
report["summary"] = {
    "sizeMin": min(sizes), "sizeMax": max(sizes), "sizeMedian": statistics.median(sizes), "sizeTotal": sum(sizes),
    "rowsMin": min(rows), "rowsMax": max(rows), "rowsMedian": statistics.median(rows), "rowsTotal": sum(rows),
    "headerMismatch": [f["name"] for f in report["files"] if not f.get("headerMatches")],
    "nameMismatch": [f["name"] for f in report["files"] if not f.get("nameMatches")],
    "nonUtf8": [f["name"] for f in report["files"] if not f.get("utf8")],
    "bom": [f["name"] for f in report["files"] if f.get("bom")],
    "ragged": [f["name"] for f in report["files"] if len(f.get("rowLengths", {})) > 1 or any(int(k) != 85 for k in f.get("rowLengths", {}))],
    "withDuplicateElapsed": [f["name"] for f in report["files"] if f.get("elapsedDuplicate")],
    "withDecreasingElapsed": [f["name"] for f in report["files"] if f.get("elapsedDecreasing")],
    "maxStepOverall": max((f["elapsedMaxStep"] or 0) for f in report["files"] if "elapsedMaxStep" in f),
    "durationMaxSeconds": max((f["durationSeconds"] or 0) for f in report["files"] if "durationSeconds" in f),
    "dates": sorted({f.get("nameDate") for f in report["files"] if f.get("nameDate")}),
    "registrations": sorted({f.get("nameRegistration") for f in report["files"] if f.get("nameRegistration")}),
    "flightsPerDayMax": max(Counter(f.get("nameDate") for f in report["files"]).values()),
}
output.write_text(json.dumps(report, indent=1, default=str), encoding="utf-8")
print(json.dumps(report["summary"], indent=1, default=str))
print("steps", report["elapsedStepsAll"])
print("dupSha", report["duplicateSha"])
print("dupIdentity", report["duplicateIdentity"])
