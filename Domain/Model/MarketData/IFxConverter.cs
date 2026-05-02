namespace Domain{
    public interface IFxConverter {
        double GetFxRate(Currency from, Currency to);
    }
}
