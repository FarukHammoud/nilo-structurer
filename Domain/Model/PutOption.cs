namespace Domain {
    public abstract class PutOption : VanillaContract {
        public abstract IPayoff Payoff { get; set; }
    }
}
