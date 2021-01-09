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
        /// 0.XX.XXX ---> mature release
        /// X.00.XXX ---> minor release (for example new functions, new components...etc.)
        /// X.XX.000 ---> bug fixes and small improvements
        /// </summary>
        public const string CurrentVersion = "0.14.000";
      
        /// <summary>
        /// Gets the current version of the Robot Components plugin as an int. 
        /// Has to be manually updated each time. 
        /// Remove the points and the zeros in front of the number. 
        /// Examples:
        /// "0.11.003" ----> 11003
        /// "1.01.000" ----> 101000
        /// </summary>
        public const int CurrentVersionAsInt = 14000;
    }
}
