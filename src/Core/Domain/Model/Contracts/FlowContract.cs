namespace Domain{
    public class FlowContract : IFlowsContract {
        public double Notional { get; set; } = 1.0;

        public IList<IFlow> Flows { get; set; } = new List<IFlow>();
        public FlowContract AddFlow(IFlow flow) {
            Flows.Add(flow);
            return this;
        } 
    }
}
