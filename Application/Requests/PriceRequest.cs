namespace Application {
    public class PriceRequest {
        public String ContractType { get; set; } = "";
        public String UnderlyingCode { get; set; } = "";
        public String Maturity { get; set; } = "";
        public double Strike { get; set; } = 0;
    }
}
