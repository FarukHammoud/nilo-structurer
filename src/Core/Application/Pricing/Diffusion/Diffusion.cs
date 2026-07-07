using Domain;
using System.Collections;

namespace Application {
    public class Diffusion : IEnumerable<Dictionary<Underlying, SimulatedPath>> {
        private Dictionary<Underlying, Realizations> _diffusionValues = new();
        private Dictionary<DateTime, Dictionary<Underlying, List<double>>> _byDate = new();
        private List<Dictionary<DateTime, Dictionary<Underlying, double>>> _scenarios = new();

        public Diffusion(IList<DateTime> dates) {
            Dates = dates;
            foreach (DateTime date in dates) {
                _byDate[date] = new();
            }
        }

        public IEnumerable<Underlying> Underlyings => _diffusionValues.Keys;
        public IList<DateTime> Dates { get; init; }

        public Realizations this[Underlying underlying] {
            get => _diffusionValues[underlying];
            set { 
                _diffusionValues[underlying] = value;
                for (int i = 0; i < Dates.Count; i++) {
                    _byDate[Dates[i]][underlying] = value.GetTranche(i);
                }
            }
        }

        public Dictionary<Underlying, List<double>> this[DateTime date] {
            get => _byDate[date];
        }

        public Dictionary<Underlying, SimulatedPath> this[int ω] {
            get => Underlyings.ToDictionary(udl => udl, udl => _diffusionValues[udl][ω]);
        }

        public Dictionary<DateTime, Dictionary<Underlying, List<double>>> ByDate() {
            return _byDate;
        }
        public Dictionary<DateTime, Dictionary<Underlying, double>> Scenario(int ω) {
            return _byDate.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.ToDictionary(
                        udlAndPath => udlAndPath.Key,
                        udlAndPath => udlAndPath.Value[ω])
                    );
        }

        public Dictionary<Underlying, List<double>> Lasts() {
            return _byDate[Dates.Last()];
        }

        public Dictionary<Underlying, double> Spots() {
            return _byDate[Dates.First()].ToDictionary(
                pair => pair.Key, 
                pair => pair.Value.Average()
                ); 
        }

        public IEnumerator<Dictionary<Underlying, SimulatedPath>> GetEnumerator() {
            for (int ω = 0; ω < NumberOfEvents; ω++) {
                yield return this[ω];
            }
        }

        public List<Dictionary<DateTime, Dictionary<Underlying, double>>> Scenarios() {
            return Enumerable.Range(0, NumberOfEvents).Select(Scenario).ToList();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int NumberOfEvents => _diffusionValues.Values.Any() ? _diffusionValues.Values.First().Size : 0;
    }
}
