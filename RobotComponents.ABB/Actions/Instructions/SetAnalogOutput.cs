// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2020-2024 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2020-2024)
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
    /// Represents a Set Analog Output instruction. 
    /// </summary>
    /// <remarks>
    /// This action is used to set the value of an analog output signal.
    /// </remarks>
    [Serializable()]
    public class SetAnalogOutput : IAction, IInstruction, ISerializable
    {
        #region fields
        private string _name;
        private double _value;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected SetAnalogOutput(SerializationInfo info, StreamingContext context)
        {
            // // Version version = (int)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _name = (string)info.GetValue("Name", typeof(string));
            _value = (double)info.GetValue("Value", typeof(double));
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
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Value", _value, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Set Analog Output class.
        /// </summary>
        public SetAnalogOutput()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Set Analog Output class.
        /// </summary>
        /// <param name="name"> The name of the Analog Output signal. </param>
        /// <param name="value"> The desired value of the signal. </param>
        public SetAnalogOutput(string name, double value)
        {
            _name = name;
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the Set Analog Output class by duplicating an existing Set Analog Output instance. 
        /// </summary>
        /// <param name="setAnalogOutput"> The Set Analog Output instance to duplicate. </param>
        public SetAnalogOutput(SetAnalogOutput setAnalogOutput)
        {
            _name = setAnalogOutput.Name;
            _value = setAnalogOutput.Value;
        }

        /// <summary>
        /// Returns an exact duplicate of this Set Analog Output instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Set Analog Output instance. 
        /// </returns>
        public SetAnalogOutput Duplicate()
        {
            return new SetAnalogOutput(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Set Analog Output instance as IInstruction.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Set Analog Output instance as an IInstruction. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new SetAnalogOutput(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Set Analog Output instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Set Analog Output instance as an Action. 
        /// </returns>
        public IAction DuplicateAction()
        {
            return new SetAnalogOutput(this);
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
            if (_name == null)
            {
                return "Empty Set Analog Output";
            }
            else if (!IsValid)
            {
                return "Invalid Set Analog Output";
            }
            else
            {
                return $"Set Analog  Output ({_name}\\{_value})";
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
            return $"SetAO {_name}, {_value};";
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
                if (_name == null) { return false; }
                if (_name == "") { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the name of the analog output signal.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the value of the analog output signal.
        /// </summary>
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }
        #endregion
    }
}