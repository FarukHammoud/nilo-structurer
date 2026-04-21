namespace Application {
    public record PricingProgress(int PathsCompleted, int TotalPaths, double RunningEstimate, double RunningStdError);
}
