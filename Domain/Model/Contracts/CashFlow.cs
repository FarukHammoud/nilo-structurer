namespace Domain {
    public class CashFlow : Contract {
        public required List<DateTime> Dates { get; set; }
        public required List<Double> Values { get; set; }
        public required Currency Currency { get; set; }
    }
}
