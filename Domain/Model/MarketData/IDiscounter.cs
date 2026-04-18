namespace Domain{
    public interface IDiscounter {
        double GetDiscountFactor(DateTime date, DateTime today);
    }
}
