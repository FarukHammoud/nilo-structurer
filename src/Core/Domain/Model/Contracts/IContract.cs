namespace Domain {
    public interface IContract {
        double Notional { get; }
        IEnumerable<DateTime> Dates { get; }
    }
}
