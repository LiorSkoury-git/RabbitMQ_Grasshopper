using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using RabbitMQ.GH.Properties;
using Rhino.Geometry;

namespace RabbitMQ.GH.Utilities
{
    public class StringToRhino : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public StringToRhino()
          : base("StringToRhino", "STR",
              "Convert string to Rhino object",
              "RabbitMQ", "Utilities")
        {
        }

        public override GH_Exposure Exposure => (GH_Exposure)4;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("sObjects", "sO", "String objects to convert", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Objects", "O", "Converted Rhino objects", GH_ParamAccess.list);
            pManager.AddTextParameter("Errors", "E", "Details about failed conversions", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> objectWrappers = new List<string>();
            if (!DA.GetDataList("sObjects", objectWrappers)) return;

            List<string> errors = new List<string>();
            List<object> jsons = Convertor.ConvertToRhino(objectWrappers, out errors);

            DA.SetDataList("Objects", jsons);
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
                return Resources.STR;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("27AD955D-6091-4986-942D-6E34283C84D6"); }
        }
    }
}