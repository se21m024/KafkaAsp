using KafkaAsp.DTOs;
using KafkaAsp.Services;
using Microsoft.AspNetCore.Mvc;

namespace KafkaAsp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionAnalyticsController
    {
        private readonly ILogger<TransactionAnalyticsController> _logger;
        private readonly ITransactionAnalyticsService _transactionAnalyticsService;

        public TransactionAnalyticsController(
            ILogger<TransactionAnalyticsController> logger,
            ITransactionAnalyticsService transactionAnalyticsService)
        {
            _logger = logger;
            _transactionAnalyticsService = transactionAnalyticsService;
        }

        [HttpGet("LaundryCheckSummary")]
        public async Task<ActionResult<LaundryCheckResult>> GetLaundryCheckSummary(CancellationToken ct = default)
        {
            try
            {
                var res = await _transactionAnalyticsService.GetLaundryChecksAsync(ct);
                _logger.LogInformation($"Returning laundry check summary: total number: {res.TotoalNumberOfPayments}, declined :{res.NumberOfDeclinedPayments}.");
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"Unexpected error occurred in GetLaundryCheckSummary: {e}");
                throw;
            }
        }
    }
}
