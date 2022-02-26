namespace KafkaAsp.Models
{
    public class Payment
    {
        public Guid Id { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public int FromAccountId { get; set; }

        public int ToAccountId { get; set; }

        public decimal Amount { get; set; }
    }
}
