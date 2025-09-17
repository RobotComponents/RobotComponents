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
    /// Represent the robot data of the IRB140-6/0.81.
    /// </summary>
    public class IRB140_6_081 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "IRB140-6/0.81"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override double[] KinematicParameters => new double[] { 70, -0, -0, 0, 352, 360, 380, 65 };

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-180, 180),
            new Interval(-90, 110),
            new Interval(-230, 50),
            new Interval(-200, 200),
            new Interval(-115, 115),
            new Interval(-400, 400)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "IRB140_6_081_link_0",
            "IRB140_6_081_link_1",
            "IRB140_6_081_link_2",
            "IRB140_6_081_link_3",
            "IRB140_6_081_link_4",
            "IRB140_6_081_link_5",
            "IRB140_6_081_link_6"
        };
        #endregion
    }
}
