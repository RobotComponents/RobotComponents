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
        /// 0.X.X ---> mature release
        /// X.0.X ---> minor release (for example new functions, new components...etc.)
        /// X.X.0 ---> bug fixes and small improvements
        /// </summary>
        public const string CurrentVersion = "1.0.0";

        /// <summary>
        /// Gets the current version of the Robot Components plugin as an int. 
        /// Typically used to check the version number inside the code.
        /// For internal use only. Not recommended to use. 
        /// Examples:
        /// before v0: "0.11.003" ---> remove dots ---> 11003
        /// after v0: "1.2.3" ---> major*10^6 + minor*10^3 + fix --->  1*10^6 + 2*10^3 + 3
        /// </summary>
        public const int CurrentVersionAsInt = 1*10^6 + 0*10^3 + 0;
    }
}
