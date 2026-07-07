namespace Domain {
    public enum ExerciseParty {
        Issuer,
        Holder
    }
    public interface IExercisableFlow : IFlow {
        ExerciseParty ExerciseParty { get; }
        IPayoff Payoff { get; } // it will need to be a contract in the future, observable at the exercise date
    }
}
