namespace Domain {
    public record Equity : Underlying {
        public String Name { get; set; }
        public Equity(string code) : base(code) {
            Name = code;
        }

        public override IEnumerable<Underlying> Dependencies => [this];
    }
}
