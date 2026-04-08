using Domain;

namespace FixedIncomeServices {
    public class ExampleCurves {
        public static Curve ExampleSwapCurve = new Curve(new OrderedDictionary<DateTime, double>() {
            { DateTime.Now.AddDays(1*365), 2.78/100},
            { DateTime.Now.AddDays(2*365), 2.88/100},
            { DateTime.Now.AddDays(3*365), 2.88/100},
            { DateTime.Now.AddDays(4*365), 2.89/100},
            { DateTime.Now.AddDays(5*365), 2.91/100},
            { DateTime.Now.AddDays(6*365), 2.94/100},
            { DateTime.Now.AddDays(7*365), 2.98/100},
            { DateTime.Now.AddDays(8*365), 3.01/100},
            { DateTime.Now.AddDays(9*365), 3.05/100},
            { DateTime.Now.AddDays(10*365), 3.09/100},
            { DateTime.Now.AddDays(12*365), 3.16/100},
            { DateTime.Now.AddDays(15*365), 3.23/100},
            { DateTime.Now.AddDays(20*365), 3.25/100},
            { DateTime.Now.AddDays(30*365), 3.14/100}
        });
    }
}
