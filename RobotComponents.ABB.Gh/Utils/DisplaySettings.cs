// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2024 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2024)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System.Drawing;
// Rhino Libs
using Rhino.Display;

namespace RobotComponents.ABB.Gh.Utils
{
    /// <summary>
    /// Represents the class that contains the Robot Components Grasshopper settings.
    /// </summary>
    internal class DisplaySettings
    {
        #region fields
        private static readonly DisplayMaterial _displayMaterialInLimits = new DisplayMaterial(Color.FromArgb(225, 225, 225), 0.0);
        private static readonly DisplayMaterial _displayMaterialOutsideLimits = new DisplayMaterial(Color.FromArgb(150, 0, 0), 0.5);
        #endregion

        #region properties
        /// <summary>
        /// The display material of a mechanical unit with joint values in range. 
        /// </summary>
        public static DisplayMaterial DisplayMaterialInLimits
        {
            get { return _displayMaterialInLimits; }
        }

        /// <summary>
        /// The display material of a mechanical unit with joint values in range. 
        /// </summary>
        public static DisplayMaterial DisplayMaterialOutsideLimits
        {
            get { return _displayMaterialOutsideLimits; }
        }
        #endregion
    }
}