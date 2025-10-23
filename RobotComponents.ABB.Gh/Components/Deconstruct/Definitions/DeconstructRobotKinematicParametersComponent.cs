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
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Properties;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.Definitions
{
    /// <summary>
    /// RobotComponents Deconstruct Robot Kinematics Parameters component.
    /// </summary>
    public class DeconstructRobotKinematicParametersComponent : GH_RobotComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public DeconstructRobotKinematicParametersComponent() : base("Deconstruct Robot Kinematic Parameters", "DeRoKiPa", "Deconstruct",
              "Deconstructs a Robot Kinematic Parameters component into its parameters.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_RobotKinematicParameters(), "Kinematic Parameters", "KP", "Robot Kinematic Parameters", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("A1", "A1", "The shoulder offset.", GH_ParamAccess.item);
            pManager.AddNumberParameter("A2", "A2", "The elbow offset.", GH_ParamAccess.item);
            pManager.AddNumberParameter("A3", "A3", "The wrist offset.", GH_ParamAccess.item);
            pManager.AddNumberParameter("B", "B", "The lateral offset.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C1", "C1", "The first link length.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C2", "C2", "The second link length.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C3", "C3", "The third link length.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C4", "C4", "The fourth link length", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            RobotKinematicParameters param = new RobotKinematicParameters();

            // Catch the input data
            if (!DA.GetData(0, ref param)) { return; }

            if (param != null)
            {
                // Check if the input is valid
                if (!param.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Robot Kinematic Parameters instnace is invalid");
                }

                // Output
                DA.SetData(0, param.A1);
                DA.SetData(1, param.A2);
                DA.SetData(2, param.A3);
                DA.SetData(3, param.B);
                DA.SetData(4, param.C1);
                DA.SetData(5, param.C2);
                DA.SetData(6, param.C3);
                DA.SetData(7, param.C4);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Resources.DeconstructRobotKinematicParameters_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9812D961-1C6D-48E8-BBCE-A9F58D7DF03D"); }
        }
        #endregion
    }
}