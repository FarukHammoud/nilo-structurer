namespace Domain {
    public class AutoCallFlow : IAutoCallFlow {
        public required DateTime Date { get; set; }

        public required IPayoff Rebate { get; set; }

        public required Func<Scenario, bool> TriggerMap { get; set; }

        public bool IsTriggered(Scenario scenario) {
            return TriggerMap(scenario);
        }
    }
}
