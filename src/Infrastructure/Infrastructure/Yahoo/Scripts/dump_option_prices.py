"""
fetch_options.py
Fetches full options chains (all maturities and strikes) for a list of tickers
from Yahoo Finance and dumps to JSON. Handles missing data/NaN smoothly.

Usage:
    python fetch_options.py                         # uses default tickers
    python fetch_options.py AAPL MSFT              # custom tickers
    python fetch_options.py --out data/options.json AAPL MSFT
"""

import json
import argparse
import math
from datetime import date
import yfinance as yf

DEFAULT_TICKERS = ["SPY", "AAPL"]

def clean_float(val) -> float | None:
    """Safely converts value to float, handling None and NaN."""
    if val is None or (isinstance(val, float) and math.isnan(val)):
        return None
    return float(val)

def clean_int(val) -> int | None:
    """Safely converts value to int, handling None and NaN float boundaries."""
    if val is None or (isinstance(val, float) and math.isnan(val)):
        return None
    return int(val)

def fetch_options_chains(tickers: list[str]) -> dict:
    results = {}

    for ticker in tickers:
        print(f"Fetching option data for {ticker}...")
        try:
            t = yf.Ticker(ticker)
            
            try:
                spot = t.fast_info.last_price
            except Exception:
                spot = None

            expirations = t.options
            if not expirations:
                print(f"  [WARN] No options chains found for {ticker}, skipping")
                continue

            print(f"  Found {len(expirations)} expiration dates.")
            results[ticker] = {
                "underlyingPrice": round(spot, 4) if spot else None,
                "asOf": date.today().isoformat(),
                "chains": {}
            }

            for exp in expirations:
                print(f"    Fetching maturity: {exp}...", end="\r")
                try:
                    chain = t.option_chain(exp)
                    
                    calls_list = []
                    for _, row in chain.calls.iterrows():
                        calls_list.append({
                            "contract":     row.get("contractSymbol"),
                            "strike":       clean_float(row.get("strike")),
                            "lastPrice":    clean_float(row.get("lastPrice")),
                            "bid":          clean_float(row.get("bid")),
                            "ask":          clean_float(row.get("ask")),
                            "volume":       clean_int(row.get("volume")),
                            "openInterest": clean_int(row.get("openInterest")),
                            "impliedVol":   round(clean_float(row.get("impliedVolatility")), 4) if row.get("impliedVolatility") is not None else None,
                        })

                    puts_list = []
                    for _, row in chain.puts.iterrows():
                        puts_list.append({
                            "contract":     row.get("contractSymbol"),
                            "strike":       clean_float(row.get("strike")),
                            "lastPrice":    clean_float(row.get("lastPrice")),
                            "bid":          clean_float(row.get("bid")),
                            "ask":          clean_float(row.get("ask")),
                            "volume":       clean_int(row.get("volume")),
                            "openInterest": clean_int(row.get("openInterest")),
                            "impliedVol":   round(clean_float(row.get("impliedVolatility")), 4) if row.get("impliedVolatility") is not None else None,
                        })

                    results[ticker]["chains"][exp] = {
                        "calls": calls_list,
                        "puts":  puts_list
                    }

                except Exception as e:
                    print(f"\n    [ERROR] Failed to fetch chain for {exp}: {e}")
            
            print(f"  Successfully loaded all chains for {ticker}.{' ' * 20}")

        except Exception as e:
            print(f"  [ERROR] Processing ticker {ticker}: {e}")

    return results

def write_json(results: dict, path: str) -> None:
    if not results:
        print("No data to write.")
        return

    with open(path, "w") as f:
        json.dump(results, f, indent=2)

    total_contracts = sum(
        len(expiry_data["calls"]) + len(expiry_data["puts"])
        for ticker_data in results.values()
        for expiry_data in ticker_data["chains"].values()
    )

    print(f"\nWrote {len(results)} tickers ({total_contracts} total option contracts) to {path}")

def main():
    parser = argparse.ArgumentParser(description="Fetch all options chains from Yahoo Finance")
    parser.add_argument("tickers", nargs="*", help="Ticker symbols")
    parser.add_argument("--out", default="options.json", help="Output JSON path (default: options.json)")
    args = parser.parse_args()

    tickers = [t.upper() for t in args.tickers] if args.tickers else DEFAULT_TICKERS

    print(f"Starting bulk options fetch for: {tickers}\n")
    results = fetch_options_chains(tickers)
    write_json(results, args.out)

if __name__ == "__main__":
    main()