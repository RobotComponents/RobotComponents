// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System lib
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Enumerations;
using RobotComponents.Utils;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents a Joint Target declaration. 
    /// This action is used to define each individual axis position, for both the robot and the external axes.
    /// </summary>
    [Serializable()]
    public class JointTarget : Action, ITarget, ISerializable
    {
        #region fields
        private ReferenceType _referenceType; // reference type
        private string _name; // joint target variable name
        private RobotJointPosition _robJointPosition; // the position of the robot
        private ExternalJointPosition _extJointPosition; // the position of the external axes
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected JointTarget(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _referenceType = (ReferenceType)info.GetValue("Reference Type", typeof(ReferenceType));
            _name = (string)info.GetValue("Name", typeof(string));
            _robJointPosition = (RobotJointPosition)info.GetValue("Robot Joint Position", typeof(RobotJointPosition));
            _extJointPosition = (ExternalJointPosition)info.GetValue("External Joint Position", typeof(ExternalJointPosition));
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VersionNumbering.CurrentVersionAsInt, typeof(int));
            info.AddValue("Reference Type", _referenceType, typeof(ReferenceType));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Robot Joint Position", _robJointPosition, typeof(RobotJointPosition));
            info.AddValue("External Joint Position", _extJointPosition, typeof(ExternalJointPosition));
        }
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
            _referenceType = ReferenceType.VAR;
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
            _referenceType = ReferenceType.VAR;
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
            _referenceType = ReferenceType.VAR;
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
            _referenceType = ReferenceType.VAR;
            _name = name;
            _robJointPosition = new RobotJointPosition(internalAxisValues);
            _extJointPosition = new ExternalJointPosition(externalAxisValues);
        }

        /// <summary>
        /// Creates a new joint target by duplicating an existing joint target. 
        /// This creates a deep copy of the existing joint target. 
        /// </summary>
        /// <param name="jointTarget"> The joint target that should be duplicated. </param>
        public JointTarget(JointTarget jointTarget)
        {
            _referenceType = jointTarget.ReferenceType;
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
        /// Checks both internal and external axis limits and returns a list with possible errors messages. 
        /// </summary>
        /// <param name="robot"> The robot info to check the axis values for. </param>
        /// <returns> Returns a list with error messages. </returns>
        public List<string> CheckAxisLimits(Robot robot)
        {
            List<string> errors = new List<string>();

            errors.AddRange(CheckInternalAxisLimits(robot));
            errors.AddRange(CheckExternalAxisLimits(robot));

            return errors;
        }

        /// <summary>
        /// Checks the internal axis limits and returns a list with possible errors messages. 
        /// </summary>
        /// <param name="robot"> The robot info to check the axis values for. </param>
        /// <returns> Returns a list with error messages. </returns>
        public List<string> CheckInternalAxisLimits(Robot robot)
        {
            // Initiate list
            List<string> errors = new List<string>();

            // Check internal axis values
            for (int i = 0; i < 6; i++)
            {
                if (robot.InternalAxisLimits[i].IncludesParameter(_robJointPosition[i], false) == false)
                {
                    errors.Add("Joint Target  " + _name + ": Internal axis value " + (i + 1).ToString() + " is not in range.");
                }
            }

            return errors;
        }

        /// <summary>
        /// Checks the external axis limits and returns a list with possible errors messages. 
        /// </summary>
        /// <param name="robot"> The robot info to check the axis values for. </param>
        /// <returns> Returns a list with error messages. </returns>
        public List<string> CheckExternalAxisLimits(Robot robot)
        {
            // Initiate list
            List<string> errors = new List<string>();

            // Check external axis values
            for (int i = 0; i < robot.ExternalAxis.Count; i++)
            {
                int logicNumber = robot.ExternalAxis[i].AxisNumber;

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
            string code = Enum.GetName(typeof(ReferenceType), _referenceType);
            code += " jointtarget ";
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
            if (!RAPIDGenerator.Targets.ContainsKey(_name))
            {
                RAPIDGenerator.Targets.Add(_name, this);
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
        /// Gets a value indicating whether the object is valid.
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
        /// Gets or sets the Reference Type. 
        /// </summary>
        public ReferenceType ReferenceType
        {
            get { return _referenceType; }
            set { _referenceType = value; }
        }

        /// <summary>
        /// Gets or sets the Joint Target variable name.
        /// Each Target variable name has to be unique.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the Robot Joint Position.
        /// </summary>
        public RobotJointPosition RobotJointPosition
        {
            get { return _robJointPosition; }
            set { _robJointPosition = value; }
        }

        /// <summary>
        /// Gets or sets the External Joint Position.
        /// </summary>
        public ExternalJointPosition ExternalJointPosition
        {
            get { return _extJointPosition; }
            set { _extJointPosition = value; }
        }
        #endregion
    }

}
