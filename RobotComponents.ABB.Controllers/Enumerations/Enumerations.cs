// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2022-2024 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2022-2024)
//
// For license details, see the LICENSE file in the project root.

namespace RobotComponents.ABB.Controllers.Enumerations
{
    /// <summary>
    /// Defines the coordinate system.
    /// </summary>
    public enum CoordinateSystemType : int
    {
        /// <summary>
        /// World coordinate system
        /// </summary>
        World = 1,

        /// <summary>
        /// Base coordinate system.
        /// </summary>
        Base = 2,

        /// <summary>
        /// Workobject coordite system.
        /// </summary>
        Workbobject = 4
    }
}
