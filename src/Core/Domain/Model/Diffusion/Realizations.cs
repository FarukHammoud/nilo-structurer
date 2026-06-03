namespace Domain {
    public class Realizations {
        public IReadOnlyList<SimulatedPath> Paths { get; init; }
        public int Size => Paths.Count;
        public SimulatedPath this[int ω] => Paths[ω];

        public Realizations(List<SimulatedPath> paths = null) {
            Paths = paths ?? new List<SimulatedPath>();
        }

        public void AddPath(SimulatedPath path) {
            ((List<SimulatedPath>)Paths).Add(path);
        }
    }
}
