using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Core;
using Rhino;
using System.Text;
using RabbitMQ.GH.Properties;

namespace RabbitMQ.GH.Consumer_Producer
{
    public class Consumer : GH_Component
    {
        private bool timerStarted;
        private List<object> lastReceivedMessage;
        private Connection connection;
        private Handler handler;
        private int counter;

        public Consumer()
          : base("Consumer", "Consumer",
            "A consumer rabbitMQ object",
            "RabbitMQ", "General")
        {
            counter = 0;
            lastReceivedMessage = new List<object>() { "Turn on the Run to start consuming" };
            timerStarted = false;
        }

        public override GH_Exposure Exposure => (GH_Exposure)4;


        private void setConnection(string host, int port, string vHost)
        {
            if (vHost==null) { vHost = "/"; }
            connection = new Connection();
            if (RhinoDoc.ActiveDoc.RuntimeData.ContainsKey("RabbitMQ_USER") && RhinoDoc.ActiveDoc.RuntimeData.ContainsKey("RabbitMQ_PASS"))
            {
                connection.setFactory(host, port, (string)RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_USER"], (string)RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_PASS"],vHost);
            }
            else
            {
                connection.setFactory(host, port,vHost);
            }
            bool connect = connection.CreateConnection();
            if (!connect) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Error connecting to RabbitMQ"); }
        }

        private void setConnectionDetails(string queue, string exchange, int exType, List<string> routingKeys)
        {
            if (queue != "") { connection.setQueue(queue, Exclusive: false); }
            else { connection.setQueue(queue, Exclusive: true); }

            if (exchange != "" && exchange != null)
            {
                connection.setExchange(exchange, exType);
                if (routingKeys != null)
                {
                    if (routingKeys.Count > 0)
                    {
                        if (queue == "") { connection.setBinding(routingKeys); }
                        else { connection.setBinding(queue, exchange, routingKeys); }
                    }
                    else
                    {
                        routingKeys = new List<string>() { "" };
                        connection.setBinding(queue, exchange, routingKeys);
                    }
                }
                else
                {
                    routingKeys = new List<string>() { "" };
                    connection.setBinding(queue, exchange, routingKeys);
                }
            }

            var consumer = new EventingBasicConsumer(connection.channel);
            consumer.Received += HandleReceivedMessage;

            lastReceivedMessage = new List<object>() { "Waiting for messages..." };
            connection.channel.BasicConsume(queue: connection.queues[0].QueueName, autoAck: false, consumer: consumer);
            timerStarted = true;
        }
        private void HandleMessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            // Acknowledge the message
            connection.channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            byte[] body = ea.Body.ToArray();
            var data = Encoding.UTF8.GetString(body);
            var result = handler.HandleMessage(data);
            lastReceivedMessage = result;
            ExpireSolution(true);
        }
        private void HandleReceivedMessage(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                RhinoApp.WriteLine("Handling message...");
                connection.channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var result = handler.HandleMessage(body);
                lastReceivedMessage = result;
                counter++;
                RhinoApp.WriteLine("Message handled successfully.");
                ExpireSolution(true);
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Exception in HandleMessageReceived: {ex.Message}");
                ExpireSolution(true);
            }

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Start or stop consuming", GH_ParamAccess.item);
            pManager.AddTextParameter("Host", "H", "Host url name", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Port", "P", "Port number of the broker", GH_ParamAccess.item);
            pManager.AddTextParameter("Queue", "Q", "Queue name for the consumer", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Competing", "C", "True if competing consumers (default=fasle)", GH_ParamAccess.item);
            pManager.AddTextParameter("Exchange", "Ex", "Exchange name for the consumer", GH_ParamAccess.item);
            pManager.AddIntegerParameter("ExchangeType", "Ex_T", "Exchange type: 0=Fanout,1=Direct,2=Topic", GH_ParamAccess.item);
            pManager.AddTextParameter("RoutingKeys", "RK", "Routing keys as list", GH_ParamAccess.list);
            pManager.AddGenericParameter("Handler", "Ha", "Handler object to do somthing with the income data", GH_ParamAccess.item);
            pManager.AddTextParameter("vHost", "VH", "Virtual host if needed", GH_ParamAccess.item);

            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;
            pManager[9].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Received", "R", "Last Recevied message", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool b = false;
            string host = null;
            int port = 5672;
            string queue = "";
            bool competing = false;
            string exchange = null;
            int exType = 1;
            List<string> routingKeys = new List<string>();
            handler = new Handler();
            string vHost = null;

            DA.GetData("Run", ref b);
            DA.GetData("Host", ref host);
            DA.GetData("Port", ref port);
            DA.GetData("Queue", ref queue);
            DA.GetData("Competing", ref competing);
            DA.GetData("Exchange", ref exchange);
            DA.GetData("ExchangeType", ref exType);
            DA.GetDataList("RoutingKeys", routingKeys);
            DA.GetData("Handler", ref handler);
            DA.GetData("vHost",ref vHost);


            if (b)
            {
                if (!timerStarted)
                {
                    setConnection(host, port, vHost);
                    setConnectionDetails(queue, exchange, exType, routingKeys);
                }
            }
            else
            {
                if (timerStarted)
                {
                    connection.closeConnection(); // Close the connection setup if not done yet
                    lastReceivedMessage = new List<object>() { "Turn on the Run to start consuming" };
                    timerStarted = false;
                }
            }

            DA.SetDataList("Received", lastReceivedMessage);

        }
        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get 
            {
                return Resources.CONS;
            }
            //return Resources.

        }
        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("656192ec-5cc8-43ee-8b8d-09970a8597c9");
    }
}