﻿using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.PaymentAPI.Messages;
using Newtonsoft.Json;
using PaymentProcessor;
using System.Text;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string subscriptionPayment;
       
        private readonly string orderPaymentProcessTopic;
        private readonly string orderUpdatePaymentResultTopic;


        

        private ServiceBusProcessor orderPaymentProcessor;
        private ServiceBusProcessor orderUpdatePaymentStatusProcessor;
        
        private readonly IProcessPayment _processPayment;

        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;

        public AzureServiceBusConsumer(IProcessPayment processPayment, IConfiguration configuration, IMessageBus messageBus)
        {
            _processPayment = processPayment;
            _configuration = configuration;
            _messageBus = messageBus;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            subscriptionPayment = _configuration.GetValue<string>("OrderPaymentProcessSubscription");
           
            orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
            orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

          

            var client = new ServiceBusClient(serviceBusConnectionString);

            orderPaymentProcessor = client.CreateProcessor(orderPaymentProcessTopic, subscriptionPayment);
            
        }

        public async Task Start()
        {
            orderPaymentProcessor.ProcessMessageAsync += ProcessPayment;
            orderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await orderPaymentProcessor.StartProcessingAsync();

            
        }
        public async Task Stop()
        {
            await orderPaymentProcessor.StopProcessingAsync();
            await orderPaymentProcessor.DisposeAsync();

            //await orderUpdatePaymentStatusProcessor.StopProcessingAsync();
            //await orderUpdatePaymentStatusProcessor.DisposeAsync();
        }
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task ProcessPayment(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

            var result = _processPayment.PaymentProcessor(); 

            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                OrderId = paymentRequestMessage.OrderId,
                Status = result,
                Email = paymentRequestMessage.Email
            };

           

            try
            {
                await _messageBus.PublishMessage(updatePaymentResultMessage, orderUpdatePaymentResultTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch(Exception e)
            {
                throw;
            }

        }

       
    }
}
