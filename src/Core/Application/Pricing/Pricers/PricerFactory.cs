using Domain;

namespace Application {
    /// <summary>
    /// Factory class to create pricers based on the model configuration.
    /// At term it needs to be replaced by dependency injection on client side.
    /// </summary>
    public class PricerFactory : IPricerFactory {
        public IPricer CreatePricer(ModelConfiguration config)
            => config.Pricing switch {
                MonteCarlo => new DiffusionPricer(),
                PDE => new FiniteDifferencePdeSolver(),
                BinaryTree => new BinaryTreePricer(),
                LongStaffSchwartz => new AmericanPathDependentDiffusionPricer(),
                American => new AmericanPricer(),
                _ => throw new NotSupportedException($"No registered pricer for {config.Pricing}")
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
                American => new DiffusionPricerConfiguration {
                    NumberOfDrawings = request.NumberOfDrawings,
                    Currency = request.PricingCurrency,
                    WithControlVariate = request.WithControlVariate
                },
                _ => throw new NotSupportedException($"No configuration for {request.ModelConfiguration.Pricing}")
            };
    }
}
