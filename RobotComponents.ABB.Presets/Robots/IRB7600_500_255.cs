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
    /// Represent the robot data of the IRB7600-500/2.55.
    /// </summary>
    public class IRB7600_500_255 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB7600-500/2.55"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override double[] KinematicParameters => new double[] { 410, -165, -0, 0, 780, 1075, 1056, 250 };

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-180, 180),
            new Interval(-60, 85),
            new Interval(-180, 60),
            new Interval(-300, 300),
            new Interval(-100, 100),
            new Interval(-360, 360)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB7600_500_255_link_0",
            "IRB7600_500_255_link_1",
            "IRB7600_500_255_link_2",
            "IRB7600_500_255_link_3",
            "IRB7600_500_255_link_4",
            "IRB7600_500_255_link_5",
            "IRB7600_500_255_link_6"
        };
        #endregion
    }
}
