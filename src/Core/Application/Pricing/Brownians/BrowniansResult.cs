using Domain;

namespace Application {
    public class BrowniansResult {
        required public Dictionary<Underlying, Realizations> Paths { get; set; }
    }
}
