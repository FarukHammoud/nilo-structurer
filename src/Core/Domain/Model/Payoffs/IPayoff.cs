namespace Domain {
    public interface IPayoff {
        IEnumerable<Underlying> Dependencies { get; }
        Currency Currency { get; }
    }
}
