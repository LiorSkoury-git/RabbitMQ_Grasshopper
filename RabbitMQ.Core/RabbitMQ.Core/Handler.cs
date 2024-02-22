using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Core
{
    public class Handler
    {
        public Handler() { }

        public virtual List<object> HandleMessage(string body)
        {
            return new List<object>() { body };
        }
    }
}
