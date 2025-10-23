// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2020 EDEK Uni Kassel
// Copyright (c) 2020-2025 Arjen Deetman
//
// Authors:
//   - Gabriel Rumph (2020)
//   - Benedikt Wannemacher (2020)
//   - Arjen Deetman (2020-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.CodeGeneration
{
    /// <summary>
    /// RobotComponents Deconstruct Rob Joint Position component.
    /// </summary>
    public class DeconstructJointTargetComponent : GH_RobotComponent
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructExtJointPos class.
        /// </summary>
        public DeconstructJointTargetComponent() : base("Deconstruct Joint Target", "DeConJointTar", "Deconstruct",
              "Deconstructs a Joint Target Component into its parameters.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_JointTarget(), "Joint Target", "JT", "The Joint Target.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string");
            pManager.RegisterParam(new Param_RobotJointPosition(), "Robot Joint Position", "RJ", "The resulting robot joint position");
            pManager.RegisterParam(new Param_ExternalJointPosition(), "External Joint Position", "EJ", "The resulting external joint position");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            JointTarget jointTarget = null;

            // Catch the input data
            if (!DA.GetData(0, ref jointTarget)) { return; }

            if (jointTarget != null)
            {
                // Check if the object is valid
                if (!jointTarget.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Joint Target is invalid");
                }

                // Output
                DA.SetData(0, jointTarget.Name);
                DA.SetData(1, jointTarget.RobotJointPosition);
                DA.SetData(2, jointTarget.ExternalJointPosition);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructJointTarget_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D32F2114-A1A2-45E3-95C6-A1D5A494AB4F"); }
        }
        #endregion
    }
}