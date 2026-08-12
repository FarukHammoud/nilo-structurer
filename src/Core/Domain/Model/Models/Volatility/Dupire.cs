namespace Domain {

    /// <summary>
    /// Local volatility from Implied volatility using Dupire's formula.
    /// Gatheral, Jim. The Volatility Surface: A Practitioner's Guide. John Wiley & Sons, 2006.
    /// 1.10 
    /// </summary>
    public class Dupire {
        private readonly IImpliedVolatilityModel _impliedVolatility;
        private readonly IDiscounter _discounter;
        private readonly IDayCountConvention _dayCount = new Actual365();
        private readonly double _dk;   // absolute bump in log-moneyness
        private readonly double _dT;   // absolute time bump
        private readonly double _varianceFloor;

        public Dupire(
            IImpliedVolatilityModel impliedVolatility,
            IDiscounter discounter,
            double logMoneynessBump = 1e-2,
            double timeBump = 1e-3,
            double varianceFloor = 1e-6) {
            _impliedVolatility = impliedVolatility;
            _discounter = discounter;
            _dk = logMoneynessBump;
            _dT = timeBump * 365.0;
            _varianceFloor = varianceFloor;
        }

        public double GetLocalVolatility(double spot, DateTime date, double referenceSpot, DateTime referenceTime) {
            double forward = referenceSpot / _discounter.GetDiscountFactor(referenceTime, date);
            double k = Math.Log(spot / forward);

            DateTime dateUp = date.AddDays(_dT);
            DateTime dateDown = date.AddDays(-_dT);
            double tUp = _dayCount.YearFraction(referenceTime, dateUp);
            double tDown = Math.Max(_dayCount.YearFraction(referenceTime, dateDown), 1e-6);

            double w = TotalVariance(k, date, referenceSpot, referenceTime);
            double wKUp = TotalVariance(k + _dk, date, referenceSpot, referenceTime);
            double wKDown = TotalVariance(k - _dk, date, referenceSpot, referenceTime);
            double wTUp = TotalVariance(k, dateUp, referenceSpot, referenceTime);
            double wTDown = TotalVariance(k, dateDown, referenceSpot, referenceTime);

            double dwdT = (wTUp - wTDown) / (tUp - tDown);
            double dwdk = (wKUp - wKDown) / (2.0 * _dk);
            double d2wdk2 = (wKUp - 2.0 * w + wKDown) / (_dk * _dk);

            if (w < 1e-12) {
                return Math.Sqrt(_varianceFloor);
            }

            double term1 = 1.0 - (k / w) * dwdk;
            double term2 = 0.25 * (-0.25 - 1.0 / w + (k * k) / (w * w)) * dwdk * dwdk;
            double term3 = 0.5 * d2wdk2;

            double denominator = term1 + term2 + term3;

            // Denominator <= 0 or dwdT < 0 signals local butterfly/calendar
            // arbitrage in the surface at this point, not a bug in the formula.
            if (denominator < 1e-8 || dwdT < 0.0) {
                return Math.Sqrt(_varianceFloor);
            }

            double localVariance = dwdT / denominator;
            localVariance = Math.Max(localVariance, _varianceFloor);

            return Math.Sqrt(localVariance);
        }

        private double TotalVariance(double logMoneyness, DateTime date, double referenceSpot, DateTime referenceTime) {
            double timeToMaturity = _dayCount.YearFraction(referenceTime, date);
            double forward = referenceSpot / _discounter.GetDiscountFactor(referenceTime, date);
            double strike = forward * Math.Exp(logMoneyness);
            double volatility = _impliedVolatility.GetVolatility(strike, date);
            return volatility * volatility * timeToMaturity;
        }
    }
}
