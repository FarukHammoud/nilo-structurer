namespace Domain {
    public class AmericanExercisableFlow : IExercisableFlow {
        public ExerciseParty ExerciseParty { get; init; }

        public required IPayoff Payoff { get; init; }
        public required DateTime StartDate { get; init; }
        public required DateTime EndDate { get; init; }
        public DateTime Date => Payoff.Date; // payoff is observable at the exercise date, so the flow date is the payoff date
    }
}
