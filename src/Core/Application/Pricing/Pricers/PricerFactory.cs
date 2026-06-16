using Domain;

namespace Application {
    public class PricerFactory : IPricerFactory {
        public IPricer CreatePricer(ModelConfiguration config)
            => config.Pricing switch {
                MonteCarlo => new DiffusionPricer(),
                PDE => new FiniteDifferencePdeSolver(),
                BinaryTree => new BinaryTreePricer(),
                LongStaffSchwartz => new AmericanPathDependentDiffusionPricer(),
                _ => throw new NotSupportedException($"No path-independent pricer for {config.Pricing}")
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
