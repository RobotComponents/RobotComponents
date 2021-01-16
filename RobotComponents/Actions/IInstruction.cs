// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// Robot Component Libs
using RobotComponents.Definitions;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents the interface for different instruction action types.
    /// </summary>
    public interface IInstruction
    {
        #region constructors

        #endregion

        #region methods
        /// <summary>
        /// Returns an exact duplicate of this Instruction.
        /// </summary>
        /// <returns> The exact copy of this Instruction. </returns>
        IInstruction DuplicateInstruction();

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        string ToRAPIDInstruction(Robot robot);

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
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
