// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2021-2024 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2021-2024)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Definitions;

namespace RobotComponents.ABB.Actions.Instructions
{
    /// <summary>
    /// Represents a Wait for Analog Input instruction.
    /// </summary>
    /// <remarks>
    /// This action is used to wait until a value of a analog input is set.
    /// </remarks>
    [Serializable()]
    public class WaitAI : IAction, IInstruction, ISerializable
    {
        #region fields
        private string _name;
        private double _value;
        private InequalitySymbol _inequalitySymbol;
        private double _maxTime;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected WaitAI(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _name = (string)info.GetValue("Name", typeof(string));
            _value = (double)info.GetValue("Value", typeof(double));
            _inequalitySymbol = (InequalitySymbol)info.GetValue("Inequality Symbol", typeof(InequalitySymbol));
            _maxTime = (double)info.GetValue("Max Time", typeof(double));
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
            info.AddValue("Inequality Symbol", _value, typeof(InequalitySymbol));
            info.AddValue("Max Time", _maxTime, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Wait AI class.
        /// </summary>
        public WaitAI()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Wait AI class.
        /// </summary>
        /// <param name="name"> The name of the signal. </param>
        /// <param name="value"> The desired value. </param>
        /// <param name="inequalitySymbol"> The inequality symbol (less than, greater than) </param>
        /// <param name="maxTime"> The maximum time to wait in seconds. </param>
        public WaitAI(string name, double value, InequalitySymbol inequalitySymbol, double maxTime = -1)
        {
            _name = name;
            _value = value;
            _inequalitySymbol = inequalitySymbol;
            _maxTime = maxTime;
        }

        /// <summary>
        /// Initializes a new instance of the Wait AI class by duplicating an existing Wait AI instance. 
        /// </summary>
        /// <param name="WaitAI"> The Wait AI instance to duplicate. </param>
        public WaitAI(WaitAI WaitAI)
        {
            _name = WaitAI.Name;
            _value = WaitAI.Value;
            _maxTime = WaitAI.MaxTime;
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait AI instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Wait AI instance. 
        /// </returns>
        public WaitAI Duplicate()
        {
            return new WaitAI(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait AI instance as IInstruction.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Wait AI instance as an IInstruction. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new WaitAI(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait AI instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Wait AI instance as an Action. 
        /// </returns>
        public IAction DuplicateAction()
        {
            return new WaitAI(this);
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
                return "Empty Wait for Analog Input";
            }
            if (!IsValid)
            {
                return "Invalid Wait for Analog Input";
            }
            else
            {
                return $"Wait for Analog Input ({_name}\\{_value})";
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
            return $"WaitAI {_name}, \\{Enum.GetName(typeof(InequalitySymbol), _inequalitySymbol)}, {_value}" +
                $"{(_maxTime > 0 ? $"\\MaxTime:={_maxTime:0.###}" : "")};";
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
        /// Gets or sets the desired state of the analog input signal.
        /// </summary>
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Gets or sets the name of the analog input signal.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the inequality symbol.
        /// </summary>
        public InequalitySymbol InequalitySymbol
        {
            get { return _inequalitySymbol; }
            set { _inequalitySymbol = value; }
        }

        /// <summary>
        /// Gets or sets te max. time to wait in seconds.
        /// </summary>
        /// <remarks>
        /// Set a negative value to wait forever (default is -1).
        /// </remarks>
        public double MaxTime
        {
            get { return _maxTime; }
            set { _maxTime = value; }
        }
        #endregion
    }
}