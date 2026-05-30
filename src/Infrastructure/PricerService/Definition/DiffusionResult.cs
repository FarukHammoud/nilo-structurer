using Domain;

namespace PricerServices {
    public class DiffusionResult {
        public required Dictionary<Underlying, Realizations> DiffusionValues { get; set; }
        public int NumberOfEvents => DiffusionValues.Values.ToList().FirstOrDefault(new Realizations() { Paths = new List<double[]>()}).Size;
    }
}
