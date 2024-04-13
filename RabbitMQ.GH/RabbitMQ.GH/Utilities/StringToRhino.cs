using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using RabbitMQ.GH.Properties;
using Rhino.Geometry;

namespace RabbitMQ.GH.Utilities
{
    /// <summary>
    /// Represents a String-to-Rhino object converter component inheriting from the GH_component class.
    /// </summary>
    public class StringToRhino : GH_Component
    {
        #region Constructor

        /// <summary>
        /// Default constructor. Invokes the base class constructor.
        /// </summary>
        public StringToRhino()
          : base("StringToRhino", "STR",
              "Convert string to Rhino object",
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
            // Reads the input data and returns early if unsuccessful.
            List<string> objectWrappers = new List<string>();
            if (!DA.GetDataList("sObjects", objectWrappers)) return;

            // Stores converted objects and conversion errors.
            List<string> errors = new List<string>();
            List<object> jsons = Convertor.ConvertToRhino(objectWrappers, out errors);

            // Outputs the converted objects.
            DA.SetDataList("Objects", jsons);
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