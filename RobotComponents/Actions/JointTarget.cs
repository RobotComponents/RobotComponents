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
    public class JointTarget : Action, ITarget, IDeclaration, ISerializable
    {
        #region fields
        private ReferenceType _referenceType; // reference type
        private string _name; // joint target variable name
        private RobotJointPosition _robotJointPosition; // the position of the robot
        private ExternalJointPosition _externalJointPosition; // the position of the external logical axes
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
            _robotJointPosition = (RobotJointPosition)info.GetValue("Robot Joint Position", typeof(RobotJointPosition));
            _externalJointPosition = (ExternalJointPosition)info.GetValue("External Joint Position", typeof(ExternalJointPosition));
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
            info.AddValue("Robot Joint Position", _robotJointPosition, typeof(RobotJointPosition));
            info.AddValue("External Joint Position", _externalJointPosition, typeof(ExternalJointPosition));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Joint Target class.
        /// </summary>
        public JointTarget()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Joint Target class with an undefined External Joint Position.
        /// </summary>
        /// <param name="name"> The target name, must be unique. </param>
        /// <param name="robotJointPosition"> The robot joint position. </param>
        public JointTarget(string name, RobotJointPosition robotJointPosition)
        {
            _referenceType = ReferenceType.VAR;
            _name = name;
            _robotJointPosition = robotJointPosition;
            _externalJointPosition = new ExternalJointPosition();
        }


        /// <summary>
        /// Initializes a new instance of the Joint Target class.
        /// </summary>
        /// <param name="name"> The target name, must be unique.</param>
        /// <param name="robotJointPosition"> The Robot Joint Position</param>
        /// <param name="externalJointPosition"> The External Joint Position</param>
        public JointTarget(string name, RobotJointPosition robotJointPosition, ExternalJointPosition externalJointPosition)
        {
            _referenceType = ReferenceType.VAR;
            _name = name;
            _robotJointPosition = robotJointPosition;
            _externalJointPosition = externalJointPosition;
        }

        /// <summary>
        /// Initializes a new instance of the Joint Target class by duplicating an existing Joint Target instance. 
        /// </summary>
        /// <param name="jointTarget"> The Joint Target instance to duplicate. </param>
        public JointTarget(JointTarget jointTarget)
        {
            _referenceType = jointTarget.ReferenceType;
            _name = jointTarget.Name;
            _robotJointPosition = jointTarget.RobotJointPosition.Duplicate();
            _externalJointPosition = jointTarget.ExternalJointPosition.Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Joint Target instance.
        /// </summary>
        /// <returns> A deep copy of the Joint Target instance. </returns>
        public JointTarget Duplicate()
        {
            return new JointTarget(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Joint Target instance as an ITarget. 
        /// </summary>
        /// <returns> A deep copy of the Joint Target instance as an ITarget. </returns>
        public ITarget DuplicateTarget()
        {
            return new JointTarget(this) as ITarget;
        }

        /// <summary>
        /// Returns an exact duplicate of this Joint Target instance as an IDeclaration.
        /// </summary>
        /// <returns> A deep copy of the Joint Target instance as an IDeclaration. </returns>
        public IDeclaration DuplicateDeclaration()
        {
            return new JointTarget(this) as IDeclaration;
        }

        /// <summary>
        /// Returns an exact duplicate of this Joint Target instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Joint Target instance as an Action. </returns>
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
        /// <param name="robot"> The robot to check the axis values for. </param>
        /// <returns> The list with error messages. </returns>
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
        /// <param name="robot"> The robot to check the axis values for. </param>
        /// <returns> The list with error messages. </returns>
        public List<string> CheckInternalAxisLimits(Robot robot)
        {
            // Initiate list
            List<string> errors = new List<string>();

            // Check internal axis values
            for (int i = 0; i < 6; i++)
            {
                if (robot.InternalAxisLimits[i].IncludesParameter(_robotJointPosition[i], false) == false)
                {
                    errors.Add("Joint Target  " + _name + ": The position of robot axis " + (i + 1).ToString() + " is not in range.");
                }
            }

            return errors;
        }

        /// <summary>
        /// Checks the external axis limits and returns a list with possible errors messages. 
        /// </summary>
        /// <param name="robot"> The robot to check the axis values for. </param>
        /// <returns> The list with error messages. </returns>
        public List<string> CheckExternalAxisLimits(Robot robot)
        {
            // Initiate list
            List<string> errors = new List<string>();

            // Check external axis values
            for (int i = 0; i < robot.ExternalAxes.Count; i++)
            {
                int number = robot.ExternalAxes[i].AxisNumber;
                char logic = robot.ExternalAxes[i].AxisLogic;

                if (_externalJointPosition[number] == 9e9)
                {
                    errors.Add("Joint Target " + _name + ": The position of external logical axis " + logic + " is not definied (9E9).");
                }

                else if (robot.ExternalAxes[i].AxisLimits.IncludesParameter(_externalJointPosition[number], false) == false)
                {
                    errors.Add("Joint Target " + _name + ": The position of external logical axis " + logic + " is not in range.");
                }
            }

            return errors;
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            string code = Enum.GetName(typeof(ReferenceType), _referenceType);
            code += " jointtarget ";
            code += _name;
            code += " := [";
            code += _robotJointPosition.ToRAPID();
            code += ", ";
            code += _externalJointPosition.ToRAPID();
            code += "];";

            return code;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An empty string. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            if (!RAPIDGenerator.Targets.ContainsKey(_name))
            {
                RAPIDGenerator.Targets.Add(_name, this);
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t" + this.ToRAPIDDeclaration(RAPIDGenerator.Robot));
            }
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
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
            get { return _robotJointPosition; }
            set { _robotJointPosition = value; }
        }

        /// <summary>
        /// Gets or sets the External Joint Position.
        /// </summary>
        public ExternalJointPosition ExternalJointPosition
        {
            get { return _externalJointPosition; }
            set { _externalJointPosition = value; }
        }
        #endregion
    }

}
