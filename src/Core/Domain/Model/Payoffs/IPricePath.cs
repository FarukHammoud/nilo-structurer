namespace Domain {
    /// <summary>
    /// Represents the price path fed to a path-dependent payoff:
    /// observation date → underlying → value.
    /// </summary>
    public interface IPricePath {
        IReadOnlySet<DateTime> ObservationDates { get; }
        double GetValue(DateTime date, Underlying underlying);
    }
}
