
using Azure.Messaging.ServiceBus;

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
        private string _connectionString =
            "Endpoint=sb://mangorestmcastelan.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gkhYSRgvDLbvRYThUpUaeZNaaWdtsVz4K+ASbFejIGQ=";
           
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
