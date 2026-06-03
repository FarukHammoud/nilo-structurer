using Domain;

namespace Application {
    public class BrowniansResult {
        required public Dictionary<Underlying, List<Double[]>> paths { get; set; }
    }
}
