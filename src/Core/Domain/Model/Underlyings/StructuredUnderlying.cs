namespace Domain {
    public abstract record StructuredUnderlying : Underlying {
        public StructuredUnderlying(string name) : base(name) {
        }

        public abstract double GetValue(Dictionary<Underlying, double> prices);

    }
}
