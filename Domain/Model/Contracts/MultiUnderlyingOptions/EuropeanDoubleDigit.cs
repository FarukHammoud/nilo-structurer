namespace Domain {
    public class EuropeanDoubleDigit : NonPathDependentContract {
        public required Underlying FirstUnderlying { get; set; }
        public required Underlying SecondUnderlying { get; set; }
        public double FirstStrike { get; set; }
        public double SecondStrike { get; set; }

        public override INonPathDependentPayoff Payoff => new BiUnderlyingNonPathDependentPayoff(
            (spot1, spot2) => Notional * (spot1 > FirstStrike && spot2 > SecondStrike ? 1 : 0), 
            FirstUnderlying, 
            SecondUnderlying);
    }
}
