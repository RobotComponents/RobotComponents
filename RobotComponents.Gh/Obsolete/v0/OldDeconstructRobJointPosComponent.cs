// This file is part of RobotComponents. RobotComponents is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Gh.Parameters.Actions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.18.000
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Deconstruct Rob Joint Position component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldDeconstructRobJointPosComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructExtJointPos class.
        /// </summary>
        public OldDeconstructRobJointPosComponent()
          : base("Deconstruct Robot Joint Position", "DeConRobJoint",
              "Deconstructs a Robot Joint Position Component into its parameters."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
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
            pManager.AddParameter(new Param_RobotJointPosition(), "Robot Joint Position", "RJ", "The Robot Joint Position.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("Robot axis position 1", "RA1", "Defines the position of robot axis 1 in degrees.");
            pManager.Register_DoubleParam("Robot axis position 2", "RA2", "Defines the position of robot axis 2 in degrees.");
            pManager.Register_DoubleParam("Robot axis position 3", "RA3", "Defines the position of robot axis 3 in degrees.");
            pManager.Register_DoubleParam("Robot axis position 4", "RA4", "Defines the position of robot axis 4 in degrees.");
            pManager.Register_DoubleParam("Robot axis position 5", "RA5", "Defines the position of robot axis 5 in degrees.");
            pManager.Register_DoubleParam("Robot axis position 6", "RA6", "Defines the position of robot axis 6 in degrees.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            RobotJointPosition robJointPosition = null;

            // Catch the input data
            if (!DA.GetData(0, ref robJointPosition)) { return; }

            // Check if the object is valid
            if (!robJointPosition.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Robot Joint Position is not valid");
            }

            // Output
            DA.SetData(0, robJointPosition[0]);
            DA.SetData(1, robJointPosition[1]);
            DA.SetData(2, robJointPosition[2]);
            DA.SetData(3, robJointPosition[3]);
            DA.SetData(4, robJointPosition[4]);
            DA.SetData(5, robJointPosition[5]);
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructRobotJointPosition_Icon; } 
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("909666AF-E627-4FB1-A54E-F9557E400211"); }
        }
    }
}