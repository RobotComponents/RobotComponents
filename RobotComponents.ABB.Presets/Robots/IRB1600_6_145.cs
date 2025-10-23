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
    /// Represent the robot data of the IRB1600-6/1.45.
    /// </summary>
    public class IRB1600_6_145 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB1600-6/1.45"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override RobotKinematicParameters RobotKinematicParameters => new RobotKinematicParameters(150, -0, -0, 0, 486.5, 700, 600, 65);

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-180, 180),
            new Interval(-63, 110),
            new Interval(-235, 55),
            new Interval(-200, 200),
            new Interval(-115, 115),
            new Interval(-400, 400)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB1600_6_145_link_0",
            "IRB1600_6_145_link_1",
            "IRB1600_6_145_link_2",
            "IRB1600_6_145_link_3",
            "IRB1600_6_145_link_4",
            "IRB1600_6_145_link_5",
            "IRB1600_6_145_link_6"
        };
        #endregion
    }
}
