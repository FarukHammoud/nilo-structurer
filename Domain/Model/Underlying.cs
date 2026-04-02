namespace Domain {
    public class Underlying {
        public string Code { get; set; }

        public Underlying(string code) {
            Code = code;
        }

        public override bool Equals(object? obj) {
            return obj is Underlying underlying &&
                   Code == underlying.Code;
        }

        public override int GetHashCode() {
            return Code.GetHashCode();
        }
    }
}
