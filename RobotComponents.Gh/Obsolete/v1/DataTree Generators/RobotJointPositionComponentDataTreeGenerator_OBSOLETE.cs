// This file is part of RobotComponents. RobotComponents is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Gh.Parameters.Actions;

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Action : Robot Joint Position component. An inherent from the GH_Component Class.
    /// This is a dummy component that is hidden in Grasshopper. It is only called and used in
    /// the background to create a datatree structure that follows the Grasshopper logic.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class RobotJointPositionComponentDataTreeGenerator_OBSOLETE : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RobotJointPositionComponentDataTreeGenerator_OBSOLETE()
          : base("Robot Joint Position", "RJ",
              "Defines a Robot Joint Position for a Joint Target declaration."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name as text", GH_ParamAccess.item, string.Empty);
            pManager.AddNumberParameter("Robot joint position 1", "RA1", "Defines the position of robot joint 1 in degrees.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Robot joint position 2", "RA2", "Defines the position of robot joint 2 in degrees.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Robot joint position 3", "RA3", "Defines the position of robot joint 3 in degrees.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Robot joint position 4", "RA4", "Defines the position of robot joint 4 in degrees.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Robot joint position 5", "RA5", "Defines the position of robot joint 5 in degrees.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Robot joint position 6", "RA6", "Defines the position of robot joint 6 in degrees.", GH_ParamAccess.item, 0.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_RobotJointPosition(), "Robot Joint Position", "RJ", "The resulting robot joint position");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Variables
            string name = string.Empty;
            double internalAxisValue1 = 0.0;
            double internalAxisValue2 = 0.0;
            double internalAxisValue3 = 0.0;
            double internalAxisValue4 = 0.0;
            double internalAxisValue5 = 0.0;
            double internalAxisValue6 = 0.0;

            // Catch input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref internalAxisValue1)) { return; }
            if (!DA.GetData(2, ref internalAxisValue2)) { return; }
            if (!DA.GetData(3, ref internalAxisValue3)) { return; }
            if (!DA.GetData(4, ref internalAxisValue4)) { return; }
            if (!DA.GetData(5, ref internalAxisValue5)) { return; }
            if (!DA.GetData(6, ref internalAxisValue6)) { return; }

            // Create external joint position
            RobotJointPosition robJointPosition = new RobotJointPosition(name, internalAxisValue1, internalAxisValue2, internalAxisValue3, internalAxisValue4, internalAxisValue5, internalAxisValue6);

            // Sets Output
            DA.SetData(0, robJointPosition);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            // This component is hidden. It is only used to create a datatree inside the real Robot Joint Position component.
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
            get { return Properties.Resources.RobotJointPosition_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("257F5226-1507-4AA0-BB55-DB4C8D419E80"); }
        }
        #endregion
    }
}

