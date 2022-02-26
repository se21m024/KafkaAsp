namespace KafkaAsp.Configuration
{
    public class KafkaAspConfig
    {
        public KafkaConfig Kafka { get; set; } = new();

        public DomainConfig Domain { get; set; } = new();

        public class DomainConfig
        {
            public decimal DeclineThreshold { get; set; } = 1000;
        }

        public class KafkaConfig
        {
            public string GroupId { get; set; } = "KafkaGroup";

            public TopicsConfig Topics { get; set; } = new();

            public BootstrapServersConfig BootstrapServers { get; set; } = new();

            public class TopicsConfig
            {
                public string Payments { get; set; } = "Payments";

                public string LaundryCheck { get; set; } = "LaundryCheck";
            }

            public class BootstrapServersConfig
            {
                public string HostName { get; set; } = "localhost";

                public int Port { get; set; } = 9092;

                public string Address => $"{this.HostName}:{this.Port}";
            }
        }
    }
}
