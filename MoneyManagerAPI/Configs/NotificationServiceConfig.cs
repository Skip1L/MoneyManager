namespace MoneyManagerAPI.Configs
{
    public class NotificationServiceConfig
    {
        public string KafkaUri { get; set; }
        public string RabbitMQUri { get; set; }
        public string RabbitMQUsername { get; set; }
        public string RabbitMQPassword { get; set; }
        public string GrpcUri { get; set; }
        public string HttpUri { get; set; }
    }
}
