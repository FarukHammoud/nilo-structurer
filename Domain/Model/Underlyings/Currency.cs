namespace Domain {
    public class Currency : Underlying {
        public String Name { get; set; }
        public Currency(string code) : base(code) {
            Name = code;
        }    
    }
}
