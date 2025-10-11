using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace MoneyManagerAPI.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static async Task EnsureTopicExistsAsync(this WebApplicationBuilder builder, string kafkaUri, string topic)
        {
            var adminConfig = new AdminClientConfig { BootstrapServers = kafkaUri };
            using var admin = new AdminClientBuilder(adminConfig).Build();

            try
            {
                var spec = new TopicSpecification
                {
                    Name = topic,
                    NumPartitions = 3,
                    ReplicationFactor = 3,
                    Configs = new Dictionary<string, string>
                    {
                        ["min.insync.replicas"] = "2"
                    }
                };

                await admin.CreateTopicsAsync([spec]);
            }
            catch (CreateTopicsException ex)
            {
                if (ex.Results.Count == 0 || ex.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                {
                    throw;
                }
            }
        }
    }
}
