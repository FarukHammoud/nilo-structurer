using Domain;

namespace Application {
    public class PriceRequest {
        public required List<String> Indicators { get; set; }
        public required List<Contract> Contracts { get; set; }
    }
}
