using Domain;
namespace Application {
    public static class Currencies {
        public static readonly Currency EUR = new("EUR") { Name = "Euro" };
        public static readonly Currency USD = new("USD") { Name = "US Dollar" };
        public static readonly Currency GBP = new("GBP") { Name = "British Pound" };
        public static readonly Currency JPY = new("JPY") { Name = "Japanese Yen" };
        public static readonly Currency CHF = new("CHF") { Name = "Swiss Franc" };
        public static readonly Currency CNH = new("CNH") { Name = "Chinese Yuan" };
        public static readonly Currency BRL = new("BRL") { Name = "Brazilian Real" };
        public static readonly Currency CAD = new("CAD") { Name = "Canadian Dollar" };
        public static readonly Currency ZAR = new("ZAR") { Name = "South African Rand" };
    }
}
