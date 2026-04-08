# Nilo Structurer

Small pricing and diffusion simulation framework with a demo vanilla pricer UI.

This repository contains .NET pricing engines, diffusion and Brownian generators, market data models, pricing contracts and unit tests. A small React client and a Blazor app are included for demonstration.

## Key technologies
- .NET 10 (SDK required)
- C# 14
- MathNet.Numerics for statistics / linear algebra
- MSTest for unit tests
- React demo client (VanillaPricer/ClientApp)
- Blazor demo app (Presentation.BlazorWebApp)

## Repository layout (high level)
- `PricerService` - pricing engines, diffusion logic, Black–Scholes formulas
- `DiffusionService` / `BrownianServices` - Brownian path generation and correlation utilities
- `Domain` - market data models, contracts, payoffs, volatilities
- `PricingServicesTests` / `DiffusionService.Tests` - unit tests
- `VanillaPricer/ClientApp` - React front-end (Create React App)
- `Presentation.BlazorWebApp` - Blazor demonstration app
- `Server` - API hosting / glue

## Quickstart

Prerequisites
- .NET 10 SDK
- Node.js and npm (for React client)
- Recommended IDE: Visual Studio 2026 

Build all .NET projects: