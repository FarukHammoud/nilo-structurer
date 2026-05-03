using System;
using System.Collections.Generic;
using System.Text;

namespace FederalReserveEconomicData {
    public record FredRate(double Tenor, double Rate, DateTime Date);
}
