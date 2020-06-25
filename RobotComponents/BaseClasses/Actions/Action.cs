// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// RobotComponents Libs
using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Action class, abstract main class for all actions.
    /// </summary>
    public abstract class Action
    {
        #region fields

        #endregion

        #region constructors
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Action()
        {
        }

        /// <summary>
        /// A method to duplicate the object as an Action type
        /// </summary>
        /// <returns> Returns a deep copy of the Action object. </returns>
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
            else if (this is Movement movement)
            {
                return movement.ToString();
            }
            else if (this is OverrideRobotTool overrideRobotTool)
            {
                return overrideRobotTool.ToString();
            }
            else if (this is SpeedData speedData)
            { 
                return speedData.ToString();
            }
            else if (this is Target target)
            {
                return target.ToString();
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
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public abstract string ToRAPIDDeclaration(Robot robotInfo);

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public abstract string ToRAPIDInstruction(Robot robotInfo);

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public abstract void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator);

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public abstract void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator);
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the Action object is valid. 
        /// </summary>
        public abstract bool IsValid { get; }
        #endregion
    }
}