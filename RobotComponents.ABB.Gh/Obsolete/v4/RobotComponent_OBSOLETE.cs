// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2018-2020 EDEK Uni Kassel
// Copyright (c) 2020-2025 Arjen Deetman
//
// Authors:
//   - Gabriel Rumph (2018-2020)
//   - Benedikt Wannemacher (2018-2020)
//   - Arjen Deetman (2019-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Components;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Robot component.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class RobotComponent_OBSOLETE : GH_RobotComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RobotComponent_OBSOLETE() : base("Robot", "Robot", "Definitions",
              "Defines a robot which is needed for code generation and simulation.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Robot Name as String", GH_ParamAccess.item, "New Robot");
            pManager.AddMeshParameter("Meshes", "M", "Robot Meshes as Mesh List", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Axis Planes", "AP", "Axis Planes as Plane List", GH_ParamAccess.list);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Interval List", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Position Plane", "PP", "Position Plane of the Robot as Plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Mounting Frame", "MF", "Mounting Frame as Plane", GH_ParamAccess.item);
            pManager.AddParameter(new Param_RobotTool(), "Robot Tool", "RT", "Robot Tool as Robot Tool Parameter", GH_ParamAccess.item);
            pManager.AddParameter(new Param_ExternalAxis(), "External Axis", "EA", "External Axis as External Axis Parameter", GH_ParamAccess.list);

            pManager[6].Optional = true;
            pManager[7].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Robot(), "Robot", "R", "Resulting Robot", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE, use the new Robot component instead. If you still use this compponent, the tool mounting frame input parameter is ignored. The tool mounting frame is calculated from the provided axis planes.");

            // Input variables
            string name = "default robot";
            List<Mesh> meshes = new List<Mesh>();
            List<Plane> axisPlanes = new List<Plane>();
            List<Interval> axisLimits = new List<Interval>();
            Plane userPositionPlane = Plane.WorldXY;
            Plane mountingFrame = Plane.Unset;
            RobotTool tool = null;
            List<IExternalAxis> externalAxes = new List<IExternalAxis>();

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetDataList(1, meshes)) { return; }
            if (!DA.GetDataList(2, axisPlanes)) { return; }
            if (!DA.GetDataList(3, axisLimits)) { return; }
            if (!DA.GetData(4, ref userPositionPlane)) { return; }
            if (!DA.GetData(5, ref mountingFrame)) { return; }
            if (!DA.GetData(6, ref tool)) { tool = new RobotTool(); }
            if (!DA.GetDataList(7, externalAxes)) { externalAxes = new List<IExternalAxis>() { }; }

            // Construct empty robot
            Robot robot = new Robot();

            // Position plane
            Plane positionPlane = new Plane(userPositionPlane);

            // Override the position plane when an external axis is coupled that moves the robot
            for (int i = 0; i < externalAxes.Count; i++)
            {
                if (externalAxes[i].MovesRobot == true)
                {
                    positionPlane = externalAxes[i].AttachmentPlane;
                    break;
                }
            }

            // Construct the robot
            try
            {
                robot = new Robot(name, meshes, axisPlanes, axisLimits, userPositionPlane, tool, externalAxes);
                Transform trans = Transform.PlaneToPlane(userPositionPlane, positionPlane);
                robot.Transform(trans);
            }
            catch (Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, ex.Message);
            }

            // Output
            DA.SetData(0, robot);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
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
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.RobotInfo_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D62D3E73-6D93-4E80-9892-591DBEA648BE"); }
        }
        #endregion
    }
}
