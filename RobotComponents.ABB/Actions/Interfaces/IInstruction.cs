// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Robot Component Libs
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Actions.Interfaces
{
    /// <summary>
    /// Represents the interface for different instruction action types.
    /// </summary>
    public interface IInstruction
    {
        #region constructors
        /// <summary>
        /// Returns an exact duplicate of this Instruction.
        /// </summary>
        /// <returns> 
        /// The exact copy of this Instruction. 
        /// </returns>
        IInstruction DuplicateInstruction();
        #endregion

        #region methods
        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        string ToRAPIDInstruction(Robot robot);

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator);
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        bool IsValid { get; }
        #endregion 
    }
}
