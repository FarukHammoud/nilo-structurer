namespace Domain {
    // TODO : This is an intermediary class until we have a contract with
    // A list of both path dependent and independent payoffs
    public interface IGeneralContract<T> : IContract where T : IPayoff {
        IEnumerable<T> Payoffs { get; }
    }
}
