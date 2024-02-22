using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace RabbitMQ.Core
{

    public class Connection
    {
        private RabbitMQ.Client.ConnectionFactory factory;
        private IConnection connection;
        public IModel channel;
        public List<QueueDeclareOk> queues = new List<QueueDeclareOk>();
        private string exchangeName;

        public EventingBasicConsumer consumer;

        public Connection() { }

        public void setFactory(string Host, int Port, string vHost = "/")
        {
            this.factory = new RabbitMQ.Client.ConnectionFactory
            {
                HostName = Host,
                Port = Port,
                VirtualHost = vHost
            };
        }

        public void setFactory(string Host, int Port, string User, string Password, string vHost = "/")
        {
            this.factory = new RabbitMQ.Client.ConnectionFactory
            {
                HostName = Host,
                Port = Port,
                UserName = User,
                Password = Password,
                VirtualHost = vHost
            };
        }

        public bool CreateConnection()
        {
            try
            {
                this.connection = factory.CreateConnection();
                this.channel = this.connection.CreateModel();
                return true;
            }
            catch (Exception ex)
            {
                // Handle connection errors
                return false;
            }
        }

        public void setQueue(string QueueName = "", bool Exclusive=false, bool competing=false)
        {
            var queue = channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: Exclusive,
                autoDelete: false,
                arguments: null);

            if (competing==true)
            {
                this.channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            }
            this.queues.Add(queue);
        }

        public void setExchange(string ExchangeName, int ExType=0)
        {
            if (ExType == 0) { channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);}
            if (ExType == 1) { channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct); }
            if (ExType == 2) { channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Topic); }
            this.exchangeName = ExchangeName;
        }

        public void setBinding(List<string> RoutingKeys)
        {
            foreach (string routingKey in RoutingKeys)
            {
                channel.QueueBind(queue: this.queues[0].QueueName, exchange: this.exchangeName, routingKey: routingKey);
            }
        }
        public void setBinding(string QueueName,string ExchangeName,List<string> RoutingKeys)
        {
            foreach (string routingKey in RoutingKeys)
            {
                channel.QueueBind(queue:QueueName, exchange: ExchangeName, routingKey: routingKey);
            }
        }

        public void startConsuming(EventingBasicConsumer consumer)
        {
            this.channel.BasicConsume(queue: this.queues[0].QueueName, autoAck: true, consumer:consumer);
        }

        public void closeConnection()
        {
            this.connection.Close();
        }

    }
}
