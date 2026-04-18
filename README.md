# NiloStructurer (work in progress)
 
A **C#/.NET pricing and diffusion framework** for exotic structured products, built from first principles. Covers the full stack from mathematical models to a live web UI — Black-Scholes, binary trees, Brownian motion, interest rate curves, and multi-asset payoffs.
 
> **Stack:** .NET 10 · C# 14 · MathNet.Numerics · Blazor · React · MSTest
 
---
 
## What it prices
 
| Payoff |
|--------|
| Vanillas / Combinations |
| Digital Options |
| Asian Options |
| Best-of / Worst-of (2 assets) |
| Variance Swaps |
| Corridor Variance Swaps |
| Generic non path-dependent payoffs |
 
---
 
## Repository layout
 
```
nilo-structurer/
│
├── Domain/                      # Core types: Underlying, ValueWithPrecision,
│                                #   payoff interfaces, vol surface contracts
│
├── Application/                 # Service interfaces: IPricer, IDiscounter,
│                                #   IMarketData, IPayoff, IVolatilitySurface
│
├── PricerService/               # Pricing engines
│   ├── BlackScholesPricer       #   Closed-form BS for vanillas and digitals
│   └── BinaryTreePricer         #   CRR recombining tree, discounter-native
│
├── DiffusionService/            # Brownian path generation, correlated paths
│                                #   via Cholesky decomposition
│
├── FixedIncomeServices/         # Zero coupon curve bootstrapping from swaps,
│                                #   OIS / EURIBOR multi-curve framework
│
├── MarketDataService/           # Spot, volatility surface, market data assembly
│
├── PositionService/             # Position and book management
│
├── Server/                      # ASP.NET Core API — hosts all pricing services
│
├── Presentation.BlazorWebApp/   # Blazor front-end for interactive pricing
│
├── VanillaPricer/               # React demo client (Create React App)
│   └── ClientApp/
│
├── PythonClient/                # Python client for validation and scripting
│
├── PricingServicesTests/        # Pricing engine unit tests (MSTest)
└── DiffusionService.Tests/      # Diffusion / path generation tests
```
 
---
 
## Architecture
 
### Pricing pipeline
 
```
IMarketData ──► IPricer.Initialize()  ──► tree / BS model built once
                                              │
IPayoff ──────► IPricer.Price()       ──► ValueWithPrecision
IDiscounter ──►
```
 
Pricers are **stateful in market data, stateless in payoffs.** The tree or closed-form model is built once on `Initialize(IMarketData, List<DateTime>)`. Any number of payoffs can then be priced against the same initialized structure without rebuilding.
 
### Key design decisions
 
**`IPayoff` as composition.** Payoffs declare their underlying dependency list and implement `GetPayoffAtMaturity`. Pricers are fully agnostic to the product — the same `BinaryTreePricer` prices a call, a digital, or any custom payoff.
 
**Model config separated from market data.** Discounting curve, vol surface, and rate conventions are configured at initialization. Spot and vol are injected at pricing time through `IMarketData`.
 
**Discounter-native risk-neutral probabilities.** The CRR up probability uses `1 / discounter.GetDiscountFactor(t_next, t)` as the forward factor — no flat rate approximation, consistent with any curve shape.
 
---
 
## Binary Tree Pricer
 
A **CRR recombining binomial tree** with O(n²) node count.
 
**Recombination** is enforced via a `Dictionary<(int step, int upMoves), TreeNode>` construction cache — nodes that mathematically coincide are the same object in memory. The value cache uses `Dictionary<TreeNode, double>` (reference equality), which is both correct and allocation-efficient.
 
```
step 0       step 1       step 2
             S·u²
S·u
S                         S        ← same node (up then down = down then up)
S·d
             S·d²
```
 
**Per-call value cache.** `GetValue` receives a fresh `Dictionary<TreeNode, double>` per pricing call — the same tree prices multiple payoffs without cache pollution.
 
**Richardson extrapolation** for precision estimation at O(1/n²) convergence:
 
```csharp
double p1 = PriceWithSteps(n);
double p2 = PriceWithSteps(2 * n);
double extrapolated = 2 * p2 - p1;
double precision    = Math.Abs(p2 - p1);
```
 
---
 
## Diffusion & Monte Carlo
 
`DiffusionService` generates correlated Brownian paths using **Cholesky decomposition** of the covariance matrix. This is the natural tool for multi-underlying payoffs (best-of, worst-of, basket) where the binary tree's quadranomial generalization breaks down under strong correlation.
 
Paths are validated against analytical moments and cross-checked against closed-form prices where available (e.g. Stulz for two-asset options).
 
---
 
## Interest Rate Curves
 
Zero coupon curve bootstrapped from EUR swap quotes using iterative stripping:
 
```
P(0, tᵢ) solved sequentially from short to long tenors
```
 
Forward discount factors follow:
 
```
P(i, j) = P(0, j) / P(0, i)
```

 
---
 
## Interfaces
 
### `Presentation.BlazorWebApp`
Interactive Blazor UI for pricing vanillas and exotics against live market data parameters.
 
### `VanillaPricer` (React)
Lightweight React demo client for vanilla option pricing with real-time parameter updates.
 
### `PythonClient`
Python validation client used to cross-check C# pricing results against scipy / analytical benchmarks — particularly for bivariate normal computations (Stulz formula) and Monte Carlo convergence.
 
---
 
## Quickstart
 
**Prerequisites:** .NET 10 SDK, Node.js + npm (for React client)
 
```bash
git clone https://github.com/FarukHammoud/nilo-structurer.git
cd nilo-structurer
 
# Build all .NET projects
dotnet build NiloStructurer.sln
 
# Run tests
dotnet test
 
# Run the API server
dotnet run --project Server
 
# Run the Blazor app
dotnet run --project Presentation.BlazorWebApp
 
# Run the React client
cd VanillaPricer/ClientApp
npm install && npm start
```
 
---
 
## Dependencies
 
- [MathNet.Numerics](https://numerics.mathdotnet.com/) — special functions, bivariate normal, linear algebra
- .NET 10 / C# 14
- MSTest — unit testing
- React (Create React App) — vanilla pricer demo
- Blazor — interactive pricing UI
---
 
## References
 
- Black, F. & Scholes, M. (1973). *The Pricing of Options and Corporate Liabilities.* Journal of Political Economy.
- Cox, J., Ross, S. & Rubinstein, M. (1979). *Option Pricing: A Simplified Approach.* Journal of Financial Economics.
- Stulz, R. (1982). *Options on the Minimum or the Maximum of Two Risky Assets.* Journal of Financial Economics.
- Derman, E. & Kani, I. (1994). *Riding on a Smile.* Risk, 7(2), 32–39.
- Dupire, B. (1994). *Pricing with a Smile.* Risk, 7(1), 18–20.
