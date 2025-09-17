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
    /// Represent the robot data of the IRB6650S-200/3.0.
    /// </summary>
    public class IRB6650S_200_300 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB6650S-200/3.0"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override double[] KinematicParameters => new double[] { 600, -200, -0, 0, 630, 1280, 1142, 200 };

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-180, 180),
            new Interval(-40, 160),
            new Interval(-180, 70),
            new Interval(-300, 300),
            new Interval(-120, 120),
            new Interval(-360, 360)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB6650S_200_300_link_0",
            "IRB6650S_200_300_link_1",
            "IRB6650S_200_300_link_2",
            "IRB6650S_200_300_link_3",
            "IRB6650S_200_300_link_4",
            "IRB6650S_200_300_link_5",
            "IRB6650S_200_300_link_6"
        };
        #endregion
    }
}
