// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// RobotComponents Libs
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Actions
{
    /// <summary>
    /// Represents the interfaace for all actions (declarations and instructions).
    /// </summary>
    public interface IAction
    {
        #region fields

        #endregion

        #region constructors
        /// <summary>
        /// Returns an exact duplicate of this Action.
        /// </summary>
        /// <returns> 
        /// The exact copy of this Action. 
        /// </returns>
        IAction DuplicateAction();
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
        #endregion
    }
}