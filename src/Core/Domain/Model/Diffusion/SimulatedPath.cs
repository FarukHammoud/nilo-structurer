namespace Domain {
    public readonly struct SimulatedPath {
        public double[] Values { get; }
        public int Length => Values.Length;
        public double Last => this[Length - 1];
        public double this[int step] {
            get => Values[step];
            set => Values[step] = value;
        }

        public SimulatedPath(double[] values) {
            Values = values;
        }

        public SimulatedPath(int length) {
            Values = new double[length];
        }
    }
}
