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
    /// Represent the robot data of the IRB6700-200/2.6.
    /// </summary>
    public class IRB6700_200_260 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB6700-200/2.6"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override RobotKinematicParameters RobotKinematicParameters => new RobotKinematicParameters(320, -200, -0, 0, 780, 1125, 1142.5, 200);

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-170, 170),
            new Interval(-65, 85),
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
            "IRB6700_200_260_link_0",
            "IRB6700_200_260_link_1",
            "IRB6700_200_260_link_2",
            "IRB6700_200_260_link_3",
            "IRB6700_200_260_link_4",
            "IRB6700_200_260_link_5",
            "IRB6700_200_260_link_6"
        };
        #endregion
    }
}
