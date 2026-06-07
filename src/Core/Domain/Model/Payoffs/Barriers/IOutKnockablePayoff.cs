namespace Domain {
    public interface IOutKnockablePayoff {
         IKnockOutBarrier KnockOutCondition { get; }
    }
}
