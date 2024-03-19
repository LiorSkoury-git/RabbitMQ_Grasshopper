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
    /// <summary>
    /// Represents a connection to a RabbitMQ server. 
    /// </summary>
    public class Connection
    {
        #region Fields

        /// <summary>
        /// Represents the RabbitMQ connection factory used to create connections to a RabbitMQ node.
        /// </summary>
        private RabbitMQ.Client.ConnectionFactory factory;

        /// <summary>
        /// Represents a TCP connection to a RabbitMQ broker.
        /// </summary>
        private IConnection connection;

        /// <summary>
        /// Represents the RabbitMQ channel associated with connection.
        /// </summary>
        public IModel channel;

        /// <summary>
        /// Represents a list of declared RabbitMQ queues to consume messages from.
        /// </summary>
        public List<QueueDeclareOk> queues = new List<QueueDeclareOk>();

        /// <summary>
        /// Represents the name of the exchange distributing messages.
        /// </summary>
        private string exchangeName;

        /// <summary>
        /// Represents the RabbitMQ consumer that will be used to get messages.
        /// </summary>
        public EventingBasicConsumer consumer;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Connection() { }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Sets up the RabbitMQ connection factory without authentication.
        /// </summary>
        /// <param name="Host">Server host name.</param>
        /// <param name="Port">Server port number.</param>
        /// <param name="vHost">Virtual host (default is "/").</param>
        public void setFactory(string Host, int Port, string vHost = "/")
        {
            this.factory = new RabbitMQ.Client.ConnectionFactory
            {
                HostName = Host,
                Port = Port,
                VirtualHost = vHost
            };
        }

        /// <summary>
        /// Sets up the RabbitMQ connection factory with authentication.
        /// </summary>
        /// <param name="Host">Server host name.</param>
        /// <param name="Port">Server port number.</param>
        /// <param name="User">User name.</param>
        /// <param name="Password">User password.</param>
        /// <param name="vHost">Virtual host (default is "/").</param>
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

        /// <summary>
        /// Creates a single-channel connection to a RabbitMQ broker.
        /// </summary>
        /// <returns>True if the connection was successfully created, false otherwise.</returns>
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

        /// <summary>
        /// Sets up a RabbitMQ queue associated to channel.
        /// </summary>
        /// <param name="QueueName">Queue name (default is "").</param>
        /// <param name="Exclusive">Declares if the queue is exclusive to connection (default is false).</param>
        /// <param name="competing">Declares if a competing consumer pattern will be used (default is false).</param>
        /// /// <remarks>
        /// The created queue is not durable, meaning it won't persist after a server restart.
        /// It won´t be deleted if it´s no longer in use.
        /// </remarks>
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
                /// Sets up limitations so at most one message (prefetchCount: 1) of unlimited size (prefetchSize: 0) 
                /// will be delivered by the server. This settings only apply to a client, and not to the channel (global: false).
                this.channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            }
            this.queues.Add(queue);
        }

        /// <summary>
        /// Sets up a RabbitMQ Exchange.
        /// </summary>
        /// <param name="ExchangeName">Exchange name (default is "").</param>
        /// <param name="ExType">Exchange type (default is 0, representing a fanout exchange).</param>
        public void setExchange(string ExchangeName, int ExType=0)
        {
            if (ExType == 0) { channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);}
            if (ExType == 1) { channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct); }
            if (ExType == 2) { channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Topic); }
            this.exchangeName = ExchangeName;
        }

        /// <summary>
        /// Sets up a binding between the exchange and the first queue in queues.
        /// </summary>
        /// <param name="RoutingKeys">List of routing keys to use for the binding.</param>
        public void setBinding(List<string> RoutingKeys)
        {
            foreach (string routingKey in RoutingKeys)
            {
                channel.QueueBind(queue: this.queues[0].QueueName, exchange: this.exchangeName, routingKey: routingKey);
            }
        }

        /// <summary>
        /// Sets up a binding between the exchange and a specified queue from queues.
        /// </summary>
        /// <param name="QueueName">Name of the queue to bind.</param>
        /// <param name="ExchangeName">Name of the exchange to bind.</param>
        /// <param name="RoutingKeys">List of routing keys to use for the binding.</param>
        public void setBinding(string QueueName,string ExchangeName,List<string> RoutingKeys)
        {
            foreach (string routingKey in RoutingKeys)
            {
                channel.QueueBind(queue:QueueName, exchange: ExchangeName, routingKey: routingKey);
            }
        }

        /// <summary>
        /// Sets a consumer to start consuming messages from the first queue in queues.
        /// </summary>
        /// <param name="consumer">The consumer to activate.</param>
        public void startConsuming(EventingBasicConsumer consumer)
        {
            this.channel.BasicConsume(queue: this.queues[0].QueueName, autoAck: true, consumer:consumer);
        }

        /// <summary>
        /// Closes the connection 
        /// </summary>
        public void closeConnection()
        {
            this.connection.Close();
        }

        #endregion Methods
    }
}
