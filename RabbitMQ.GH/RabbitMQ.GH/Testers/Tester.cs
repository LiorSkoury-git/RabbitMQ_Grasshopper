using System;
using System.Collections.Generic;
using System.Text;
using Grasshopper.Kernel;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using RabbitMQ.Core;
using Rhino.Geometry;
using RabbitMQ.GH.Properties;

/*
namespace RabbitMQ.GH.Testers
{
    public class TestComponent : GH_Component
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        private Handler handler;
        private bool timerStarted;
        private List<object> lastReceivedMessage;

        public TestComponent()
            : base("Tester", "Tester",
                  "Description of component",
                  "RabbitMQ", "Competing")
        {
            factory = new ConnectionFactory
            {
                HostName = "rq.desfab.xyz",
                Port = 8432,
                UserName = "rq_a_user",
                Password = "rq18023005khtur"
            };

            handler = new Handler();
            timerStarted = false;
        }

        private void ConnectToRabbitMQ()
        {
            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.QueueDeclare(
                    queue: "letterbox_cs",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += HandleMessageReceived;

                channel.BasicConsume(queue: "letterbox_cs", autoAck: false, consumer: consumer);

                timerStarted = true; // Set the flag to indicate that the timer should be started
            }
            catch (Exception ex)
            {
                // Handle connection errors
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Error connecting to RabbitMQ: {ex.Message}");
            }
        }

        private void HandleMessageReceived(object sender, BasicDeliverEventArgs ea)
        {

            // Acknowledge the message
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            byte[] body = ea.Body.ToArray();
            var data = Encoding.UTF8.GetString(body);
            var result = handler.HandleMessage(data);
            lastReceivedMessage = result;
            ExpireSolution(true);
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "R", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("M", "M", "M", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool b = false;
            DA.GetData("Run", ref b);

            if (b)
            {
                if (!timerStarted)
                {
                    ConnectToRabbitMQ(); // Start the connection setup if not done yet
                }
            }
            else
            {
                if (timerStarted)
                {
                    connection.Close(); // Close the connection setup if not done yet
                    timerStarted = false;
                }
            }

            // Set the Grasshopper data
            DA.SetDataList("M", lastReceivedMessage);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {

                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("83A75A94-6B7A-442D-97D8-151155A9384D"); }
        }
    }
}
*/