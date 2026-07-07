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

        public List<double> GetTranche(int position) {
            return Paths.Select(path => path[position]).ToList();
        }

        public SimulatedPath Average() {             
            if (Paths.Count == 0) {
                throw new InvalidOperationException("Cannot compute average of empty realizations.");
            }
            int pathLength = Paths[0].Length;
            double[] averageValues = new double[pathLength];
            foreach (var path in Paths) {
                for (int i = 0; i < pathLength; i++) {
                    averageValues[i] += path[i];
                }
            }
            for (int i = 0; i < pathLength; i++) {
                averageValues[i] /= Paths.Count;
            }
            return new SimulatedPath(averageValues);
        }
    }
}
