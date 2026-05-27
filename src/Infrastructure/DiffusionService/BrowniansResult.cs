using Domain;

namespace BrownianServices {
    public class BrowniansResult {
        required public Dictionary<Underlying, List<Double[]>> paths { get; set; }
    }
}
