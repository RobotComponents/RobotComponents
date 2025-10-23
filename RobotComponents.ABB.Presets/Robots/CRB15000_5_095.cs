// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2021-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2021-2025)
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
    /// Represent the robot data of the CRB15000-5/0.95.
    /// </summary>
    public class CRB15000_5_095 : RobotPresetData
    {
        #region properties
        /// <summary>
        /// Gets the name of the Robot.
        /// </summary>
        public override string Name
        {
            get { return "CRB15000-5/0.95"; }
        }

        /// <summary>
        /// Gets the kinematics parameters.
        /// </summary>
        public override RobotKinematicParameters RobotKinematicParameters => new RobotKinematicParameters(0, -110, -80, 0, 265, 444, 470, 101);

        /// <summary>
        /// Gets the axis limits.
        /// </summary>
        public override List<Interval> AxisLimits => new List<Interval>
        {
            new Interval(-180, 180),
            new Interval(-180, 180),
            new Interval(-225, 85),
            new Interval(-180, 180),
            new Interval(-180, 180),
            new Interval(-270, 270)
        };

        /// <summary>
        /// Gets the name of the Mesh resources embedded in the assembly.
        /// </summary>
        public override string[] MeshResources => new[]
        {
            "CRB15000_5_095_link_0",
            "CRB15000_5_095_link_1",
            "CRB15000_5_095_link_2",
            "CRB15000_5_095_link_3",
            "CRB15000_5_095_link_4",
            "CRB15000_5_095_link_5",
            "CRB15000_5_095_link_6"
        };
        #endregion
    }
}