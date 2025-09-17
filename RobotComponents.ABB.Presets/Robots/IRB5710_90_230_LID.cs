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
    /// Represent the robot data of the IRB5710-90/2.3-LID.
    /// </summary>
    public class IRB5710_90_230_LID : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB5710-90/2.3-LID"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override double[] KinematicParameters => new double[] { 280, -180, -0, 0, 670, 950, 1055, 333 };

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-170, 170),
            new Interval(-75, 145),
            new Interval(-160, 70),
            new Interval(-300, 300),
            new Interval(-120, 120),
            new Interval(-200, 200)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB5710_90_230_LID_link_0",
            "IRB5710_90_230_LID_link_1",
            "IRB5710_90_230_LID_link_2",
            "IRB5710_90_230_LID_link_3",
            "IRB5710_90_230_LID_link_4",
            "IRB5710_90_230_LID_link_5",
            "IRB5710_90_230_LID_link_6"
        };
        #endregion
    }
}
