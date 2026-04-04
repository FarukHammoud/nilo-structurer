namespace Domain {
    public abstract class StructuredUnderlying : Underlying {
        public StructuredUnderlying(string name) : base(name) {
        }

        public abstract double GetValue(Dictionary<Underlying, double> prices);

    }
}
