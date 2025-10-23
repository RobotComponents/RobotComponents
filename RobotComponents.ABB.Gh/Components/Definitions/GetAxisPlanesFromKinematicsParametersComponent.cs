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
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;

namespace RobotComponents.ABB.Gh.Components.Definitions
{
    /// <summary>
    /// RobotComponents Get Axis Planes from Kinematics Parameters component.
    /// </summary>
    public class GetAxisPlanesFromKinematicsParametersComponent : GH_RobotComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public GetAxisPlanesFromKinematicsParametersComponent() : base("Get Axis Planes from Robot Kinematics Parameters", "KiPa2AxPl", "Definitions",
              "Gets the robot axis planes from the given robot kinematics parameters.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Position Plane", "PP", "Position Plane of the Robot as Plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddParameter(new Param_RobotKinematicParameters(), "Kinematic Parameters", "KP", "Robot Kinematic Parameters", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Axis Planes", "AP", "The Axis Planes as a list with Planes.", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Mounting Frame", "MF", "The tool mounting frame as a Plane.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            Plane plane = Plane.WorldXY;
            RobotKinematicParameters param = new RobotKinematicParameters();

            // Catch the input data
            if (!DA.GetData(0, ref plane)) { return; }
            if (!DA.GetData(1, ref param)) { return; }

            // Axis Planes
            Plane[] axisPlanes = param.GetAxisPlanes(plane, out Plane mountingFrame);

            // Output
            DA.SetDataList(0, axisPlanes);
            DA.SetData(1, mountingFrame);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary; }
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
            get { return Properties.Resources.GetAxisPlanesFromKinematicsParameters_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B35CC990-6885-4D62-BA18-C73200E901D4"); }
        }
        #endregion
    }
}