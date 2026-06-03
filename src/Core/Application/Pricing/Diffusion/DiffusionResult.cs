using Domain;
namespace Application {
    public class DiffusionResult {
        public Dictionary<Underlying, Realizations> DiffusionValues { get; set; }

        public Realizations this[Underlying underlying] {
            get => DiffusionValues[underlying];
            set => DiffusionValues[underlying] = value;
        }

        public int NumberOfEvents => DiffusionValues.Values.FirstOrDefault(new Realizations()).Size;
        public DiffusionResult(Dictionary<Underlying, Realizations>? diffusionValues = null) {
            DiffusionValues = diffusionValues ?? new Dictionary<Underlying, Realizations>();
        }
    }
}
