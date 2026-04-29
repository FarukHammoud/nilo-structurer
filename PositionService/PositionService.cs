using Application;
using Domain;

namespace PositionServices {
    public class PositionService {
        private IDictionary<String, List<VanillaContract>> _portfolioToContracts;
        public PositionService() {
            _portfolioToContracts = new Dictionary<String, List<VanillaContract>>();
            _portfolioToContracts["VANILL"] = new List<VanillaContract>() { 
                new EuropeanCall() {
                    Maturity = DateTime.Today,
                    Strike = 10,
                    Underlying = new Equity("MSFT"),
                    Currency = Currencies.USD
                },
                new EuropeanPut() {
                    Maturity = DateTime.Today,
                    Strike = 10,
                    Underlying = new Equity("MSFT"),
                    Currency = Currencies.USD
                },
                new AmericanCall() {
                    Maturity = DateTime.Today,
                    Strike = 10,
                    Underlying = new Equity("MSFT"),
                    Currency = Currencies.USD
                },
                new AmericanPut() {
                    Maturity = DateTime.Today,
                    Strike = 10,
                    Underlying = new Equity("MSFT"),
                    Currency = Currencies.USD
                },
            };
        }

        public List<VanillaContract>? getContracts(String portfolioName) {
            List<VanillaContract> contracts;
            if (_portfolioToContracts.TryGetValue(portfolioName, out contracts)) {
                return contracts;
            }
            return null;
        }
    }
}
