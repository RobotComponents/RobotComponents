using System;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;

namespace RobotComponents.Components
{
    public class NamingComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public NamingComponent()
          : base("Naming", "N",
              "This components makes the datatree for the target and speed datas names if datatrees are used." 
                + System.Environment.NewLine + "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name as string", GH_ParamAccess.item, "defaultName");
            pManager.AddGenericParameter("DT", "DT", "The data tree structure", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string stored in the data tree structure");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            GH_Structure<IGH_Goo> data;
            string name = String.Empty;

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetDataTree(1, out data)) return;

            // Get tthe paths of the datatree
            var paths = data.Paths;

            // Output variable
            GH_Structure<GH_String> names = new GH_Structure<GH_String>();

            // Make the output datatree with names
            for (int i = 0; i < data.Branches.Count; i++)
            {
                var branches = data.Branches[i];
                GH_Path iPath = paths[i];
                string pathString = iPath.ToString();
                string newPath = pathString.Replace("{", "").Replace(";", "_").Replace("}", "");

                for (int j = 0; j < branches.Count; j++)
                {
                    string myName = name + "_" + newPath + "_" + j;
                    GH_String converted = new GH_String(myName);
                    names.Append(converted, iPath);
                }
            }

            // Output
            DA.SetDataTree(0, names);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.Naming_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("74DE087C-11F8-4212-AB08-A5B572644D57"); }
        }

    }
}