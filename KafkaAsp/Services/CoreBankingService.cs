using KafkaAsp.Configuration;
using KafkaAsp.DTOs;
using Microsoft.Extensions.Options;

namespace KafkaAsp.Services
{
    public interface ICoreBankingService
    {
        Task SendKafkaMessageAsync(PaymentOrder paymentOrder, CancellationToken ct);
    }

    public class CoreBankingService : ICoreBankingService
    {
        private readonly KafkaAspConfig _config;
        private readonly IKafkaMessageProducer _kafkaMessageProducer;

        public CoreBankingService(
            IOptions<KafkaAspConfig> config,
            IKafkaMessageProducer kafkaMessageProducer)
        {
            _config = config.Value;
            _kafkaMessageProducer = kafkaMessageProducer;
        }

        public async Task SendKafkaMessageAsync(PaymentOrder paymentOrder, CancellationToken ct)
        {
            await _kafkaMessageProducer.SendAsync(_config.Kafka.Topics.Payments, paymentOrder, ct);
        }
    }
}
