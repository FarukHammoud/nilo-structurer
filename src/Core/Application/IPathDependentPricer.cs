using Domain;

namespace Application {
    public interface IPathDependentPricer : IPayoffPricer<IPathDependentPayoff>;
}