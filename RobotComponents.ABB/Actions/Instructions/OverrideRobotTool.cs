﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Actions.Instructions
{
    /// <summary>
    /// Represents the Override Robot Tool action.
    /// </summary>
    /// <remarks>
    /// This action is used to set a new default Robot Tool from this action. 
    /// </remarks>
    [Serializable()]
    public class OverrideRobotTool : IAction, IInstruction, ISerializable
    {
        #region fields
        private RobotTool _robotTool;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected OverrideRobotTool(SerializationInfo info, StreamingContext context)
        {
            // Version version = (int)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _robotTool = (RobotTool)info.GetValue("Robot Tool", typeof(RobotTool));
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VersionNumbering.Version, typeof(Version));
            info.AddValue("Robot Tool", _robotTool, typeof(RobotTool));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Override Robot Tool class.
        /// </summary>
        public OverrideRobotTool()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Override Robot Tool class.
        /// </summary>
        /// <param name="robotTool"> The Robot Tool that should be set. </param>
        public OverrideRobotTool(RobotTool robotTool)
        {
            _robotTool = robotTool;
        }

        /// <summary>
        /// Initializes a new instance of the Override Robot Tool class by duplicating an existing Override Robot Tool instance. 
        /// </summary>
        /// <param name="overrideRobotTool"> The Override Robot Tool instance to duplicate. </param>
        public OverrideRobotTool(OverrideRobotTool overrideRobotTool)
        {
            _robotTool = overrideRobotTool.RobotTool.Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Override Robot Tool instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Override Robot Tool instance. 
        /// </returns>
        public OverrideRobotTool Duplicate()
        {
            return new OverrideRobotTool(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Override Robot Tool instance as IInstruction.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Override Robot Tool instance as an IInstruction. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new OverrideRobotTool(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Override Robot Tool instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Override Robot Tool instance as an Action. 
        /// </returns>
        public IAction DuplicateAction()
        {
            return new OverrideRobotTool(this);
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> 
        /// A string that represents the current object. 
        /// </returns>
        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid Override Robot Tool";
            }
            else
            {
                return $"Override Robot Tool ({_robotTool.Name})";
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// An empty string. 
        /// </returns>
        public string ToRAPIDDeclaration(Robot robot)
        {
            return string.Empty; // We don't write a comment between our declarations.
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        public string ToRAPIDInstruction(Robot robot)
        {
            robot.Tool = _robotTool;
            return $"! Default Robot Tool changed to {robot.Tool.Name}.";
        }

        /// <summary>
        /// Creates declarations and instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public void ToRAPIDGenerator(RAPIDGenerator RAPIDGenerator)
        {
            _robotTool.ToRAPIDGenerator(RAPIDGenerator);
            RAPIDGenerator.ProgramInstructions.Add("    " + "    " + ToRAPIDInstruction(RAPIDGenerator.Robot));
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (_robotTool == null) { return false; }
                if (_robotTool.IsValid == false) { return false; }
                if (_robotTool.Name == null) { return false; }
                if (_robotTool.Name == "") { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the Robot Tool.
        /// </summary>
        public RobotTool RobotTool
        {
            get { return _robotTool; }
            set { _robotTool = value; }
        }

        /// <summary>
        /// Gets the name of the Robot Tool.
        /// </summary>
        public string ToolName
        {
            get { return _robotTool.Name; }
        }
        #endregion
    }
}