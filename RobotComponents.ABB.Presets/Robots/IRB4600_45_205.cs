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
    /// Represent the robot data of the IRB4600-45/2.05.
    /// </summary>
    public class IRB4600_45_205 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB4600-45/2.05"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override RobotKinematicParameters RobotKinematicParameters => new RobotKinematicParameters(175, -175, -0, 0, 495, 900, 960, 135);

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-180, 180),
            new Interval(-90, 150),
            new Interval(-180, 75),
            new Interval(-400, 400),
            new Interval(-125, 120),
            new Interval(-400, 400)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB4600_45_205_link_0",
            "IRB4600_45_205_link_1",
            "IRB4600_45_205_link_2",
            "IRB4600_45_205_link_3",
            "IRB4600_45_205_link_4",
            "IRB4600_45_205_link_5",
            "IRB4600_45_205_link_6"
        };
        #endregion
    }
}
