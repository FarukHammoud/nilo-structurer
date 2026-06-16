namespace Domain {
    public interface IContract {
        double Notional { get; }
        IEnumerable<IPayoff> Payoffs { get; }
        IEnumerable<DateTime> Dates { get; }
    }
}
