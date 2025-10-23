// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2018-2020 EDEK Uni Kassel
// Copyright (c) 2020-2024 Arjen Deetman
//
// Authors:
//   - Gabriel Rumph (2018-2020)
//   - Benedikt Wannemacher (2018-2020)
//   - Arjen Deetman (2019-2024)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Actions.Instructions
{
    /// <summary>
    /// Represent the Wait Time instruction.
    /// </summary>
    /// <remarks>
    /// This action is used to wait a given amount of time between two actions.
    /// </remarks>
    [Serializable()]
    public class WaitTime : IAction, IInstruction, ISerializable
    {
        #region fields
        private double _duration;
        private bool _inPosition;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected WaitTime(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _duration = (double)info.GetValue("Duration", typeof(double));
            _inPosition = (bool)info.GetValue("In Position", typeof(bool));
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
            info.AddValue("Duration", _duration, typeof(double));
            info.AddValue("In Postion", _inPosition, typeof(bool));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Wait Time class.
        /// </summary>
        public WaitTime()
        {
        }

        /// <summary>
        /// Initializes an empty instance of the Wait Time class.
        /// </summary>
        /// <param name="duration"> The time, expressed in seconds, that program execution is to wait. </param>
        /// <param name="inPosition"> Specifies whether or not the mechanial units must have come to a standstill before the wait time starts. </param>
        public WaitTime(double duration, bool inPosition = false)
        {
            _duration = duration;
            _inPosition = inPosition;
        }

        /// <summary>
        /// Initializes a new instance of the Wait Time class by duplicating an existing Wait Time instance. 
        /// </summary>
        /// <param name="waitTime"> The Wait Time instance to duplicate. </param>
        public WaitTime(WaitTime waitTime)
        {
            _duration = waitTime.Duration;
            _inPosition = waitTime.InPosition;
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait Time instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Wait Time instance. 
        /// </returns>
        public WaitTime Duplicate()
        {
            return new WaitTime(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait Time instance as IInstruction.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Wait Time instance as an IInstruction. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new WaitTime(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait Time instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Wait Time instance as an Action. 
        /// </returns>
        public IAction DuplicateAction()
        {
            return new WaitTime(this);
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
                return "Invalid Wait Time";
            }
            else
            {
                return $"Wait Time ({_duration:0.###} sec.)";
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
            return string.Empty;
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
            return $"WaitTime {(_inPosition ? "\\InPos, " : "")}{_duration:0.###};";
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
                if (_duration < 0) { return false; }
                else { return true; }
            }
        }

        /// <summary>
        /// Gets or sets the time, expressed in seconds, that program execution is to wait. 
        /// </summary>
        /// <remarks>
        /// Min. value 0 seconds. Max. value no limit. Resolution 0.001 seconds.
        /// </remarks>
        public double Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the robot and external axes must have come to a standstill 
        /// before this program task starts waiting for other program tasks to reach its meeting point.
        /// </summary>
        public bool InPosition
        {
            get { return _inPosition; }
            set { _inPosition = value; }
        }
        #endregion
    }
}