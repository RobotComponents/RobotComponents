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
    /// RobotComponents Deconstruct Target component.
    /// </summary>
    public class DeconstructRobotTargetComponent : GH_RobotComponent
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructTarget class.
        /// </summary>
        public DeconstructRobotTargetComponent() : base("Deconstruct Robot Target", "DeConRobTar", "Deconstruct",
              "Deconstructs a Robot Target into its parameters.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_RobotTarget(), "Robot Target", "RT", "Robot Target as Robot Target", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string");
            pManager.Register_PlaneParam("Plane", "P", "Plane as Plane");
            pManager.RegisterParam(new Param_ConfigurationData(), "Configuration Data", "CD", "The robot configuration as Configuration Data");
            pManager.RegisterParam(new Param_ExternalJointPosition(), "External Joint Position", "EJ", "The positions of the external logical axes as an External Joint Position");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            RobotTarget target = null;

            // Catch the input data
            if (!DA.GetData(0, ref target)) { return; }

            if (target != null)
            {
                // Is Valid?
                if (!target.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Robot Target is not valid");
                }

                // Output
                DA.SetData(0, target.Name);
                DA.SetData(1, target.Plane);
                DA.SetData(2, target.ConfigurationData);
                DA.SetData(3, target.ExternalJointPosition);
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
            get { return Properties.Resources.DeconstructRobTarget_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("BD810C50-2530-4F3B-A693-B43BB49FDDF0"); }
        }
        #endregion
    }
}