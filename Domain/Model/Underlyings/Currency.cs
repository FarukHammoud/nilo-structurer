namespace Domain {
    public record Currency : Underlying {
        public String Name { get; set; }
        public Currency(string code) : base(code) {
            Name = code;
        }

        public override IEnumerable<Underlying> Dependencies => [this];
    }
}
