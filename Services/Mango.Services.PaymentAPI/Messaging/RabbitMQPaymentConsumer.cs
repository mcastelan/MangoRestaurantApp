﻿using Mango.Services.PaymentAPI.Messages;

using Mango.Services.PaymentAPI.RabbitMQSender;
using Newtonsoft.Json;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {

        
        private readonly IRabbitMQPaymentMessageSender _rabbitMQPaymentMessageSender;
        private readonly IProcessPayment _processPayment;
        private IConnection _connection;
        private IModel _channel;


        public RabbitMQPaymentConsumer( IRabbitMQPaymentMessageSender rabbitMQPaymentMessageSender, IProcessPayment processPayment)
        {
          
            this._rabbitMQPaymentMessageSender = rabbitMQPaymentMessageSender;
            this._processPayment = processPayment;
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "orderpaymentprocesstopic", durable: false, exclusive: false, autoDelete: false, arguments: null);
           
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
           stoppingToken.ThrowIfCancellationRequested();
           var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(content);
                HandleMessage(paymentRequestMessage).GetAwaiter().GetResult();
                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume("orderpaymentprocesstopic", false, consumer);
            return Task.CompletedTask;
        }
        private async Task HandleMessage(PaymentRequestMessage paymentRequestMessage)
        {


            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                OrderId = paymentRequestMessage.OrderId,
                Status = result,
                Email = paymentRequestMessage.Email
            };



            try
            {
                //await _messageBus.PublishMessage(updatePaymentResultMessage, orderUpdatePaymentResultTopic);
                //await args.CompleteMessageAsync(args.Message);
                _rabbitMQPaymentMessageSender.SendMessage(updatePaymentResultMessage);

            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
