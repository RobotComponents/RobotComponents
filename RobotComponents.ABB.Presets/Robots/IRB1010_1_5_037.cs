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
    /// Represent the robot data of the IRB1010-1.5/0.37.
    /// </summary>
    public class IRB1010_1_5_037 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB1010-1.5/0.37"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override double[] KinematicParameters => new double[] { 0, -0, -0, 0, 190, 185, 185, 45 };

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-170, 170),
            new Interval(-75, 125),
            new Interval(-180, 50),
            new Interval(-170, 170),
            new Interval(-125, 125),
            new Interval(-242, 242)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB1010_1_5_037_link_0",
            "IRB1010_1_5_037_link_1",
            "IRB1010_1_5_037_link_2",
            "IRB1010_1_5_037_link_3",
            "IRB1010_1_5_037_link_4",
            "IRB1010_1_5_037_link_5",
            "IRB1010_1_5_037_link_6"
        };
        #endregion
    }
}
