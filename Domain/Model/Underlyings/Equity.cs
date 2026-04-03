namespace Domain {
    public class Equity : Underlying {
        public String Name { get; set; }
        public Equity(string code) : base(code) {
            Name = code;
        }    
    }
}
