
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class AzureServiceBusMessageBus: IMessageBus
    {
        private string _connectionString = "";
     

        public AzureServiceBusMessageBus(IConfiguration configuration)
        {
            _connectionString = configuration["ServiceBusConnectionString"]; // Use indexer instead of GetValue
        }

        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            //ISenderClient senderClient = new TopicClient(_connectionString, topicName);
            //var jsonMessage = JsonConvert.SerializeObject(message);
            //var finalMessage = new Message(Encoding.UTF8.GetBytes(jsonMessage))
            //{
            //    CorrelationId = Guid.NewGuid().ToString()
            //};
            //await senderClient.SendAsync(finalMessage);

            //await senderClient.CloseAsync();


            await using var client = new ServiceBusClient(_connectionString);

            ServiceBusSender sender = client.CreateSender(topicName);

            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(finalMessage);

            await client.DisposeAsync();
        }
    }
    
}
