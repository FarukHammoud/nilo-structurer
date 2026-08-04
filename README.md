<img width="350" height="60" alt="image" src="https://github.com/user-attachments/assets/3662e9b3-d12a-4a1a-b807-4cf1023eed2f" />

**NiloStructurer** is a **C#/.NET pricing framework for derivatives contracts**. Designed as a modular codebase that lets you define financial products using 3 different flow primitives, set market data and model assumptions in order to price them using one of its several numerical methods.

## Core model

Any contract is a **sequence of `IFlow`**, ordered by observation date:

| Flow | Role |
|------|------|
| **`IPayoff`** | At its observation date, maps a Scenario to a cash value at a certain date. (e.g. European Call)|
| **`IExercisableFlow`** | At its observation date, chooses between **immediate exercise** (an observable payoff) and **continuation** (a conditional expectation given the current path state). (e.g. American Put) |
| **`IAutoCallFlow`** | At its observation date, if a (observable) condition is met, cancels all future payoffs and pays a rebate instead. (e.g. AutoCall) |

Pricers walk this flow sequence backward through simulated paths, applying payoffs, regressing continuation values for exercise decisions, and autocall triggers in inverse date order.

## Contracts

### Equity & FX options
- **Vanilla:** European call / put, American call / put
- **Digital:** binary call / put, double digital
- **Barrier:** up/down, in/out on calls and puts (continuous monitoring)
- **Asian:** arithmetic and geometric averaging calls / puts
- **Forward start:** European call / put
- **Quanto:** European call / put, converse
- **Composite:** European call / put on a foreign asset (FX risk), converse

### Packages & spreads
- Straddle, strangle
- Call / put spreads, call / put calendar spreads

### Rates & linear
- Swap, swaption, zero-coupon bond, bond
- Cash flow(s), forward, future, converse

### Volatility & structured
- Variance swap, corridor variance swap, variance dispersion
- Autocall, product, book

### Contract features
- Fee on premium

## Structured underlyings

Multi-asset payoffs are built on structured underlyings rather than single spots:

- **Best-of** — maximum of two/+ underlyings
- **Worst-of** — minimum of two/+ underlyings
- **Rainbow** — weighted sum ordered by performance
- **Basket** — weighted linear combination

## Features

### Pricing engines
- **Monte Carlo diffusion pricer** — multi-underlying, multi-currency, correlated Brownian motions via Cholesky decomposition; optional control variates;
- **Generalized Longstaff–Schwartz** - Least Squares Regression for continuation value vs observable immediate exercise value;
- **Finite-difference Black–Scholes PDE Solver** - Crank-Nicolson Heat Equation PDE Solver;
- **Cox-Ross-Rubinstein Binomial Tree** - Classical Binomial Tree;
- **Derman-Kani Implied Tree** - Discrete Dupire Local Volatility from Call/Put Prices Implied Tree;

### Models & dynamics
- Local volatility diffusion
- Merton jump-diffusion and Lévy process variations
- Stochastic short rates (Hull–White, Vasicek)
- Multi-currency and quanto payoffs

### Path-dependent simulation
- **American-style exercisability** via `IExercisableFlow`
- **Continuous barrier monitoring** with Brownian-bridge simulation between discretization points
- **Autocall early termination** via `IAutoCallFlow`

### Risk & analytics
- Greeks and other indicators (delta, gamma, cross-gamma, vega, rho, theta, duration, implied volatility, correlation sensitivity) via finite differences on a generic bump interface

## Benchmarks

Tests compare pricer output against closed-form and semi-analytic benchmarks on simple cases:

- Best-of, worst-of, exchange options — Stulz (1982), Margrabe (1978)
- Continuous knock-in / knock-out barriers — Reiner (1992) & Rubinstein (1991) 
- Quanto options — Reiner (1992)
- American puts — Barone–Adesi & Whaley (1987)
- Swaptions — Jamshidian (1989)
- Displaced Diffusion - Rubinstein (1983)

> **Stack:** .NET 10 · C# 14 · MathNet.Numerics · MSTest · Python (market data)

## References
- Merton, R. C. (1976). *Option Pricing when Underlying Stock Returns are Discontinuous.* Journal of Financial Economics, 3(1–2), 125–144.
- Margrabe, W. (1978). *The Value of an Option to Exchange One Asset for Another.* Journal of Finance, 33(1), 177–186.
- Cox, J., Ross, S. & Rubinstein, M. (1979). *Option Pricing: A Simplified Approach.* Journal of Financial Economics.
- Stulz, R. (1982). *Options on the Minimum or the Maximum of Two Risky Assets.* Journal of Financial Economics.
- Rubinstein, M. (1983). *Displaced Diffusion Option Pricing.* Journal of Finance.
- Barone-Adesi, G. & Whaley, R. E. (1987). *Efficient Analytic Approximation of American Option Values.* Journal of Finance, 42(2), 301–320.
- Jamshidian, F. (1989). *An Exact Bond Option Formula.* The Journal of Finance, 44(1), 205–209.
- Rubinstein, M. & Reiner, E. (1991). *Breaking Down the Barriers.* Risk Magazine, 4(8), 28–35.
- Reiner, E. (1992). *Quanto Mechanics.* Risk Magazine, 5(3), 59–63.
- Derman, E. & Kani, I. (1994). *Riding on a Smile.* Risk, 7(2), 32–39.
- Longstaff, F. A. & Schwartz, E. S. (2001). *Valuing American Options by Simulation: A Simple Least-Squares Approach.* Review of Financial Studies, 14(1), 113–147.
- Gatheral, J. (2006). *The Volatility Surface: A Practitioner's Guide.* Wiley Finance.
- Bouzouba, L. (2012). *Exotics and Hybrids: Structured Products and Derivatives Pricing.* Wiley.
- Hull, J. C. (2024). *Options, Futures, and Other Derivatives* (12th ed.). Pearson.
- Schadner, W. (2026). *An Explicit Solution to Black–Scholes Implied Volatility.*
