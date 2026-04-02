namespace Domain {
    public class Currency : Underlying {
        public required String Name { get; set; }
        public Currency(string code) : base(code) {
        }    
    }
}
