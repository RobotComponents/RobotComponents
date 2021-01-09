// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Joint Target component. An inherent from the GH_Component Class.
    /// This is a dummy component that is hidden in Grasshopper. It is only called and used in
    /// the background to create a datatree structure that follows the Grasshopper logic.
    /// </summary>
    public class JointTargetComponentDataTreeGenerator : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public JointTargetComponentDataTreeGenerator()
          : base("Joint Target", "JT",
              "Defines a Joint Target for a Move instruction."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; } // This component is hidden. It is only used to create a datatree inside the real Joint Target component (according to the Grasshopper logic)
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name as text", GH_ParamAccess.item, "defaultTar");
            pManager.AddParameter(new RobotJointPositionParameter(), "Robot Joint Position", "RJ", "Defines the robot joint position", GH_ParamAccess.item);
            pManager.AddParameter(new ExternalJointPositionParameter(), "External Joint Position", "EJ", "Defines the external axis joint position", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new JointTargetParameter(), "Joint Target", "JT", "The resulting Joint Target");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Variables
            string name = "";
            RobotJointPosition robotJointPosition = new RobotJointPosition();
            ExternalJointPosition externalJointPosition = new ExternalJointPosition(); 

            // Catch input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref robotJointPosition)) { robotJointPosition = new RobotJointPosition(); }
            if (!DA.GetData(2, ref externalJointPosition)) { externalJointPosition = new ExternalJointPosition(); }

            // Replace spaces
            name = HelperMethods.ReplaceSpacesAndRemoveNewLines(name);

            JointTarget jointTarget = new JointTarget(name, robotJointPosition, externalJointPosition);

            // Sets Output
            DA.SetData(0, jointTarget);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.JointTarget_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B0CA5AB2-402B-4F01-B7E6-FDAEC59DAC5C"); }
        }

    }
}
