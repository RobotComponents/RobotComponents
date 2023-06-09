// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

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
        public const string CurrentVersion = "2.0.5";

        /// <summary>
        /// Gets the current version as an int. 
        /// </summary>
        /// <remarks>
        /// Typically used to check the version number inside the code.
        /// For internal use only. Not recommended to use. 
        /// Used logic: major*10^6 + minor*10^3 + build.
        /// </remarks>
        public const int CurrentVersionAsInt = 2 * 1000000 + 0 * 1000 + 5;
    }
}
