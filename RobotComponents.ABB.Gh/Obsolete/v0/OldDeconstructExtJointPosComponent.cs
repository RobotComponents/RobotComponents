// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Deconstruct External Joint Position component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldDeconstructExtJointPosComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructExtJointPos class.
        /// </summary>
        public OldDeconstructExtJointPosComponent()
          : base("Deconstruct External Joint Position", "DeConExtJoint",
              "Deconstructs an External Joint Position Component into its parameters."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components : v" + RobotComponents.VersionNumbering.CurrentVersion,
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
            pManager.AddParameter(new Param_ExternalJointPosition(), "External Joint Position", "EJ", "The External Joint Position.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("External axis position A", "EAa", "Defines the position of the external logical axis A");
            pManager.Register_DoubleParam("External axis position B", "EAb", "Defines the position of the external logical axis B");
            pManager.Register_DoubleParam("External axis position C", "EAc", "Defines the position of the external logical axis C");
            pManager.Register_DoubleParam("External axis position D", "EAd", "Defines the position of the external logical axis D");
            pManager.Register_DoubleParam("External axis position E", "EAe", "Defines the position of the external logical axis E");
            pManager.Register_DoubleParam("External axis position F", "EAf", "Defines the position of the external logical axis F");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            ExternalJointPosition extJointPosition = null;

            // Catch the input data
            if (!DA.GetData(0, ref extJointPosition)) { return; }

            // Check if the object is valid
            if (!extJointPosition.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The External Joint Position is invalid");
            }

            // Output
            DA.SetData(0, extJointPosition[0]);
            DA.SetData(1, extJointPosition[1]);
            DA.SetData(2, extJointPosition[2]);
            DA.SetData(3, extJointPosition[3]);
            DA.SetData(4, extJointPosition[4]);
            DA.SetData(5, extJointPosition[5]);
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructExternalJointPosition_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8461CEC9-AADE-4818-A657-09A51C38882F"); }
        }
    }
}