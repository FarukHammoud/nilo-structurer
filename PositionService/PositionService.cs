using Domain;

namespace PositionServices {
    public class PositionService {
        private IDictionary<String, List<VanillaContract>> _portfolioToContracts;
        public PositionService() {
            _portfolioToContracts = new Dictionary<String, List<VanillaContract>>();
            _portfolioToContracts["VANILL"] = new List<VanillaContract>() { 
                new EuropeanCallOption() {
                    Maturity = DateTime.Today,
                    Strike = 10,
                    Underlying = new Underlying("MSFT")
                },
                new EuropeanPutOption() {
                    Maturity = DateTime.Today,
                    Strike = 10,
                    Underlying = new Underlying("MSFT")
                },
                new AmericanCallOption() {
                    Maturity = DateTime.Today,
                    Strike = 10,
                    Underlying = new Underlying("MSFT")
                },
                new AmericanPutOption() {
                    Maturity = DateTime.Today,
                    Strike = 10,
                    Underlying = new Underlying("MSFT")
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
