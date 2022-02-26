using System.Net;
using System.Text.Json;
using Confluent.Kafka;
using KafkaAsp.Configuration;
using KafkaAsp.Extensions;
using Microsoft.Extensions.Options;

namespace KafkaAsp.Services
{
    public interface IKafkaMessageProducer
    {
        Task SendAsync(string topic, object message, CancellationToken ct);
    }

    public class KafkaMessageProducer : IKafkaMessageProducer
    {
        private readonly KafkaAspConfig _config;
        private readonly ILogger<KafkaMessageProducer> _logger;

        public KafkaMessageProducer(
            IOptions<KafkaAspConfig> config,
            ILogger<KafkaMessageProducer> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task SendAsync(string topic, object message, CancellationToken ct)
        {
            var serializedMessage = JsonSerializer.Serialize(message);

            try
            {
                _logger.LogDebug($"Attempting to send Kafka message to topic {topic}: {serializedMessage.Dump()}, to address {_config.Kafka.BootstrapServers.Address}.");
                var config = new ProducerConfig
                {
                    BootstrapServers = _config.Kafka.BootstrapServers.Address,
                    ClientId = Dns.GetHostName()
                };

                using var producer = new ProducerBuilder<Null, string>(config).Build();
                var result = await producer.ProduceAsync(
                    topic,
                    new Message<Null, string>
                    {
                        Value = serializedMessage
                    },
                    ct);

                _logger.LogInformation($"Published Kafka message to topic {topic}: {serializedMessage.Dump()}, to address {_config.Kafka.BootstrapServers.Address}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send Kafka message to topic {topic}: {serializedMessage.Dump()}, {ex}");
                throw;
            }
        }
    }
}
