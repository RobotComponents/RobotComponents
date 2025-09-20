// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2024-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2024-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Gh.Parameters.Actions.Instructions;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.CodeGeneration
{
    /// <summary>
    /// RobotComponents Deconstruct Path Acceleration Limitation component.
    /// </summary>
    public class DeconstructPathAccelerationLimitationComponent : GH_RobotComponent
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructPathAccelerationLimitationComponent class.
        /// </summary>
        public DeconstructPathAccelerationLimitationComponent() : base("Deconstruct Path Acceleration Limitation", "DePaAcLi", "Deconstruct",
              "Deconstructs a Path Acceleration Limitation into its parameters.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_PathAccelerationLimitation(), "Path Acceleration Limitation", "PAL", "Path Acceleration Limitation as Path Acceleration Limitation", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_BooleanParam("Acceleration Limitation", "AL", "Specifies whether or not the acceleration is limited as a Boolean.");
            pManager.Register_DoubleParam("Acceleration Max", "AM", "The absolute value of the acceleration limitation in m/s^2 as a Number.");
            pManager.Register_BooleanParam("Deceleration Limitation", "DL", "Specifies whether or not the deceleration is limited as a Boolean.");
            pManager.Register_DoubleParam("Deceleration Max", "DM", "The absolute value of the deceleration limitation in m/s^2 as a Number.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            PathAccelerationLimitation pathAccelerationLimitation = null;

            // Catch the input data
            if (!DA.GetData(0, ref pathAccelerationLimitation)) { return; }

            if (pathAccelerationLimitation != null)
            {
                // Check if the object is valid
                if (!pathAccelerationLimitation.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Path Acceleration Limitation is invalid");
                }

                // Output
                DA.SetData(0, pathAccelerationLimitation.AccelerationLimitation);
                DA.SetData(1, pathAccelerationLimitation.AccelerationMax);
                DA.SetData(2, pathAccelerationLimitation.DecelerationLimitation);
                DA.SetData(3, pathAccelerationLimitation.DecelerationMax);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
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
            get { return Properties.Resources.DeconstructPathAccelerationLimitation_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6D125379-6D2D-4CF0-98D7-2C30AB4C639D"); }
        }
        #endregion
    }
}