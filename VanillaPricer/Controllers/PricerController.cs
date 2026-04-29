using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using PositionServices;
using Application;

namespace VanillaPricer.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class PricerController : ControllerBase {

        private readonly ILogger<PricerController> _logger;
        private readonly PricerService _pricerService;
        private readonly PositionService _positionService;

        public PricerController(ILogger<PricerController> logger,
            PricerService pricerService,
            PositionService positionService) {
            _logger = logger;
            _pricerService = pricerService;
            _positionService = positionService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Route("price")]

        // only server project should have this controller
        public ActionResult<double> GetPrice() {
            EuropeanCall contract = new() {
                Maturity = DateTime.Today,
                Strike = 10,
                Underlying = new Equity("MSFT"),
                Currency = Currencies.USD
            };
            return CreatedAtAction("price", _pricerService.Price(contract));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Route("contracts")]
        public ActionResult<List<String>> GetContracts() {
            List<VanillaContract> contracts = _positionService.getContracts("VANILL");
            if (contracts == null) {
                _logger.LogError("No contracts found in this portfolio");
                return null;
            }
            return CreatedAtAction("contracts", JsonSerializer.Serialize(contracts.Select(contract => contract.Underlying.Code).ToList()));
        }
    }
}