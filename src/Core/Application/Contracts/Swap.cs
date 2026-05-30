using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts {
    public class Swap : IPathIndependentContract{
        private IEnumerable<Tuple<DateTime, double>> _cashFlows;

        public Swap(Underlying FloatingRate, IEnumerable<Tuple<DateTime, double>> cashFlows) {
            _cashFlows = cashFlows;
        }

        public IEnumerable<DateTime> Dates => _cashFlows.Select(e => e.Item1);
        public IEnumerable<Double> Values => _cashFlows.Select(e => e.Item2);
        public required Currency Currency { get; set; } // ignored for the moment
        public IEnumerable<Tuple<DateTime, IPathIndependentPayoff>> Payoffs =>
            _cashFlows.Select(e => Tuple.Create(e.Item1, (IPathIndependentPayoff)new DeterministicPayoff(e.Item2, Currency)));
        public double Notional { get; set; } = 1.0;
    }
}
