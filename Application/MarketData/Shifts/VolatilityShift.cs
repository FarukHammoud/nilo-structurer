using Domain;

namespace Application {
    public record VolatilityShift(double Bump) : UnderlyingShift;
}
