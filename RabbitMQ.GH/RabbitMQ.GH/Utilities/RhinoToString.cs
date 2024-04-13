using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Newtonsoft.Json;
using RabbitMQ.GH.Properties;
using Rhino.DocObjects;
using Rhino.FileIO;
using Rhino.Geometry;
using Rhino.Render.ChangeQueue;
using static Rhino.FileIO.FileObjWriteOptions;

namespace RabbitMQ.GH.Utilities
{
    /// <summary>
    /// Represents a Rhino-to-String object converter component inheriting from the GH_component class.
    /// </summary>
    public class RhinoToString : GH_Component
    {
        #region Constructor

        /// <summary>
        /// Default constructor. Invokes the base class constructor.
        /// </summary>
        public RhinoToString()
          : base("RhinoToString", "RTS",
              "Convert rhino object to string",
              "RabbitMQ", "Utilities")
        {
        }

        #endregion Constructor

        /// <summary>
        /// Represents the exposure level of the component.
        /// </summary>
        /// <value>
        /// The value of 4 sets the Consumer component to be displayed in the second section of the Grasshopper toolbar.
        /// </value>
        public override GH_Exposure Exposure => (GH_Exposure)4;

        #region Methods

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Objects", "O", "Rhino objects to convert", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("JsonString", "S", "Json strings representing the rhino objects", GH_ParamAccess.list);
            pManager.AddTextParameter("Errors", "E", "Details about failed conversions", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Reads the input data and returns early if unsuccessful.
            List<GH_ObjectWrapper> objectWrappers = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList("Objects", objectWrappers)) return;

            // Stores converted objects and conversion errors.
            List<string> errors = new List<string>();
            List<string> jsons = Convertor.ConvertToStrings(objectWrappers, out errors);

            // Outputs the converted objects.
            DA.SetDataList("JsonString", jsons);
            DA.SetDataList("Errors", errors);

        }

        #endregion Methods

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.RTS;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("942688C4-255B-495E-8115-559BCBBEF248"); }
        }
    }
}