using Domain;
namespace Application {
    public class DiffusionResult {
        public required Dictionary<Underlying, Realizations> DiffusionValues { get; set; }
        public int NumberOfEvents => DiffusionValues.Values.FirstOrDefault(new Realizations()).Size;
    }
}
