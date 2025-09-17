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
    /// Represent the robot data of the IRB2600ID-15/1.85.
    /// </summary>
    public class IRB2600ID_15_185 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB2600ID-15/1.85"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override double[] KinematicParameters => new double[] { 150, -150, -0, 0, 445, 900, 786, 135 };

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-180, 180),
            new Interval(-95, 155),
            new Interval(-180, 75),
            new Interval(-175, 175),
            new Interval(-120, 120),
            new Interval(-400, 400)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB2600ID_15_185_link_0",
            "IRB2600ID_15_185_link_1",
            "IRB2600ID_15_185_link_2",
            "IRB2600ID_15_185_link_3",
            "IRB2600ID_15_185_link_4",
            "IRB2600ID_15_185_link_5",
            "IRB2600ID_15_185_link_6"
        };
        #endregion
    }
}
