"""
spx_futures_dump.py
--------------------
Fetches the next N quarterly CME E-mini S&P 500 (ES) futures maturities
from Yahoo Finance and dumps them as JSON.
 
CME quarterly cycle: March (H), June (M), September (U), December (Z)
Yahoo Finance ticker format: ES<MonthCode><YY>.CME  e.g. ESM25.CME
 
Usage:
    python spx_futures_dump.py                  # next 4 maturities, saves to spx_futures.json
    python spx_futures_dump.py --n 2            # next 2 maturities
    python spx_futures_dump.py --out my.json    # custom output path
    python spx_futures_dump.py --print          # pretty-print to stdout only
"""
 
import argparse
import json
import sys
from datetime import datetime, date, timedelta
import yfinance as yf
 
 
# ── CME quarterly expiry calendar ──────────────────────────────────────────────
 
MONTH_CODES = {3: "H", 6: "M", 9: "U", 12: "Z"}   # March, June, Sept, Dec
QUARTERLY_MONTHS = sorted(MONTH_CODES.keys())
 
 
def third_friday(year: int, month: int) -> date:
    """Return the third Friday of the given year/month (CME ES expiry day)."""
    # Find first Friday
    first_day = date(year, month, 1)
    # weekday(): Monday=0 … Friday=4
    days_until_friday = (4 - first_day.weekday()) % 7
    first_friday = first_day + timedelta(days=days_until_friday)
    return first_friday + timedelta(weeks=2)  # third Friday
 
 
def next_expiries(n: int = 4, reference: date | None = None) -> list[dict]:
    """
    Return the next `n` quarterly ES expiry dates after `reference` (today if None).
    Each item: {"maturity": "YYYY-MM-DD", "ticker": "ESM25.CME", "month_code": "M", "year": 25}
    """
    if reference is None:
        reference = date.today()
 
    expiries = []
    year = reference.year
 
    # Generate candidates a few years out to be safe
    for y in range(year, year + 3):
        for m in QUARTERLY_MONTHS:
            exp = third_friday(y, m)
            if exp > reference:
                code = MONTH_CODES[m]
                yy = y % 100
                ticker = f"ES{code}{yy:02d}.CME"
                expiries.append({
                    "maturity": exp.isoformat(),
                    "ticker": ticker,
                    "month_code": code,
                    "year": y,
                })
 
    expiries.sort(key=lambda x: x["maturity"])
    return expiries[:n]
 
 
# ── Yahoo Finance fetch ─────────────────────────────────────────────────────────
 
def fetch_price(ticker: str) -> float | None:
    """Fetch latest price for a Yahoo Finance ticker. Returns None on failure."""
    try:
        tk = yf.Ticker(ticker)
        # fast_info is lighter than .info (no HTML scrape)
        fast = tk.fast_info
        price = fast.get("last_price") or fast.get("regularMarketPrice")
        if price:
            return float(price)
 
        # Fallback: pull last closing price from 5-day history
        hist = tk.history(period="5d")
        if not hist.empty:
            return float(hist["Close"].iloc[-1])
    except Exception as exc:
        print(f"  [warn] Could not fetch {ticker}: {exc}", file=sys.stderr)
    return None
 
 
# ── Main ────────────────────────────────────────────────────────────────────────
 
def build_snapshot(n: int = 4) -> dict:
    expiries = next_expiries(n)
    records = []
 
    print(f"Fetching {n} nearest ES futures maturities from Yahoo Finance …\n")
    for exp in expiries:
        ticker = exp["ticker"]
        print(f"  {ticker}  (expires {exp['maturity']}) … ", end="", flush=True)
        price = fetch_price(ticker)
        status = f"{price:.2f}" if price else "N/A"
        print(status)
        records.append({
            "ticker":   ticker,
            "maturity": exp["maturity"],
            "price":    price,
            "currency": "USD",
        })
 
    snapshot = {
        "generated_at": datetime.utcnow().strftime("%Y-%m-%dT%H:%M:%SZ"),
        "underlying":   "S&P 500 (ES E-mini)",
        "exchange":     "CME",
        "futures":      records,
    }
    return snapshot
 
 
def main():
    parser = argparse.ArgumentParser(description="Dump SPX (ES) futures prices from Yahoo Finance.")
    parser.add_argument("--n",     type=int, default=4,               help="Number of next maturities to fetch (default: 4)")
    parser.add_argument("--out",   type=str, default="spx_futures.json", help="Output JSON file path (default: spx_futures.json)")
    parser.add_argument("--print", dest="print_only", action="store_true", help="Print JSON to stdout, do not write file")
    args = parser.parse_args()
 
    snapshot = build_snapshot(n=args.n)
 
    output = json.dumps(snapshot, indent=2)
 
    if args.print_only:
        print("\n" + output)
    else:
        with open(args.out, "w") as f:
            f.write(output)
        print(f"\nSaved → {args.out}")
        print(json.dumps(snapshot, indent=2))
 
 
if __name__ == "__main__":
    main()