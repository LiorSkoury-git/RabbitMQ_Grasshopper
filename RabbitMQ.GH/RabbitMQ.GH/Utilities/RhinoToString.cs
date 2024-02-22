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
    public class RhinoToString : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public RhinoToString()
          : base("RhinoToString", "RTS",
              "Convert rhino object to string",
              "RabbitMQ", "Utilities")
        {
        }

        public override GH_Exposure Exposure => (GH_Exposure)4;

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

            List<GH_ObjectWrapper> objectWrappers = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList("Objects", objectWrappers)) return;

            List<string> errors = new List<string>();
            List<string> jsons = Convertor.ConvertToStrings(objectWrappers, out errors);

            DA.SetDataList("JsonString", jsons);
            DA.SetDataList("Errors", errors);

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