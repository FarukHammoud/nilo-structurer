using Domain;

namespace Application {
    public interface IPathIndependentPricer : IPayoffPricer<IPathIndependentPayoff>;
}