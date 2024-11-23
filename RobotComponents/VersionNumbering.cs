// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

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
        public const string CurrentVersion = "3.1.2";

        /// <summary>
        /// Gets the current version.
        /// </summary>
        /// <remarks>
        /// Has to be manually updated each time. 
        /// 0.x.x ---> MAJOR version when you make incompatible API changes
        /// x.0.x ---> MINOR version when you add functionality in a backwards compatible manner,
        /// x.x.0 ---> BUILD version when you make backwards compatible bug fixes
        /// </remarks>
        public static Version Version = new Version(3, 1, 2);
    }
}