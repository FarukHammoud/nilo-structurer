namespace Application {
    public interface IPricer<T> {
        double Price(T contract);
    }
}