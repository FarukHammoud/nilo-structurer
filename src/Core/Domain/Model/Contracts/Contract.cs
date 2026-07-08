namespace Domain {
    public class Contract : IContract {
        public double Notional { get; set; } = 1.0;

        public List<IFlow> Flows { get; set; } = new List<IFlow>();

        IEnumerable<IFlow> IContract.Flows => Flows;

        public IContract AddFlow(IFlow flow) {
            Flows.Add(flow);
            return this;
        }
    }
}
