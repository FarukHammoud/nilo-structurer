using Application;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers {
    // DEPRECATED
    [ApiController]
    [Route("[controller]")]
    public class PricerController : ControllerBase {

        private readonly ILogger<PricerController> _logger;
        private readonly PricerService _pricerService;

        public PricerController(ILogger<PricerController> logger,
            PricerService pricerService) {
            _logger = logger;
            _pricerService = pricerService;
        }

        [HttpPost(Name = "PostPricer")]
        public IActionResult Post([FromBody] PriceRequest request) {
            double? price = _pricerService.Price(request.Contracts[0]);
            if (!price.HasValue) {
                return Ok("No price");
            }
            return Ok(price.Value);
        }
    }
}
