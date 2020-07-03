// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System lib
using System;
using System.Collections.Generic;
// RobotComponents Libs
using RobotComponents.Definitions;

// NOTE: The namespace is missing '.Declarations' to keep the access to the actions simple.
namespace RobotComponents.Actions
{
    /// <summary>
    /// Joint Target class, defines each individual axis position, for both the robot and the external axes.
    /// </summary>
    public class JointTarget : Action, ITarget
    {
        #region fields
        private string _name; // joint target variable name
        private RobotJointPosition _robJointPosition; // the position of the robot
        private ExternalJointPosition _extJointPosition; // the position of the external axes
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty Joint Target object.
        /// </summary>
        public JointTarget()
        {
        }

        /// <summary>
        /// Defines a joint target with an undefined external joint position
        /// </summary>
        /// <param name="name">Joint target name, must be unique.</param>
        /// <param name="robJointPosition">The rob joint position</param>
        public JointTarget(string name, RobotJointPosition robJointPosition)
        {
            _name = name;
            _robJointPosition = robJointPosition;
            _extJointPosition = new ExternalJointPosition();
        }


        /// <summary>
        /// Defines a joint target with an undefined external joint position
        /// </summary>
        /// <param name="name">Joint target name, must be unique.</param>
        /// <param name="axisValues">The rob joint position defined as a list with axis values</param>
        public JointTarget(string name, List<double> axisValues)
        {
            _name = name;
            _robJointPosition = new RobotJointPosition(axisValues);
            _extJointPosition = new ExternalJointPosition();
        }

        /// <summary>
        /// Defines a joint target
        /// </summary>
        /// <param name="name">Joint target name, must be unique.</param>
        /// <param name="robJointPosition">The robot joint position</param>
        /// <param name="extJointPosition">The external joint position</param>
        public JointTarget(string name, RobotJointPosition robJointPosition, ExternalJointPosition extJointPosition)
        {
            _name = name;
            _robJointPosition = robJointPosition;
            _extJointPosition = extJointPosition;
        }

        /// <summary>
        /// Defines a joint target
        /// </summary>
        /// <param name="name">Joint target name, must be unique.</param>
        /// <param name="internalAxisValues">The robot joint position as a list with axis values</param>
        /// <param name="externalAxisValues">The external joint position as a list wiht axis values</param>
        public JointTarget(string name, List<double> internalAxisValues, List<double> externalAxisValues)
        {
            _name = name;
            _robJointPosition = new RobotJointPosition(internalAxisValues);
            _extJointPosition = new ExternalJointPosition(externalAxisValues);
        }

        /// <summary>
        /// Creates a new joint target by duplicating an existing joint target. 
        /// This creates a deep copy of the existing joint target. 
        /// </summary>
        /// <param name="jointtarget"> The joint target that should be duplicated. </param>
        public JointTarget(JointTarget jointTarget)
        {
            _name = jointTarget.Name;
            _robJointPosition = jointTarget.RobotJointPosition.Duplicate();
            _extJointPosition = jointTarget.ExternalJointPosition.Duplicate();
        }

        /// <summary>
        /// Method to duplicate the Joint Target object.
        /// </summary>
        /// <returns>Returns a deep copy of the Joint Target object.</returns>
        public JointTarget Duplicate()
        {
            return new JointTarget(this);
        }

        /// <summary>
        /// A method to duplicate the Joint Target object to an ITarget object. 
        /// </summary>
        /// <returns> Returns a deep copy of the Joint Target object as an ITarget object. </returns>
        public ITarget DuplicateTarget()
        {
            return new JointTarget(this) as ITarget;
        }

        /// <summary>
        /// A method to duplicate the Joint Target object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the Joint Target object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new JointTarget(this) as Action;
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid Joint Target";
            }
            else
            {
                return "Joint Target (" + this.Name + ")";
            }
        }

        /// <summary>
        /// Cheks for the axis limits and returns a list with possible errors messages. 
        /// </summary>
        /// <param name="robot"> The robot info to check the axis values for. </param>
        /// <returns> Returns a list with error messages. </returns>
        public List<string> CheckForAxisLimits(Robot robot)
        {
            // Initiate list
            List<string> errors = new List<string>();

            // Check for internal axis values
            for (int i = 0; i < 6; i++)
            {
                if (robot.InternalAxisLimits[i].IncludesParameter(_robJointPosition[i], false) == false)
                {
                    errors.Add("Joint Target  " + _name + ": Internal axis value " + (i + 1).ToString() + " is not in range.");
                }
            }

            // Check for external axis values
            for (int i = 0; i < robot.ExternalAxis.Count; i++)
            {
                int logicNumber = (int)robot.ExternalAxis[i].AxisNumber;

                if (_extJointPosition[logicNumber] == 9e9)
                {
                    errors.Add("Joint Target " + _name + ": External axis value " + (i + 1).ToString() + " is not definied (9E9).");
                }

                else if (robot.ExternalAxis[i].AxisLimits.IncludesParameter(_extJointPosition[logicNumber], false) == false)
                {
                    errors.Add("Joint Target " + _name + ": External axis value " + (i + 1).ToString() + " is not in range.");
                }
            }

            return errors;
        }

        /// <summary>
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            string code = "CONST jointtarget ";
            code += _name;
            code += " := [";
            code += _robJointPosition.ToRAPIDDeclaration(robot);
            code += ", ";
            code += _extJointPosition.ToRAPIDDeclaration(robot);
            code += "];";

            return code;
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            if (!RAPIDGenerator.JointTargets.ContainsKey(_name))
            {
                RAPIDGenerator.JointTargets.Add(_name, this);
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t" + this.ToRAPIDDeclaration(RAPIDGenerator.Robot));
            }
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// Defines if the Joint Target object is valid. 
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Name == null) { return false; }
                if (Name == "") { return false; }
                if (RobotJointPosition == null) { return false; }
                if (RobotJointPosition.IsValid == false) { return false; }
                if (ExternalJointPosition == null) { return false; }
                if (ExternalJointPosition.IsValid == false) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Defines the joint target variable name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Defines the Robot Joint Position
        /// </summary>
        public RobotJointPosition RobotJointPosition
        {
            get { return _robJointPosition; }
            set { _robJointPosition = value; }
        }

        /// <summary>
        /// Defines the External Joint Position
        /// </summary>
        public ExternalJointPosition ExternalJointPosition
        {
            get { return _extJointPosition; }
            set { _extJointPosition = value; }
        }
        #endregion
    }

}
