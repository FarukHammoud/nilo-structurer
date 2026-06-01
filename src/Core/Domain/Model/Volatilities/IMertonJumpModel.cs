namespace Domain {
    public interface IMertonJumpModel : IVolatility {
        public JumpParameters JumpParameters { get; }
    }
}
