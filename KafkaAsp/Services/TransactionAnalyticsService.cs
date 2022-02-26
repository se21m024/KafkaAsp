using KafkaAsp.Configuration;
using KafkaAsp.DTOs;
using KafkaAsp.Extensions;
using KafkaAsp.Models;
using Microsoft.Extensions.Options;

namespace KafkaAsp.Services
{
    public interface ITransactionAnalyticsService
    {
        Task<LaundryCheckResult> GetLaundryChecksAsync(CancellationToken ct);
    }

    public class TransactionAnalyticsService : IHostedService, ITransactionAnalyticsService
    {
        private readonly KafkaAspConfig _config;
        private readonly ILogger<TransactionAnalyticsService> _logger;
        private readonly IKafkaMessageConsumer _kafkaMessageConsumer;
        private readonly IList<LaundryCheck> _laundryChecks = new List<LaundryCheck>();

        public TransactionAnalyticsService(
            IOptions<KafkaAspConfig> config,
            ILogger<TransactionAnalyticsService> logger,
            IKafkaMessageConsumer kafkaMessageConsumer)
        {
            _config = config.Value;
            _logger = logger;
            _kafkaMessageConsumer = kafkaMessageConsumer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () => await this.ConsumeKafkaMessages(cancellationToken), cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<LaundryCheckResult> GetLaundryChecksAsync(CancellationToken ct)
        {
            return new LaundryCheckResult
            {
                TotoalNumberOfPayments = _laundryChecks.Count,
                NumberOfDeclinedPayments = _laundryChecks.Count(x => x.Declined),
            };
        }

        private async Task ConsumeKafkaMessages(CancellationToken ct)
        {
            await _kafkaMessageConsumer.ConsumeAsync<LaundryCheck>(
                _config.Kafka.GroupId,
                _config.Kafka.Topics.LaundryCheck,
                this.ProcessKafkaMessageAsync,
                ct);
        }

        private async Task ProcessKafkaMessageAsync(LaundryCheck? laundryCheck, CancellationToken ct)
        {
            _logger.LogInformation($"Persisting new laundry check: {laundryCheck.Dump()}.");

            if (laundryCheck is null)
            {
                return;
            }

            _laundryChecks.Add(laundryCheck);
        }
    }
}
