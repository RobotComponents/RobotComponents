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
    /// Represent the robot data of the IRB1100-4/0.58.
    /// </summary>
    public class IRB1100_4_058 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB1100-4/0.58"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override RobotKinematicParameters RobotKinematicParameters => new RobotKinematicParameters(0, -10, -0, 0, 327, 280, 300, 64);

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-230, 230),
            new Interval(-115, 113),
            new Interval(-205, 55),
            new Interval(-230, 230),
            new Interval(-125, 120),
            new Interval(-400, 400)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB1100_4_058_link_0",
            "IRB1100_4_058_link_1",
            "IRB1100_4_058_link_2",
            "IRB1100_4_058_link_3",
            "IRB1100_4_058_link_4",
            "IRB1100_4_058_link_5",
            "IRB1100_4_058_link_6"
        };
        #endregion
    }
}
