using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _conf;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _conf=configuration;
            var factory=new ConnectionFactory(){HostName=_conf["RabbitMQHost"],
            Port=int.Parse(_conf["RabbitMQPort"])};

            try{
               _connection=factory.CreateConnection();
               _channel=_connection.CreateModel();

               _channel.ExchangeDeclare(exchange: "trigger",type: ExchangeType.Fanout);
               _connection.ConnectionShutdown+=RabbitMQ_ConnectionShutdown;
               Console.WriteLine("--> Connected to Message Bus");

            }catch(Exception ex)
            {
                Console.WriteLine($"--> Coudn't connect to message bus: {ex.Message}");
            }
        }
        public void PublishedNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message=JsonSerializer.Serialize(platformPublishedDto);
            if(_connection.IsOpen)
            {
               Console.WriteLine("-->RabbitMQ Connection Open, Sending a message");
               sendMessage(message);
            }else
            {
                Console.WriteLine("-->RabbitMQ Connection Closed, Couldn't send a message");
            }
        }
        private void sendMessage(string message)
        {
            var body=Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger",
                                    routingKey: "",
                                    basicProperties: null,
                                    body: body);
            Console.WriteLine("--> We have sent the message "+body);
        }

        public void dispose()
        {
            Console.WriteLine("--> message bus is disposed");

            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
        private void RabbitMQ_ConnectionShutdown(object sender,ShutdownEventArgs e)
        {
            Console.WriteLine("--> Rabbit MQ Connection Shut Down");
        }
    }
}