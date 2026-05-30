namespace Domain {
    public interface IPayoff {
        IEnumerable<Underlying> Dependencies { get; }
        DateTime PaymentDate { get; }
        Currency Currency { get; }
    }
}
