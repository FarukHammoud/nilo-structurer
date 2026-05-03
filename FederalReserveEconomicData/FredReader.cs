using System;
using System.Collections.Generic;
using System.Text;

namespace FederalReserveEconomicData {
    public static class FredReader {
        public static List<FredRate> Read(string path) {
            return File.ReadLines(path)
                .Skip(1)
                .Select(line => {
                    var parts = line.Split(',');
                    return new FredRate(
                        Tenor: double.Parse(parts[0]),
                        Rate: double.Parse(parts[1]),
                        Date: DateTime.Parse(parts[2])
                    );
                })
                .ToList();
        }
    }
}
