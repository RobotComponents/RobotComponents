// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2025)
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

namespace RobotComponents.ABB.Gh.Components.Definitions
{
    /// <summary>
    /// RobotComponents Robot Kinematic Parameters component.
    /// </summary>
    public class RobotKinematicParametersComponent : GH_RobotComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RobotKinematicParametersComponent() : base("Robot Kinematic Parameters", "RoKiPa", "Definitions",
              "Defines the robot kinematic parameters.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("A1", "A1", "The shoulder offset as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("A2", "A2", "The elbow offset as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("A3", "A3", "The wrist offset as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("B", "B", "The lateral offset as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C1", "C1", "The first link length as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C2", "C2", "The second link length as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C3", "C3", "The third link length as a Number.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C4", "C4", "The fourth link length as a Number.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_RobotKinematicParameters(), "Kinematic Parameters", "KP", "Robot Kinematic Parameter", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            double a1 = 0;
            double a2 = 0;
            double a3 = 0;
            double b = 0;
            double c1 = 0;
            double c2 = 0;
            double c3 = 0;
            double c4 = 0;

            // Catch the input data
            if (!DA.GetData(0, ref a1)) { return; }
            if (!DA.GetData(1, ref a2)) { return; }
            if (!DA.GetData(2, ref a3)) { return; }
            if (!DA.GetData(3, ref b)) { return; }
            if (!DA.GetData(4, ref c1)) { return; }
            if (!DA.GetData(5, ref c2)) { return; }
            if (!DA.GetData(6, ref c3)) { return; }
            if (!DA.GetData(7, ref c4)) { return; }

            // Axis Planes
            RobotKinematicParameters param = new RobotKinematicParameters(a1, a2, a3, b, c1, c2, c3, c4);

            // Output
            DA.SetData(0, param);
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
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Resources.RobotKinematicParameters_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4B5C6AB4-5481-4EEB-865D-3960D8250FAF"); }
        }
        #endregion
    }
}