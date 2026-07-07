namespace Domain {
    public class ExercisableFlow : IExercisableFlow {
        public ExerciseParty ExerciseParty { get; init; }

        public required IPayoff Payoff { get; init; }
        public required DateTime Date { get; set; } 
    }
}
