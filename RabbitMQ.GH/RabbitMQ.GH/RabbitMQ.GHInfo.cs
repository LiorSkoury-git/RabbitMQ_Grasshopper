using Grasshopper;
using Grasshopper.Kernel;
using RabbitMQ.GH.Properties;
using System;
using System.Drawing;

namespace RabbitMQ.GH
{

    /// <summary>
    /// Represents the general information for a grasshopper assembly.
    /// </summary>
    public class RabbitMQ_GHInfo : GH_AssemblyInfo
    {
        public override string Name => "RabbitMQ.GH";

        // Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon
        { get { return Resources.BASIC; } }

        // Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        // Unique identifier for the assembly.
        public override Guid Id => new Guid("3fd45f85-857f-4bf3-b5c3-237d7332444b");

        // Return a string identifying you or your company.
        public override string AuthorName => "Lior Skoury";

        // Return a string representing your preferred contact details.
        public override string AuthorContact => "lior.skoury@icd.uni-stuttgart.de";
    }
}