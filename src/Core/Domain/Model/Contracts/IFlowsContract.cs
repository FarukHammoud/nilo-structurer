namespace Domain {
    /// <summary>
    /// Target IContract
    /// </summary>
    public interface IFlowsContract {
        double Notional { get; }
        IList<IFlow> Flows { get; }
        IList<DateTime> Dates => Flows.Select(f => f.Date).Distinct().Order().ToList();
    }
}
