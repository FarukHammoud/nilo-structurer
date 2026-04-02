namespace Domain {
    public class MultiRealizations {
        public required List<Dictionary<Underlying, double[]>> PathsByUnderlying { get; set; }
        public int Size => PathsByUnderlying.Count;
    }
}
