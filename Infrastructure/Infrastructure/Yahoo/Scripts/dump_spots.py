"""
fetch_spots.py
Fetches spot prices and metadata for a list of tickers from Yahoo Finance
and dumps to JSON.

Usage:
    python fetch_spots.py                            # uses default tickers
    python fetch_spots.py AAPL MSFT EURUSD=X        # custom tickers
    python fetch_spots.py --out data/spots.json AAPL MSFT EURUSD=X
"""

import json
import argparse
from datetime import date
import yfinance as yf

DEFAULT_TICKERS = ["SPY", "QQQ", "GLD", "TLT", "EURUSD=X"]

# Yahoo quote_type values → clean domain type names
QUOTE_TYPE_MAP = {
    "EQUITY":       "Stock",
    "ETF":          "Etf",
    "INDEX":        "Index",
    "CURRENCY":     "CurrencyPair",
    "CRYPTOCURRENCY": "Crypto",
    "FUTURE":       "Future",
    "MUTUALFUND":   "MutualFund",
}

def fetch_spots(tickers: list[str]) -> dict:
    results = {}

    for ticker in tickers:
        try:
            t = yf.Ticker(ticker)
            fi = t.fast_info

            spot = fi.last_price
            if spot is None or spot <= 0:
                print(f"  [WARN] No price for {ticker}, skipping")
                continue

            prev_close  = fi.previous_close
            change_pct  = ((spot - prev_close) / prev_close * 100) if prev_close else None
            raw_type    = fi.quote_type or "UNKNOWN"
            asset_type  = QUOTE_TYPE_MAP.get(raw_type.upper(), raw_type)

            results[ticker] = {
                "spot":       round(spot, 6),
                "prevClose":  round(prev_close, 6) if prev_close else None,
                "changePct":  round(change_pct, 4) if change_pct else None,
                "currency":   fi.currency or None,
                "assetType":  asset_type,
                "asOf":       date.today().isoformat(),
            }

            print(f"  {ticker:<12} {spot:>12.6f}  {asset_type:<14}  {fi.currency or ''}")

        except Exception as e:
            print(f"  [ERROR] {ticker}: {e}")

    return results


def write_json(results: dict, path: str) -> None:
    if not results:
        print("No data to write.")
        return

    with open(path, "w") as f:
        json.dump(results, f, indent=2)

    print(f"\nWrote {len(results)} entries to {path}")


def main():
    parser = argparse.ArgumentParser(description="Fetch spot prices from Yahoo Finance")
    parser.add_argument("tickers", nargs="*", help="Ticker symbols")
    parser.add_argument("--out", default="spots.json", help="Output JSON path (default: spots.json)")
    args = parser.parse_args()

    tickers = [t.upper() for t in args.tickers] if args.tickers else DEFAULT_TICKERS

    print(f"Fetching spots for: {tickers}\n")
    results = fetch_spots(tickers)
    write_json(results, args.out)


if __name__ == "__main__":
    main()
