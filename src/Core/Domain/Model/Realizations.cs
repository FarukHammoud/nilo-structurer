namespace Domain {
    public class Realizations {
        public required List<Double[]> Paths { get; set; }
        public int Size => Paths.Count;
    }
}
