"""
fetch_timeseries.py
Fetches OHLCV time series for a list of tickers from Yahoo Finance
and dumps to JSON.

Usage:
    python fetch_timeseries.py                                      # default tickers, 1y daily
    python fetch_timeseries.py AAPL MSFT EURUSD=X                  # custom tickers
    python fetch_timeseries.py --period 6mo --interval 1wk AAPL    # weekly bars over 6 months
    python fetch_timeseries.py --start 2023-01-01 --end 2024-01-01 SPY QQQ
    python fetch_timeseries.py --out data/ts.json SPY               # custom output path

Valid periods  : 1d 5d 1mo 3mo 6mo 1y 2y 5y 10y ytd max
Valid intervals: 1m 2m 5m 15m 30m 60m 90m 1h 1d 5d 1wk 1mo 3mo
  (intraday intervals only available for periods ≤ 60 days)
"""

import json
import argparse
from datetime import date
import yfinance as yf
import pandas as pd

DEFAULT_TICKERS = ["^GSPC", "MSFT", "AAPL", "EURUSD=X"] # S&P 500

QUOTE_TYPE_MAP = {
    "EQUITY":         "Stock",
    "ETF":            "Etf",
    "INDEX":          "Index",
    "CURRENCY":       "CurrencyPair",
    "CRYPTOCURRENCY": "Crypto",
    "FUTURE":         "Future",
    "MUTUALFUND":     "MutualFund",
}


def fetch_timeseries(
    tickers: list[str],
    period: str | None,
    start: str | None,
    end: str | None,
    interval: str,
) -> dict:
    results = {}

    for ticker in tickers:
        try:
            t = yf.Ticker(ticker)
            fi = t.fast_info

            # --- download OHLCV bars ---
            kwargs = dict(interval=interval, auto_adjust=True)
            if start or end:
                if start:
                    kwargs["start"] = start
                if end:
                    kwargs["end"] = end
            else:
                kwargs["period"] = period or "1y"

            hist: pd.DataFrame = t.history(**kwargs)

            if hist.empty:
                print(f"  [WARN] No data for {ticker}, skipping")
                continue

            # normalise column names → camelCase
            hist.index = hist.index.tz_localize(None) if hist.index.tzinfo else hist.index
            records = []
            for ts, row in hist.iterrows():
                bar: dict = {"date": ts.strftime("%Y-%m-%d")}
                if interval in ("1m", "2m", "5m", "15m", "30m", "60m", "90m", "1h"):
                    bar["date"] = ts.strftime("%Y-%m-%dT%H:%M:%S")
                bar["open"]   = round(float(row["Open"]),   6)
                bar["high"]   = round(float(row["High"]),   6)
                bar["low"]    = round(float(row["Low"]),    6)
                bar["close"]  = round(float(row["Close"]),  6)
                if "Volume" in row and not pd.isna(row["Volume"]):
                    bar["volume"] = int(row["Volume"])
                records.append(bar)

            raw_type   = (fi.quote_type or "UNKNOWN").upper()
            asset_type = QUOTE_TYPE_MAP.get(raw_type, raw_type)

            results[ticker] = {
                "assetType": asset_type,
                "currency":  fi.currency or None,
                "interval":  interval,
                "start":     records[0]["date"] if records else None,
                "end":       records[-1]["date"] if records else None,
                "bars":      len(records),
                "series":    records,
            }

            print(
                f"  {ticker:<12}  {len(records):>5} bars  "
                f"{records[0]['date']} → {records[-1]['date']}  "
                f"{asset_type:<14}  {fi.currency or ''}"
            )

        except Exception as e:
            print(f"  [ERROR] {ticker}: {e}")

    return results


def write_json(results: dict, path: str) -> None:
    if not results:
        print("No data to write.")
        return

    with open(path, "w") as f:
        json.dump(results, f, indent=2)

    print(f"\nWrote {len(results)} ticker(s) to {path}")


def main():
    parser = argparse.ArgumentParser(
        description="Fetch OHLCV time series from Yahoo Finance and dump to JSON"
    )
    parser.add_argument("tickers", nargs="*", help="Ticker symbols (default: built-in list)")

    # date range — mutually exclusive with period
    group = parser.add_mutually_exclusive_group()
    group.add_argument(
        "--period", default="1y",
        help="Lookback period (default: 1y). Ignored if --start/--end are used.",
    )
    group.add_argument(
        "--start", metavar="YYYY-MM-DD",
        help="Start date (inclusive). Use with optional --end.",
    )

    parser.add_argument(
        "--end", metavar="YYYY-MM-DD", default=None,
        help="End date (exclusive). Defaults to today when --start is provided.",
    )
    parser.add_argument(
        "--interval", default="1d",
        help="Bar interval (default: 1d). E.g. 1d, 1wk, 1mo, 1h, 5m.",
    )
    parser.add_argument(
        "--out", default="timeseries.json",
        help="Output JSON path (default: timeseries.json)",
    )
    args = parser.parse_args()

    tickers = [t.upper() for t in args.tickers] if args.tickers else DEFAULT_TICKERS
    period  = None if args.start else args.period

    print(f"Fetching {args.interval} bars for: {tickers}")
    if args.start:
        print(f"  range: {args.start} → {args.end or date.today().isoformat()}")
    else:
        print(f"  period: {period}")
    print()

    results = fetch_timeseries(
        tickers,
        period=period,
        start=args.start,
        end=args.end,
        interval=args.interval,
    )
    write_json(results, args.out)


if __name__ == "__main__":
    main()