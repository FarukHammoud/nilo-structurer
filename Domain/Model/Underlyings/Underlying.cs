namespace Domain {
    public abstract class Underlying {
        public string Code { get; set; }

        public Underlying(string code) {
            Code = code;
        }

        public abstract List<Underlying> GetUnderlyingDependencyList();

        public override bool Equals(object? obj) {
            return obj is Underlying underlying &&
                   Code == underlying.Code;
        }

        public override int GetHashCode() {
            return Code.GetHashCode();
        }

       
    }
}
