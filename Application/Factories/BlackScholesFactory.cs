using Domain;

namespace Application {
    public static class BlackScholesFactory {

        public static BlackScholes Create(
            VanillaContract contract,
            IMarketData marketData,
            DateTime pricingDate) {

            IUnderlyingMarketData underlyingData = marketData.GetUnderlyingMarketData(contract.Underlying);
            double timeToMaturity = (contract.Maturity - pricingDate).TotalYears;
            double r = marketData.GetDiscounter(contract.Currency).GetForwardRate(pricingDate, contract.Maturity);
            double σ = underlyingData.GetVolatility().getVolatility(underlyingData.GetSpot(), timeToMaturity);
            double b = r - underlyingData.GetDividend() - underlyingData.GetRepo();
            OptionType optionType = contract is ICall ? OptionType.Call : OptionType.Put;

            return new BlackScholes(
                optionType,
                underlyingData.GetSpot(),
                contract.Strike,
                timeToMaturity,
                r,
                σ,
                costOfCarry: b
            );
        }
    }
}
