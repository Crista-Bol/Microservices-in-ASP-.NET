using System.Text;
using CommandsService.EventProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _conf;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _conf=configuration;
            _eventProcessor=eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory=new ConnectionFactory(){ HostName=_conf["RabbitMQHost"],Port=int.Parse(_conf["RabbitMQPort"])};

            _connection=factory.CreateConnection();
            _channel=_connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger",type: ExchangeType.Fanout);
            _queueName=_channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName,
                        exchange: "trigger",
                        routingKey: "");

            Console.WriteLine("--> Listening on the message bus");

            _connection.ConnectionShutdown+=RabbitMQ_ConnectionShutdown;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer=new EventingBasicConsumer(_channel);
            consumer.Received+=(ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event recieved.");
                var body=ea.Body;
                var notificationMessage=Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.EventProcessor(notificationMessage);

            };
            _channel.BasicConsume(queue:_queueName,autoAck:true,consumer:consumer);
            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object sender,ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection shutdown");
        }
        public override void Dispose()
        {
            if(_channel.IsOpen){
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}