namespace Domain {
    /// <summary>
    /// Represents a specific scenario. i.e one realization of all underlying involved.
    /// </summary>
    public class Scenario {

        public Dictionary<Underlying, SimulatedPath> Values { get; } = new();
        public IEnumerable<Underlying> Underlyings => Values.Keys;
        public List<DateTime> Dates { get; init; } = new();
        private Dictionary<DateTime, int> _dateIndex = new();
        public SimulatedPath this[Underlying underlying] {
            get => Values[underlying];
            set => Values[underlying] = value;
        }

        public Dictionary<Underlying, double> this[DateTime date] {
            get {
                int dateIndex = _dateIndex[date];
                return Values.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.Values[dateIndex]
                );
            }
        }

        public Scenario(Dictionary<DateTime, Dictionary<Underlying, double>> prices) {
            Dates = prices.Keys.Order().ToList();
            List<Underlying> underlyings = prices.Values.First().Keys.ToList();
            foreach (var underlying in underlyings) {
                Values[underlying] = new SimulatedPath(Dates.Select(date => prices[date][underlying]).ToArray());
            }
            BuildDateIndex();
        }

        public Scenario(DateTime date, Underlying underlying, double value) {
            Dates.Add(date);
            Values[underlying] = new SimulatedPath([value]);
            BuildDateIndex();
        }

        private void BuildDateIndex() {
            _dateIndex = Dates.Select((date, index) => new { date, index })
                .ToDictionary(x => x.date, x => x.index);
        }
    }
}
