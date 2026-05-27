using Domain;

namespace Application {
    public class PriceRequest {
        public required List<String> Indicators { get; set; }
        public required List<IContract> Contracts { get; set; }
    }
}
