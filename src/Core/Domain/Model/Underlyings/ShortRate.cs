using System.Xml.Linq;

namespace Domain {
    public record ShortRate : Underlying {
        public override Currency Currency { get; }
        public ShortRate(Currency currency) : base(currency.Code + "_SHORT_RATE") {
            Currency = currency;
        }
        public override IEnumerable<Underlying> Dependencies => [this];
    }
}
