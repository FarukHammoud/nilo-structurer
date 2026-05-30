namespace Domain {
    public interface IKnockOutBarrierContract {
        IReadOnlyList<IKnockOutBarrier> KnockOutBarriers { get; }
    }
}
