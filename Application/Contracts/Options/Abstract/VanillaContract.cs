using Domain;

namespace Application {
    public abstract class VanillaContract : NonPathDependentContract {
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
    }
}
