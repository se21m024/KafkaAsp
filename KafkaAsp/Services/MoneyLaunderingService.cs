using KafkaAsp.Configuration;
using KafkaAsp.DTOs;
using KafkaAsp.Models;
using Microsoft.Extensions.Options;

namespace KafkaAsp.Services
{
    public class MoneyLaunderingService : IHostedService
    {
        private readonly KafkaAspConfig _config;
        private readonly IKafkaMessageConsumer _kafkaMessageConsumer;
        private readonly IKafkaMessageProducer _kafkaMessageProducer;

        public MoneyLaunderingService(
            IOptions<KafkaAspConfig> config,
            IKafkaMessageConsumer kafkaMessageConsumer,
            IKafkaMessageProducer kafkaMessageProducer)
        {
            _config = config.Value;
            _kafkaMessageConsumer = kafkaMessageConsumer;
            _kafkaMessageProducer = kafkaMessageProducer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () => await this.ConsumeKafkaMessages(cancellationToken), cancellationToken);
            return Task.CompletedTask;
        }

        private async Task ConsumeKafkaMessages(CancellationToken ct)
        {
            await _kafkaMessageConsumer.ConsumeAsync<PaymentOrder>(
                _config.Kafka.GroupId,
                _config.Kafka.Topics.Payments,
                this.ProcessPaymentAsync,
                ct);
        }

        private async Task ProcessPaymentAsync(PaymentOrder? payment, CancellationToken ct)
        {
            if (payment is null)
            {
                throw new ArgumentNullException(nameof(payment));
            }

            var laundryCheck = new LaundryCheck
            {
                Declined = payment.Amount > _config.Domain.DeclineThreshold,
                Payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    DateTimeUtc = DateTime.UtcNow,
                    FromAccountId = payment.FromAccountId,
                    ToAccountId = payment.ToAccountId,
                    Amount = payment.Amount,
                },
            };

            await _kafkaMessageProducer.SendAsync(_config.Kafka.Topics.LaundryCheck, laundryCheck, ct);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
