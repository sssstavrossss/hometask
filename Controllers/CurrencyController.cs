using Microsoft.AspNetCore.Mvc;
using HometaskLib.Application;
using HometaskLib.Models.Dto;

namespace Hometask.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableCurrencies()
        {
            var currencies = await _currencyService.GetAvailableCurrenciesAsync();
            return Ok(currencies);
        }
        
        [HttpPost("convert")]
        public async Task<IActionResult> ConvertCurrency([FromBody] CurrencyConversionRequest request)
        {
            var conversionResult = await _currencyService.ConvertCurrencyAsync(request);
            if (conversionResult == null)
            {
                return BadRequest("Conversion failed. One or both currency rates could not be found.");
            }
            return Ok(conversionResult);
        }
    }
