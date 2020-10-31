// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// RobotComponents Libs
using RobotComponents.Definitions;

namespace RobotComponents.Actions
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
            if (!this.IsValid)
            {
                return "Invalid Action";
            }
            else if (this is AbsoluteJointMovement absoluteJointMovement)
            {
                return absoluteJointMovement.ToString();
            }
            else if (this is AutoAxisConfig autoAxisConfig)
            {
                return autoAxisConfig.ToString();
            }
            else if (this is CodeLine codeLine)
            {
                return codeLine.ToString();
            }
            else if (this is Comment comment)
            {
                return comment.ToString();
            }
            else if (this is DigitalOutput digitalOutput)
            {
                return digitalOutput.ToString();
            }
            else if (this is ExternalJointPosition extJointPosition)
            {
                return extJointPosition.ToString();
            }
            else if (this is JointTarget jointTarget)
            {
                return jointTarget.ToString();
            }
            else if (this is Movement movement)
            {
                return movement.ToString();
            }
            else if (this is OverrideRobotTool overrideRobotTool)
            {
                return overrideRobotTool.ToString();
            }
            else if (this is RobotJointPosition robotJointPosition)
            {
                return robotJointPosition.ToString();
            }
            else if (this is SpeedData speedData)
            { 
                return speedData.ToString();
            }
            else if (this is RobotTarget robotTarget)
            {
                return robotTarget.ToString();
            }
            else if (this is WaitTime timer)
            {
                return timer.ToString();
            }
            else if (this is WaitDI waitDI)
            {
                return waitDI.ToString();
            }
            else
            {
                return "Action";
            }
        }

        /// <summary>
        /// Creates the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public abstract string ToRAPIDDeclaration(Robot robot);

        /// <summary>
        /// Creates the RAPID instruction code line of the this action. 
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