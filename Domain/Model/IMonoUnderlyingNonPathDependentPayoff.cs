namespace Domain.Model {
    public interface IMonoUnderlyingNonPathDependentPayoff : IPayoff {
        double GetPayoff(double spotPrice);}
}
