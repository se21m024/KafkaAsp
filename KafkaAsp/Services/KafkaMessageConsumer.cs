using System.Text.Json;
using Confluent.Kafka;
using KafkaAsp.Configuration;
using KafkaAsp.Extensions;
using Microsoft.Extensions.Options;

namespace KafkaAsp.Services
{
    public interface IKafkaMessageConsumer
    {
        Task ConsumeAsync<T>(
            string groupId,
            string topic,
            Func<T?, CancellationToken, Task> processMessageAsync,
            CancellationToken ct);
    }

    public class KafkaMessageConsumer : IKafkaMessageConsumer
    {
        private readonly KafkaAspConfig _config;
        private readonly ILogger<KafkaMessageConsumer> _logger;

        public KafkaMessageConsumer(
            IOptions<KafkaAspConfig> config,
            ILogger<KafkaMessageConsumer> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task ConsumeAsync<T>(
            string groupId,
            string topic,
            Func<T?, CancellationToken, Task> processMessageAsync,
            CancellationToken ct)
        {
            var config = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = _config.Kafka.BootstrapServers.Address,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            try
            {
                using var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build();
                consumerBuilder.Subscribe(topic);
                _logger.LogInformation($"Ready to process incoming Kafka messages of type {typeof(T).Name} from address {_config.Kafka.BootstrapServers.Address}.");

                while (true)
                {
                    try
                    {

                        var consumer = consumerBuilder.Consume(ct);
                        _logger.LogInformation(
                            $"Received Kafka message of type {typeof(T).Name}: {consumer.Message.Value.Dump()}.");

                        var message = JsonSerializer.Deserialize<T>(consumer.Message.Value);
                        await processMessageAsync(message, ct);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogError(
                            $"Failed to process Kafka message of type {typeof(T).Name}: Operation was canceled.");

                        consumerBuilder.Close();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Failed to process Kafka message of type {typeof(T).Name}: {e}");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }
    }
}
