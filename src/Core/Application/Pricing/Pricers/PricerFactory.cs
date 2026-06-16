using Domain;

namespace Application {
    public class PricerFactory : IPricerFactory {
        public IPricer<IPathIndependentPayoff> CreatePathIndependentPricer(ModelConfiguration config)
            => config.Pricing switch {
                MonteCarlo => new PathIndependentDiffusionPricer(),
                PDE => new FiniteDifferencePdeSolver(),
                BinaryTree => new BinaryTreePricer(),
                _ => throw new NotSupportedException($"No path-independent pricer for {config.Pricing}")
            };

        public IPricer<IPathDependentPayoff> CreatePathDependentPricer(ModelConfiguration config)
            => config.Pricing switch {
                MonteCarlo => new PathDependentDiffusionPricer(),
                PDE => new FiniteDifferencePdeSolver(),
                LongStaffSchwartz => new AmericanPathDependentDiffusionPricer(),
                _ => throw new NotSupportedException($"No path-dependent pricer for {config.Pricing}")
            };

        public IPricerConfiguration CreateConfiguration(PricingRequest request)
            => request.ModelConfiguration.Pricing switch {
                MonteCarlo => new DiffusionPricerConfiguration {
                    NumberOfDrawings = request.NumberOfDrawings,
                    Currency = request.PricingCurrency,
                    WithControlVariate = request.WithControlVariate
                },
                BinaryTree => new BinaryTreePricerConfiguration(),
                LongStaffSchwartz => new DiffusionPricerConfiguration {
                    NumberOfDrawings = request.NumberOfDrawings,
                    Currency = request.PricingCurrency,
                    WithControlVariate = request.WithControlVariate
                },
                _ => throw new NotSupportedException($"No configuration for {request.ModelConfiguration.Pricing}")
            };
    }
}
