using Domain;

namespace Application {
    public class CashFlow {
        public required DateTime PaymentDate { get; init; }
        public required double Amount { get; init; }
        public required Currency Currency { get; init; }
    }
}
