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
using System.Collections.Generic;
using System.Linq;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Presets.Robots
{
    /// <summary>
    /// Abstract base class for Robot preset data.
    /// </summary>
    public abstract class RobotPresetData
    {
        #region methods
        /// <summary>
        /// Returns a predefined robot preset. 
        /// </summary>
        /// <param name="positionPlane"> The position and orientation of the Robot in world coordinate space. </param>
        /// <param name="tool"> The Robot Tool. </param>
        /// <param name="externalAxes"> The external axes attached to the Robot. </param>
        /// <returns> The predefined robot instance. </returns>

        public virtual Robot GetRobot(Plane positionPlane, RobotTool tool = null, IList<IExternalAxis> externalAxes = null)
        {
            // Substract the meshes
            List<Mesh> meshes = this.MeshResources.Select(
                r => Mesh.FromJSON(Properties.Resources.ResourceManager.GetObject(r) as string) as Mesh).ToList();

            // Apply defaults if null
            tool = tool ?? new RobotTool();
            externalAxes = externalAxes ?? new List<IExternalAxis>();

            // Override the position plane when an external axis is coupled that moves the robot
            positionPlane = externalAxes.FirstOrDefault(axis => axis.MovesRobot)?.AttachmentPlane ?? positionPlane;

            // Create the robot preset
            Robot robot = new Robot(this.Name, meshes, this.RobotKinematicParameters, this.AxisLimits, Plane.WorldXY, tool, externalAxes);

            // Transform the robot to the defined position plane
            robot.Transform(Transform.PlaneToPlane(Plane.WorldXY, positionPlane));

            return robot;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public abstract RobotKinematicParameters RobotKinematicParameters { get; }

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public abstract List<Interval> AxisLimits { get; }

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public abstract string[] MeshResources { get; }
        #endregion
    }
}