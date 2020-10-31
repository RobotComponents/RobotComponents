// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// Robot Component Libs
using RobotComponents.Enumerations;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents the interface for different target types.
    /// </summary>
    public interface ITarget
    {
        #region constructors

        #endregion

        #region methods
        /// <summary>
        /// Returns an exact duplicate of this Target.
        /// </summary>
        /// <returns> The exact copy of this Target. </returns>
        ITarget DuplicateTarget();

        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets or sets the Reference Type.
        /// </summary>
        ReferenceType ReferenceType { get; set;}

        /// <summary>
        /// Gets or sets the Target variable name.
        /// Each Target variable name has to be unique.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the External Joint Position.
        /// </summary>
        ExternalJointPosition ExternalJointPosition { get; set; }
        #endregion 
    }
}
