// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.Actions
{
    /// <summary>
    /// Defines the interface for different target types
    /// </summary>
    public interface ITarget
    {
        #region properties
        /// <summary>
        /// Defines the variable name of the Target
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Defines the External Joint Position
        /// </summary>
        ExternalJointPosition ExternalJointPosition { get; set; }
        #endregion 
    }
}
