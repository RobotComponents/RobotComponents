// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Naming component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldNameGeneratorComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public OldNameGeneratorComponent()
          : base("Name Generator", "NG",
              "This components can be used to generate the datatree for the target, speed data and zone data names if datatrees are used."
                + System.Environment.NewLine + System.Environment.NewLine + "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return true; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name as text", GH_ParamAccess.item, "defaultName");
            pManager.AddGenericParameter("DT", "DT", "The data tree structure", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string stored in the data tree structure");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "This component is OBSOLETE.");

            // Input variables
            GH_Structure<IGH_Goo> data;
            string name = string.Empty;

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetDataTree(1, out data)) return;

            // Output variable
            GH_Structure<GH_String> names = new GH_Structure<GH_String>();

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