<img width="350" height="60" alt="image" src="https://github.com/user-attachments/assets/3662e9b3-d12a-4a1a-b807-4cf1023eed2f" />



NiloStructurer is a **C#/.NET pricing framework for quantitative finance**. It has been designed as a modular code base that lets you choose between several product modeling design choices, as well as price with different algorithms.

The base code includes a generic local volatility diffusion pricer, that handles multi-underlying/currency structures/payoffs, that can also include merton's poisson jumps (or any type of lévy's process variations), useful to price complex payoffs and structures. It generates deterministic correlated brownian motions via Cholesky decomposition and has support to control variates for a higher precision result.

It also includes a Finite Difference's Black-Scholes PDE solver, used to price American Options, and other benchmark algorithms like classical binary-trees.

Most tests compare pricer vs benchmark models on simple cases using a range of known models:
* For Best-of, Worst-of, Exchange options : Stulz, Margrabe
* For continuous knock-in, knock-out barrier : Reiner & Rubinstein
* For quanto options : Reiner
* For American Puts Aproximation: Barone-Adesi Whaley
* For Swaptions : Jamshidian

On-going path-dependent pricer project: Asian Options, Autocalls, etcs.

We implement several indicators (greeks but not only) via finite differences, using a generic interface that enables us to defined the needed bumped market data/pricing date, and how to get a value from the bumped results.
 
> **Stack:** .NET 10 · C# 14 · MathNet.Numerics · MSTest
 
## References
- Merton, R. C. (1976). Option Pricing when Underlying Stock Returns are Discontinuous. Journal of Financial Economics, 3(1–2), 125–144.
- Cox, J., Ross, S. & Rubinstein, M. (1979). *Option Pricing: A Simplified Approach.* Journal of Financial Economics.
- Stulz, R. (1982). *Options on the Minimum or the Maximum of Two Risky Assets.* Journal of Financial Economics.
- Jamshidian, F. (1989). "An Exact Bond Option Formula." The Journal of Finance, 44(1), 205–209.
- Rubinstein, M. & Reiner, E. (1991). Breaking Down the Barriers. Risk Magazine, 4(8), 28–35.
- Reiner, E. (1992). Quanto Mechanics. Risk Magazine, 5(3), 59–63.
- Derman, E. & Kani, I. (1994). *Riding on a Smile.* Risk, 7(2), 32–39.
- Longstaff, F. A. & Schwartz, E. S. (2001). Valuing American Options by Simulation: A Simple Least-Squares Approach. Review of Financial Studies, 14(1), 113–147.
- Schadner, W. (2026). An Explicit Solution to Black–Scholes Implied Volatility.
  
