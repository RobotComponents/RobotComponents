// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Actions.Generators;
using RobotComponents.ABB.Actions.Interfaces;

namespace RobotComponents.ABB.Actions
{
    /// <summary>
    /// Represents a base class for all robot actions (declarations and instructions).
    /// </summary>
    [Serializable()]
    public abstract class Action
    {
        #region fields

        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Action class. 
        /// </summary>
        public Action()
        {
        }

        /// <summary>
        /// Returns an exact duplicate of this Action.
        /// </summary>
        /// <returns> The exact copy of this Action. </returns>
        public abstract Action DuplicateAction();
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid Action";
            }
            else if (this is ActionGroup group)
            {
                return group.ToString();
            }
            else if (this is IDeclaration declaration)
            {
                return declaration.ToString();
            }
            else if (this is IInstruction instruction)
            {
                return instruction.ToString();
            }
            else if (this is IDynamic dynamic)
            {
                return dynamic.ToString();
            }
            else
            {
                return "Action";
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public abstract string ToRAPIDDeclaration(Robot robot);

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public abstract string ToRAPIDInstruction(Robot robot);

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public abstract void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator);

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public abstract void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator);
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public abstract bool IsValid { get; }
        #endregion
    }
}