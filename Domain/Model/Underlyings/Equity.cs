namespace Domain {
    public record Equity : Underlying {
        public String Name { get; set; }
        public Equity(string code) : base(code) {
            Name = code;
        }

        public override List<Underlying> GetUnderlyingDependencyList() {
            return new List<Underlying>() { this };
        }
    }
}
