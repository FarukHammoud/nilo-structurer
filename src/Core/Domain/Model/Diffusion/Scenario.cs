namespace Domain {
    // Underlying -> SimulatedPath
    public class Scenario {
        public Dictionary<Underlying, SimulatedPath> Values { get; } = new();
        public SimulatedPath this[Underlying underlying] {
            get => Values[underlying];
            set => Values[underlying] = value;
        }
    }
}
