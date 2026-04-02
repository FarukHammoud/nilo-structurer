namespace Domain.Model.Payoffs {
    public class StatelessPayoff : IMonoUnderlyingNonPathDependentPayoff {
        private readonly Func<double, double> _map;
        public StatelessPayoff(Func<double, double> map) {
            _map = map;
        }

        public double GetPayoff(double spotPrice) {
            return _map(spotPrice);
        }
    }
}
