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
    /// Represent the robot data of the IRB1200-7/0.7.
    /// </summary>
    public class IRB1200_7_070 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB1200-7/0.7"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override double[] KinematicParameters => new double[] { 0, -42, -0, 0, 399.1, 350, 351, 82 };

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-170, 170),
            new Interval(-100, 135),
            new Interval(-200, 70),
            new Interval(-270, 270),
            new Interval(-130, 130),
            new Interval(-360, 360)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB1200_7_070_link_0",
            "IRB1200_7_070_link_1",
            "IRB1200_7_070_link_2",
            "IRB1200_7_070_link_3",
            "IRB1200_7_070_link_4",
            "IRB1200_7_070_link_5",
            "IRB1200_7_070_link_6"
        };
        #endregion
    }
}
