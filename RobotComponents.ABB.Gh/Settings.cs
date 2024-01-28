// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System.Drawing;
// Rhino Libs
using Rhino.Display;

namespace RobotComponents.ABB.Gh
{
    /// <summary>
    /// Represents the class that contains the Robot Components Grasshopper settings.
    /// </summary>
    internal class Settings
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