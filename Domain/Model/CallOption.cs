namespace Domain {
    public abstract class CallOption : VanillaContract {
        public abstract IPayoff Payoff { get; set; }
    }
}
