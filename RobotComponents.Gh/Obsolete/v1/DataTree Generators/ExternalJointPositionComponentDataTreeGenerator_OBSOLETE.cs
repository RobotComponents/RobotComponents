// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.Gh.Parameters.Actions;

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Action : External Joint Position component. An inherent from the GH_Component Class.
    /// This is a dummy component that is hidden in Grasshopper. It is only called and used in
    /// the background to create a datatree structure that follows the Grasshopper logic.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class ExternalJointPositionComponentDataTreeGenerator_OBSOLETE : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public ExternalJointPositionComponentDataTreeGenerator_OBSOLETE()
          : base("External Joint Position", "RJ",
              "Defines a External Joint Position for a Joint Target declaration."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.ABB.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name as text", GH_ParamAccess.item, string.Empty);
            pManager.AddNumberParameter("External axis position A", "EAa", "Defines the position of external logical axis A.", GH_ParamAccess.item, 9e9);
            pManager.AddNumberParameter("External axis position B", "EAb", "Defines the position of external logical axis B.", GH_ParamAccess.item, 9e9);
            pManager.AddNumberParameter("External axis position C", "EAc", "Defines the position of external logical axis C.", GH_ParamAccess.item, 9e9);
            pManager.AddNumberParameter("External axis position D", "EAd", "Defines the position of external logical axis D.", GH_ParamAccess.item, 9e9);
            pManager.AddNumberParameter("External axis position E", "EAe", "Defines the position of external logical axis E.", GH_ParamAccess.item, 9e9);
            pManager.AddNumberParameter("External axis position F", "EAf", "Defines the position of external logical axis F.", GH_ParamAccess.item, 9e9);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_ExternalJointPosition(), "External Joint Position", "EJ", "The resulting external joint position");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            string name = string.Empty;
            double externalAxisValueA = 9e9;
            double externalAxisValueB = 9e9;
            double externalAxisValueC = 9e9;
            double externalAxisValueD = 9e9;
            double externalAxisValueE = 9e9;
            double externalAxisValueF = 9e9;

            // Catch input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref externalAxisValueA)) { return; }
            if (!DA.GetData(2, ref externalAxisValueB)) { return; }
            if (!DA.GetData(3, ref externalAxisValueC)) { return; }
            if (!DA.GetData(4, ref externalAxisValueD)) { return; }
            if (!DA.GetData(5, ref externalAxisValueE)) { return; }
            if (!DA.GetData(6, ref externalAxisValueF)) { return; }

            // Create external joint position
            ExternalJointPosition extJointPosition = new ExternalJointPosition(name, externalAxisValueA, externalAxisValueB, externalAxisValueC, externalAxisValueD, externalAxisValueE, externalAxisValueF);

            // Sets Output
            DA.SetData(0, extJointPosition);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            // This component is hidden. It is only used to create a datatree inside the real External Joint Position component.
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
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.ExternalJointPosition_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("244DC305-7367-4D50-A4B7-9CD24442C779"); }
        }
        #endregion
    }
}

