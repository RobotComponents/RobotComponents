// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

namespace RobotComponents.Actions
{
    /// <summary>
    /// A boolean that indicates if the Target object is valid.
    /// </summary>
    public interface ITarget
    {
        #region constructors

        #endregion

        #region methods
        /// <summary>
        /// Method to duplicate the Target object as an ITarget object
        /// </summary>
        /// <returns>Returns a deep copy of the ITarget object. </returns>
        ITarget DuplicateTarget();

        #endregion

        #region properties
        /// <summary>
        /// Indicates if the Target object is valid. 
        /// </summary>
        bool IsValid { get; }

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
