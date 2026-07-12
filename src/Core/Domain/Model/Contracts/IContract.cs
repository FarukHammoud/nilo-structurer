namespace Domain {
    /// <summary>
    /// A contract is a sequence of flows ordered by observation order
    /// </summary>
    public interface IContract {
        double Notional { get; }
        IEnumerable<IFlow> Flows { get; }
        IEnumerable<DateTime> Dates => Flows.SelectMany(f => 
            f is IPayoff payoff ? payoff.ObservationDates 
          : f is IExercisableFlow exercisableFlow ? exercisableFlow.Payoff.ObservationDates.Union([exercisableFlow.Date])
          : Enumerable.Empty<DateTime>())
               .Distinct()
               .Order();
    }
}
