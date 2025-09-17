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
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotComponents.ABB.Presets.Robots
{
    /// <summary>
    /// Represent the robot data of the CRB15000-10/1.52.
    /// </summary>
    public class CRB15000_10_152 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "CRB15000-10/1.52"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override double[] KinematicParameters => new double[] { 150, -110, -80, 0, 400, 707, 637, 101 };

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-270, 270),
            new Interval(-180, 180),
            new Interval(-225, 85),
            new Interval(-180, 180),
            new Interval(-180, 180),
            new Interval(-270, 270)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "CRB15000_10_152_link_0",
            "CRB15000_10_152_link_1",
            "CRB15000_10_152_link_2",
            "CRB15000_10_152_link_3",
            "CRB15000_10_152_link_4",
            "CRB15000_10_152_link_5",
            "CRB15000_10_152_link_6"
        };
        #endregion

        #region obsolete
        /// <summary>
        /// Returns a new CRB15000-10/1.52 Robot instance. 
        /// </summary>
        /// <param name="positionPlane"> The position and orientation of the Robot in world coordinate space. </param>
        /// <param name="tool"> The Robot Tool. </param>
        /// <param name="externalAxes"> The external axes attached to the Robot. </param>
        /// <returns> The Robot preset. </returns>
        [Obsolete("This method is OBSOLETE and will be removed in the future.", false)]
        public static Robot GetRobot(Plane positionPlane, RobotTool tool = null, IList<IExternalAxis> externalAxes = null)
        {
            string name = "CRB15000-10/1.52";
            List<Mesh> meshes = GetMeshes();
            List<Plane> axisPlanes = GetAxisPlanes();
            List<Interval> axisLimits = GetAxisLimits();
            Plane mountingFrame = GetToolMountingFrame();

            // Check Robot Tool data
            if (tool == null)
            {
                tool = new RobotTool();
            }

            // Make empty list with external axes if the value is null
            if (externalAxes == null)
            {
                externalAxes = new List<IExternalAxis>() { };
            }

            // Override the position plane when an external axis is coupled that moves the robot
            for (int i = 0; i < externalAxes.Count; i++)
            {
                if (externalAxes[i].MovesRobot == true)
                {
                    positionPlane = externalAxes[i].AttachmentPlane;
                    break;
                }
            }

            Robot robot = new Robot(name, meshes, axisPlanes, axisLimits, Plane.WorldXY, mountingFrame, tool, externalAxes);
            Transform trans = Transform.PlaneToPlane(Plane.WorldXY, positionPlane);
            robot.Transform(trans);

            return robot;
        }

        /// <summary>
        /// Returns the list with the base and link meshes of the robot in robot coordinate space.
        /// </summary>
        /// <returns> The list with robot meshes. </returns>
        [Obsolete("This method is OBSOLETE and will be removed in the future.", false)]
        public static List<Mesh> GetMeshes()
        {
            List<Mesh> meshes = new List<Mesh>
            {
                Mesh.FromJSON(Properties.Resources.CRB15000_10_152_link_0) as Mesh,
                Mesh.FromJSON(Properties.Resources.CRB15000_10_152_link_1) as Mesh,
                Mesh.FromJSON(Properties.Resources.CRB15000_10_152_link_2) as Mesh,
                Mesh.FromJSON(Properties.Resources.CRB15000_10_152_link_3) as Mesh,
                Mesh.FromJSON(Properties.Resources.CRB15000_10_152_link_4) as Mesh,
                Mesh.FromJSON(Properties.Resources.CRB15000_10_152_link_5) as Mesh,
                Mesh.FromJSON(Properties.Resources.CRB15000_10_152_link_6) as Mesh,
            };

            return meshes;
        }

        /// <summary>
        /// Returns the list with axis limits.
        /// </summary>
        /// <returns> The list with axis limits. </returns>
        [Obsolete("This method is OBSOLETE and will be removed in the future.", false)]
        public static List<Interval> GetAxisLimits()
        {
            List<Interval> axisLimits = new List<Interval>
            {
                new Interval(-270, 270),
                new Interval(-180, 180),
                new Interval(-225, 85),
                new Interval(-180, 180),
                new Interval(-180, 180),
                new Interval(-270, 270)
            };

            return axisLimits;
        }

        /// <summary>
        /// Returns the list with the axis planes in robot coordinate space. 
        /// </summary>
        /// <returns> Returns a list with planes. </returns>
        [Obsolete("This method is OBSOLETE and will be removed in the future.", false)]
        public static List<Plane> GetAxisPlanes()
        {
            Plane[] axisPlanes = Robot.GetAxisPlanesFromKinematicsParameters(Plane.WorldXY, 150, -110, -80, 0, 400, 707, 637, 101, out _);

            return axisPlanes.ToList();
        }

        /// <summary>
        /// Returns the tool mounting frame in robot coordinate space.
        /// </summary>
        /// <returns> The tool mounting frame. </returns>
        [Obsolete("This method is OBSOLETE and will be removed in the future.", false)]
        public static Plane GetToolMountingFrame()
        {
            Robot.GetAxisPlanesFromKinematicsParameters(Plane.WorldXY, 150, -110, -80, 0, 400, 707, 637, 101, out Plane mountingFrame);

            return mountingFrame;
        }
        #endregion
    }
}