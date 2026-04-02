using BrownianServices;

namespace PricerServices {
    public static class BsDiffusion {
        const int N_PATHS = 100;

        public static List<Dictionary<DateTime, Double>> Diffuse(double drift, double volatility, double spot, DateTime startDate, DateTime endDate) {
            BrowniansService browniansService = new();
            List<Double[]> brownian = browniansService.CreateBrownian((int)(endDate - startDate).TotalDays, N_PATHS);
            List<Dictionary<DateTime, Double>> paths = new();
            for (int event_id = 0; event_id < N_PATHS; event_id++) {
                Dictionary<DateTime, Double> path = new();
                path.Add(startDate, spot);
                double[] dW = brownian[event_id];
                for (int date_j = 0; date_j < (endDate - startDate).TotalDays; date_j++) {
                    double dt = 1;
                    double ds = drift * dt + volatility * dW[date_j];
                    spot += ds;
                    path.Add(startDate.AddDays(date_j), ds);
                }
            }
            return paths;
        }
    }
}
