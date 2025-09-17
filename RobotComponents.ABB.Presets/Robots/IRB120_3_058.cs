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
    /// Represent the robot data of the IRB120-3/0.58.
    /// </summary>
    public class IRB120_3_058 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB120-3/0.58"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override double[] KinematicParameters => new double[] { 0, -70, -0, 0, 290, 270, 302, 72 };

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-165, 165),
            new Interval(-110, 110),
            new Interval(-110, 70),
            new Interval(-160, 160),
            new Interval(-120, 120),
            new Interval(-400, 400)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB120_3_058_link_0",
            "IRB120_3_058_link_1",
            "IRB120_3_058_link_2",
            "IRB120_3_058_link_3",
            "IRB120_3_058_link_4",
            "IRB120_3_058_link_5",
            "IRB120_3_058_link_6"
        };
        #endregion
    }
}
