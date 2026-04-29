namespace Domain {
    public abstract record Underlying(String Code) : IUnderlying {
        public abstract IEnumerable<Underlying> Dependencies { get; }
    }
}
