using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Core
{
    /// <summary>
    /// Represents a Handler to wrap the received messages inside of a list.
    /// </summary>
    public class Handler
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Handler() { }

        /// <summary>
        /// Wraps the body of a message inside of a list.
        /// </summary>
        /// <param name="body">Body of the message to wrap.</param>
        /// <returns>A List with body as its only element</returns>
        public virtual List<object> HandleMessage(string body)
        {
            return new List<object>() { body };
        }
    }
}
