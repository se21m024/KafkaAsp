using Microsoft.AspNetCore.Mvc;
using KafkaAsp.DTOs;
using KafkaAsp.Services;

namespace KafkaAsp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoreBankingController : ControllerBase
    {
        private readonly ILogger<CoreBankingController> _logger;
        private readonly ICoreBankingService _coreBankingService;

        public CoreBankingController(
            ILogger<CoreBankingController> logger,
            ICoreBankingService coreBankingService)
        {
            _logger = logger;
            _coreBankingService = coreBankingService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentOrder paymentOrder, CancellationToken ct = default)
        {
            try
            {
                await _coreBankingService.SendKafkaMessageAsync(paymentOrder, ct);
                return Accepted();
            }
            catch (Exception e)
            {
                _logger.LogError($"Unexpected error occurred in CreatePayment: {e}");
                throw;
            }
        }
    }
}
