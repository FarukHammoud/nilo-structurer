namespace Domain {
    /// <summary>
    /// A flow represents an potential exchange between the contract holder and the issuer.
    /// </summary>
    public interface IFlow {
        DateTime Date { get; }
    }
}
