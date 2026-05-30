using Domain;

namespace Application {
    public class EuropeanDoubleDigit : SinglePayoffPathIndependentContract {
        public required Underlying FirstUnderlying { get; set; }
        public required Underlying SecondUnderlying { get; set; }
        public double FirstStrike { get; set; }
        public double SecondStrike { get; set; }

        public override IPathIndependentPayoff Payoff => new BiUnderlyingPathIndependentPayoff() { 
            Payoff = (s1, s2) => Notional * (s1 > FirstStrike && s2 > SecondStrike ? 1 : 0),
            FirstUnderlying = FirstUnderlying,
            SecondUnderlying = SecondUnderlying,
            Currency = Currency,
            Maturity = Maturity, 
            PaymentDate = Maturity};
    }
}
