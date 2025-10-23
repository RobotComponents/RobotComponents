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
    /// Represent the robot data of the IRB1520ID-4/1.5.
    /// </summary>
    public class IRB1520ID_4_150 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB1520ID-4/1.5"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override RobotKinematicParameters RobotKinematicParameters => new RobotKinematicParameters(160, -200, -0, 0, 453, 590, 723, 200);

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-170, 170),
            new Interval(-90, 150),
            new Interval(-100, 80),
            new Interval(-155, 155),
            new Interval(-135, 135),
            new Interval(-200, 200)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB1520ID_4_150_link_0",
            "IRB1520ID_4_150_link_1",
            "IRB1520ID_4_150_link_2",
            "IRB1520ID_4_150_link_3",
            "IRB1520ID_4_150_link_4",
            "IRB1520ID_4_150_link_5",
            "IRB1520ID_4_150_link_6"
        };
        #endregion
    }
}
