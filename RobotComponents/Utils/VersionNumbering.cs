// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.Utils
{
    /// <summary>
    /// Stores the current version number of Robot Components
    /// </summary>
    public static class VersionNumbering
    {
        /// <summary>
        /// Gets the current version of the Robot Component Plugin. 
        /// Has to be manually updated each time. 
        /// 0.x.x ---> MAJOR version when you make incompatible API changes
        /// x.0.x ---> MINOR version when you add functionality in a backwards compatible manner,
        /// x.x.0 ---> PATCH version when you make backwards compatible bug fixes
        /// </summary>
        public const string CurrentVersion = "1.0.2";

        /// <summary>
        /// Gets the current version of the Robot Components plugin as an int. 
        /// Typically used to check the version number inside the code.
        /// For internal use only. Not recommended to use. 
        /// Used logic: major*10^6 + minor*10^3 + patch.
        /// </summary>
        public const int CurrentVersionAsInt = 1*1000000 + 0*1000 + 2; 
    }
}
