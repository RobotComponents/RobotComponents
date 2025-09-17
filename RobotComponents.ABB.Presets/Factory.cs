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
using System.Collections.Generic;
using System.Linq;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Presets.Enumerations;
using RobotComponents.ABB.Presets.Robots;
using static RobotComponents.ABB.Presets.Utils.HelperMethods;

namespace RobotComponents.ABB.Presets
{
    /// <summary>
    /// Represents the presets factory. 
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Returns a predefined ABB Robot preset. 
        /// </summary>
        /// <param name="preset"> The Robot preset type. </param>
        /// <param name="positionPlane"> The position and orientation of the Robot in world coordinate space. </param>
        /// <param name="tool"> The Robot Tool. </param>
        /// <param name="externalAxes"> The external axes attached to the Robot. </param>
        /// <returns> The predefined robot instance. </returns>
        public static Robot GetRobotPreset(RobotPreset preset, Plane positionPlane, RobotTool tool = null, IList<IExternalAxis> externalAxes = null)
        {
            // Get the preset data
            RobotPresetData data = GetRobotPresetData(preset);

            // Substract the data
            List<Plane> axisPlanes = GetAxisPlanes(data.KinematicParameters, out Plane mountingFrame);
            List<Mesh> meshes = data.MeshResources.Select(
                r => Mesh.FromJSON(Properties.Resources.ResourceManager.GetObject(r) as string) as Mesh).ToList();

            // Apply defaults if null
            tool = tool ?? new RobotTool();
            externalAxes = externalAxes ?? new List<IExternalAxis>();

            // Override the position plane when an external axis is coupled that moves the robot
            positionPlane = externalAxes.FirstOrDefault(axis => axis.MovesRobot)?.AttachmentPlane ?? positionPlane;

            // Create the robot preset
            Robot robot = new Robot(data.Name, meshes, axisPlanes, data.AxisLimits, Plane.WorldXY, mountingFrame, tool, externalAxes);

            // Transform the robot to the defined position plane
            robot.Transform(Transform.PlaneToPlane(Plane.WorldXY, positionPlane));

            return robot;
        }

        /// <summary>
        /// Returns the list with the axis planes of the robot in robot coordinate space.
        /// </summary>
        /// <param name="kinematicParameters"> The kinematics parameters as an array </param>
        /// <param name="mountingFrame"> The tool mounting frame. </param>
        /// <returns> The internal axis planes of the robot. </returns>
        private static List<Plane> GetAxisPlanes(double[] kinematicParameters, out Plane mountingFrame)
        {
            return Robot.GetAxisPlanesFromKinematicsParameters(
                Plane.WorldXY,
                kinematicParameters[0],
                kinematicParameters[1],
                kinematicParameters[2],
                kinematicParameters[3],
                kinematicParameters[4],
                kinematicParameters[5],
                kinematicParameters[6],
                kinematicParameters[7],
                out mountingFrame).ToList();
        }

        /// <summary>
        /// Returns the data of the defined Robot preset type.
        /// </summary>
        /// <param name="preset"> The robot preset enum. </param>
        /// <returns> The defined robot preset data. </returns>
        private static RobotPresetData GetRobotPresetData(RobotPreset preset)
        {
            // Build the expected class name from enum
            string typeName = $"RobotComponents.ABB.Presets.Robots.{preset}";

            // Try to get the type from the assembly
            Type presetType = Type.GetType(typeName);
            presetType = presetType ?? throw new InvalidOperationException($"No Robot Preset Data class found for {GetRobotNameFromPresetName(preset)}.");

            // Create an instance (assumes parameterless constructor)
            object instance = Activator.CreateInstance(presetType);

            return instance as RobotPresetData ?? throw new InvalidOperationException($"{typeName} does not inherit RobotPresetData.");
        }
    }
}
