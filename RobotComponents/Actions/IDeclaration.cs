// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// Robot Component Libs
using RobotComponents.Definitions;
using RobotComponents.Enumerations;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents the interface for different declaration action types.
    /// </summary>
    public interface IDeclaration
    {
        #region constructors

        #endregion

        #region methods
        /// <summary>
        /// Returns an exact duplicate of this Declaration.
        /// </summary>
        /// <returns> The exact copy of this Declaration. </returns>
        IDeclaration DuplicateDeclaration();

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        string ToRAPIDDeclaration(Robot robot);

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator);
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets or sets the Reference Type. 
        /// </summary>
        ReferenceType ReferenceType { get; set; }

        /// <summary>
        /// Gets or sets the variable name of the declaration.
        /// </summary>
        string Name { get; set; }
        #endregion 
    }
}
