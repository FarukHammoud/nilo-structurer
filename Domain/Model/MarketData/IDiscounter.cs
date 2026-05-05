namespace Domain{
    public interface IDiscounter {
        double GetDiscountFactor(DateTime date, DateTime today);
        double GetForwardRate(DateTime from, DateTime to) {
            double dt = (to - from).TotalYears;
            return Math.Log(GetDiscountFactor(from, to)) / dt;
        }
    }
}
