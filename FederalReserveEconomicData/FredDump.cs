using System;
using System.Collections.Generic;
using System.Text;

namespace FederalReserveEconomicData {
    
    public static class FredDump {
        public static void Write(string path, IEnumerable<FredRate> rates) {
            using var writer = new StreamWriter(path);
            writer.WriteLine("Tenor,Rate,Date");
            foreach (var r in rates) {
                writer.WriteLine($"{r.Tenor},{r.Rate},{r.Date:yyyy-MM-dd}");
            }
        }
    }
}
