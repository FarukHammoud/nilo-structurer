using Domain;

namespace Application {
    public class SchemeProvider : ISchemeProvider {
        public INumericalScheme GetScheme(IProcessDynamics dynamics) {
            return dynamics switch {
                LevyProcessDynamics 
                    => new LogEulerScheme(),
                VasicekDynamics or CoxIngersollRossDynamics or HullWhiteDynamics 
                    => new EulerMaruyamaScheme(),
                HestonVolatilityDynamics 
                    => new EulerMaruyamaScheme() { EnsurePositivity = true },
                _ => throw new NotSupportedException(nameof(dynamics)),
            };
        }
    }
}

           