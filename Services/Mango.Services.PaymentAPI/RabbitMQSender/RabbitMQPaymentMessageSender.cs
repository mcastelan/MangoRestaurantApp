﻿using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Mango.Services.PaymentAPI.RabbitMQSender
{
    public class RabbitMQPaymentMessageSender : IRabbitMQPaymentMessageSender
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;
        private const string ExchangeName = "DirectPaymentUpdate_Exchange";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
        private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

        public RabbitMQPaymentMessageSender()
        {
            _hostName = "localhost";
            _username = "guest";
            _password = "guest";
        }

        public void SendMessage(BaseMessage message)
        {
            if (ConnectionExists())
            {
                using var channel = _connection.CreateModel();
               channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct,durable:false);
                channel.QueueDeclare(queue: PaymentOrderUpdateQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue: PaymentEmailUpdateQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                channel.QueueBind(queue: PaymentOrderUpdateQueueName, exchange: ExchangeName, routingKey: "PaymentOrder");
                channel.QueueBind(queue: PaymentEmailUpdateQueueName, exchange: ExchangeName, routingKey: "PaymentEmail");

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                channel.BasicPublish(exchange: ExchangeName, "PaymentEmail", basicProperties: null, body: body);
                channel.BasicPublish(exchange: ExchangeName, "PaymentOrder", basicProperties: null, body: body);
            }
            


        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _username,
                    Password = _password
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception)
            {
                //log exception
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }
            CreateConnection();
            return _connection != null;
        }
    }
}
