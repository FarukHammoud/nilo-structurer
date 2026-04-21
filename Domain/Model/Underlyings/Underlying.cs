namespace Domain {
    public abstract record Underlying(String Code) : IUnderlying {
        public abstract IReadOnlyList<Underlying> GetUnderlyingDependencyList();
    }
}
