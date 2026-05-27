"""
fetch_options.py

Fetches full options chains (all maturities and strikes) for a list of
tickers or indices from Yahoo Finance and dumps to JSON.

Examples:
    python fetch_options.py
    python fetch_options.py AAPL MSFT
    python fetch_options.py SPX NDX RUT
    python fetch_options.py --out data/options.json SPX AAPL
"""

import json
import argparse
import math
from datetime import date
import yfinance as yf


DEFAULT_TICKERS = ["SPX", "SPY"]


# Index → Yahoo ticker + fallback ETF
INDEX_CONFIG = {
    "SPX": {"yahoo": "^SPX", "fallback": "SPY"},
    "NDX": {"yahoo": "^NDX", "fallback": "QQQ"},
    "RUT": {"yahoo": "^RUT", "fallback": "IWM"},
    "DJI": {"yahoo": "^DJI", "fallback": "DIA"},
    "VIX": {"yahoo": "^VIX", "fallback": "VXX"},
    "CAC": {"yahoo": "^FCHI", "fallback": None},
    "SX5E": {"yahoo": "^STOXX50E", "fallback": None},
}


def clean_float(val) -> float | None:
    """Safely converts value to float, handling None and NaN."""
    if val is None or (isinstance(val, float) and math.isnan(val)):
        return None
    return float(val)


def clean_int(val) -> int | None:
    """Safely converts value to int, handling None and NaN."""
    if val is None or (isinstance(val, float) and math.isnan(val)):
        return None
    return int(val)


def get_ticker(symbol: str):
    """
    Returns Yahoo ticker and display name.

    Example:
        SPX -> (^SPX, SPX, SPY fallback)
        AAPL -> (AAPL, AAPL, None)
    """
    symbol = symbol.upper()

    if symbol in INDEX_CONFIG:
        config = INDEX_CONFIG[symbol]
        return config["yahoo"], symbol, config["fallback"]

    return symbol, symbol, None


def get_spot_price(ticker):
    """Try several ways to retrieve underlying price."""
    try:
        return ticker.fast_info.last_price
    except Exception:
        pass

    try:
        return ticker.fast_info.get("lastPrice")
    except Exception:
        pass

    try:
        hist = ticker.history(period="1d")
        if not hist.empty:
            return float(hist["Close"].iloc[-1])
    except Exception:
        pass

    return None


def fetch_chain_for_ticker(ticker_obj, expirations):
    """Fetch all chains for one ticker object."""
    chains = {}

    for exp in expirations:
        print(f"    Fetching maturity: {exp}...", end="\r")

        try:
            chain = ticker_obj.option_chain(exp)

            calls_list = []
            for _, row in chain.calls.iterrows():
                iv = clean_float(row.get("impliedVolatility"))

                calls_list.append({
                    "contract": row.get("contractSymbol"),
                    "strike": clean_float(row.get("strike")),
                    "lastPrice": clean_float(row.get("lastPrice")),
                    "bid": clean_float(row.get("bid")),
                    "ask": clean_float(row.get("ask")),
                    "volume": clean_int(row.get("volume")),
                    "openInterest": clean_int(row.get("openInterest")),
                    "impliedVol": round(iv, 4) if iv is not None else None,
                })

            puts_list = []
            for _, row in chain.puts.iterrows():
                iv = clean_float(row.get("impliedVolatility"))

                puts_list.append({
                    "contract": row.get("contractSymbol"),
                    "strike": clean_float(row.get("strike")),
                    "lastPrice": clean_float(row.get("lastPrice")),
                    "bid": clean_float(row.get("bid")),
                    "ask": clean_float(row.get("ask")),
                    "volume": clean_int(row.get("volume")),
                    "openInterest": clean_int(row.get("openInterest")),
                    "impliedVol": round(iv, 4) if iv is not None else None,
                })

            chains[exp] = {
                "calls": calls_list,
                "puts": puts_list
            }

        except Exception as e:
            print(f"\n    [ERROR] Failed to fetch chain for {exp}: {e}")

    return chains


def fetch_options_chains(tickers: list[str]) -> dict:
    results = {}

    for symbol in tickers:
        print(f"Fetching option data for {symbol}...")

        try:
            yahoo_symbol, output_symbol, fallback = get_ticker(symbol)

            t = yf.Ticker(yahoo_symbol)

            try:
                expirations = t.options
            except Exception:
                expirations = []

            used_symbol = yahoo_symbol

            # Yahoo index options frequently fail → fallback ETF
            if not expirations and fallback:
                print(
                    f"  [WARN] No chains found for {yahoo_symbol}. "
                    f"Trying fallback ETF {fallback}"
                )

                t = yf.Ticker(fallback)
                expirations = t.options
                used_symbol = fallback

            if not expirations:
                print(
                    f"  [WARN] No options chains found for "
                    f"{symbol}, skipping"
                )
                continue

            spot = get_spot_price(t)

            print(
                f"  Found {len(expirations)} expiration dates "
                f"(source={used_symbol})."
            )

            results[output_symbol] = {
                "underlyingPrice": (
                    round(spot, 4)
                    if spot is not None
                    else None
                ),
                "asOf": date.today().isoformat(),
                "chains": fetch_chain_for_ticker(t, expirations)
            }

            print(
                f"  Successfully loaded all chains "
                f"for {symbol}.{' ' * 20}"
            )

        except Exception as e:
            print(f"  [ERROR] Processing ticker {symbol}: {e}")

    return results


def write_json(results: dict, path: str) -> None:
    if not results:
        print("No data to write.")
        return

    with open(path, "w") as f:
        json.dump(results, f, indent=2)

    total_contracts = sum(
        len(expiry_data["calls"])
        + len(expiry_data["puts"])
        for ticker_data in results.values()
        for expiry_data in ticker_data["chains"].values()
    )

    print(
        f"\nWrote {len(results)} tickers "
        f"({total_contracts} total option contracts) "
        f"to {path}"
    )


def main():
    parser = argparse.ArgumentParser(
        description="Fetch all options chains from Yahoo Finance"
    )

    parser.add_argument(
        "tickers",
        nargs="*",
        help="Ticker symbols or indices"
    )

    parser.add_argument(
        "--out",
        default="options.json",
        help="Output JSON path (default: options.json)"
    )

    args = parser.parse_args()

    tickers = (
        [t.upper() for t in args.tickers]
        if args.tickers
        else DEFAULT_TICKERS
    )

    print(f"Starting bulk options fetch for: {tickers}\n")

    results = fetch_options_chains(tickers)
    write_json(results, args.out)


if __name__ == "__main__":
    main()