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
            RobotPresetData data = GetRobotPresetData(preset);
            return data.GetRobot(positionPlane, tool, externalAxes);
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
