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
// Rhino Libs
using Rhino.Geometry;

namespace RobotComponents.ABB.Presets.Robots
{
    /// <summary>
    /// Abstract base class for Robot preset data.
    /// </summary>
    public abstract class RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public abstract double[] KinematicParameters { get; }

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