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
// Robot Components Libs
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Presets.Robots
{
    /// <summary>
    /// Represent the robot data of the IRB5720-125/3.0.
    /// </summary>
    public class IRB5720_125_300 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB5720-125/3.0"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override RobotKinematicParameters RobotKinematicParameters => new RobotKinematicParameters(320, -200, -0, 0, 670, 1120, 1547.5, 167);

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-170, 170),
            new Interval(-75, 145),
            new Interval(-180, 70),
            new Interval(-300, 300),
            new Interval(-130, 130),
            new Interval(-360, 360)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB5720_125_300_link_0",
            "IRB5720_125_300_link_1",
            "IRB5720_125_300_link_2",
            "IRB5720_125_300_link_3",
            "IRB5720_125_300_link_4",
            "IRB5720_125_300_link_5",
            "IRB5720_125_300_link_6"
        };
        #endregion
    }
}
