// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2018-2020 EDEK Uni Kassel
// Copyright (c) 2023 Arjen Deetman
//
// Authors:
//   - Gabriel Rumph (2018-2020)
//   - Benedikt Wannemacher (2018-2020)
//   - Arjen Deetman (2023)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;

namespace RobotComponents
{
    /// <summary>
    /// Represents a class that stores the current version number of Robot Components.
    /// </summary>
    public static class VersionNumbering
    {
        /// <summary>
        /// Gets the current version as a string.
        /// </summary>
        /// <remarks>
        /// Has to be manually updated each time. 
        /// 0.x.x ---> MAJOR version when you make incompatible API changes
        /// x.0.x ---> MINOR version when you add functionality in a backwards compatible manner,
        /// x.x.0 ---> BUILD version when you make backwards compatible bug fixes
        /// </remarks>
        public const string CurrentVersion = "4.0.1";

        /// <summary>
        /// Gets the current version.
        /// </summary>
        /// <remarks>
        /// Has to be manually updated each time. 
        /// 0.x.x ---> MAJOR version when you make incompatible API changes
        /// x.0.x ---> MINOR version when you add functionality in a backwards compatible manner,
        /// x.x.0 ---> BUILD version when you make backwards compatible bug fixes
        /// </remarks>
        public static Version Version = new Version(4, 0, 1);
    }
}