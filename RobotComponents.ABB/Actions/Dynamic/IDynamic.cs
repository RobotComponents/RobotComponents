// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Robot Component Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;

namespace RobotComponents.ABB.Actions.Dynamic
{
    /// <summary>
    /// Represents the interface for different dynamic action types.
    /// </summary>
    public interface IDynamic
    {
        #region constructors
        /// <summary>
        /// Returns an exact duplicate of this dynamic action.
        /// </summary>
        /// <returns> 
        /// The exact copy of this dynamic action.
        /// </returns>
        IDynamic DuplicateDynamic();
        #endregion

        #region methods
        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        string ToRAPIDDeclaration(Robot robot);

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        string ToRAPIDInstruction(Robot robot);

        /// <summary>
        /// Creates declarations and instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        void ToRAPIDGenerator(RAPIDGenerator RAPIDGenerator);
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets or sets the Code Type.
        /// </summary>
        CodeType Type { get; set; }
        #endregion 
    }
}