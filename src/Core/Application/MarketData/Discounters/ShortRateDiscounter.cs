using Domain;

namespace Application {
    public class ShortRateDiscounter : IDiscounter {
        private readonly Realizations _realizations;
        private readonly IDiscounter internalDiscounter;
        private ShortRateDiscounter(Realizations realizations, List<DateTime> timeDiscretization) {
            throw new NotImplementedException();
        }

        public double GetDiscountFactor(DateTime date, DateTime today) {
            throw new NotImplementedException();
        }
    }
}
