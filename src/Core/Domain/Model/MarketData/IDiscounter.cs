namespace Domain{
    public interface IDiscounter {
        IDayCountConvention DayCounter { get; } 
        double GetDiscountFactor(DateTime from, DateTime to);
        double GetForwardRate(DateTime from, DateTime to) {
            double dt = DayCounter.YearFraction(from, to);
            return -Math.Log(GetDiscountFactor(from, to)) / dt;
        }
    }
}
