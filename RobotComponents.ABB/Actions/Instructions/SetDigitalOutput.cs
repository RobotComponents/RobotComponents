// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Actions.Interfaces;

namespace RobotComponents.ABB.Actions.Instructions
{
    /// <summary>
    /// Represents a Set Digital Output instruction. 
    /// </summary>
    /// <remarks>
    /// This action is used to set the value (state) of a digital output signal.
    /// </remarks>
    [Serializable()]
    public class SetDigitalOutput : Action, IInstruction, ISerializable
    {
        #region fields
        private string _name;
        private double _delay;
        private bool _sync;
        private bool _value;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected SetDigitalOutput(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _name = (string)info.GetValue("Name", typeof(string));
            _delay = (double)info.GetValue("Delay", typeof(double));
            _sync = (bool)info.GetValue("Sync", typeof(bool));
            _value = (bool)info.GetValue("Value", typeof(bool));
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
            info.AddValue("Delay", _delay, typeof(double));
            info.AddValue("Sync", _sync, typeof(bool));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Value", _value, typeof(bool));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Set Digital Output class.
        /// </summary>
        public SetDigitalOutput()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Set Digital Output class.
        /// </summary>
        /// <param name="name"> The name of the Digital Output signal. </param>
        /// <param name="value"> Specifies whether the Digital Output is active. </param>
        public SetDigitalOutput(string name, bool value)
        {
            _delay = 0;
            _sync = false;
            _name = name;
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the Set Digital Output class by duplicating an existing Set Digital Output instance. 
        /// </summary>
        /// <param name="setDigitalOutput"> The Set Digital Output instance to duplicate. </param>
        public SetDigitalOutput(SetDigitalOutput setDigitalOutput)
        {
            _delay = setDigitalOutput.Delay;
            _sync = setDigitalOutput.Sync;
            _name = setDigitalOutput.Name;
            _value = setDigitalOutput.Value;
        }

        /// <summary>
        /// Returns an exact duplicate of this Set Digital Output instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Set Digital Output instance. 
        /// </returns>
        public SetDigitalOutput Duplicate()
        {
            return new SetDigitalOutput(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Set Digital Output instance as IInstruction.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Set Digital Output instance as an IInstruction. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new SetDigitalOutput(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Set Digital Output instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Set Digital Output instance as an Action. 
        /// </returns>
        public override Action DuplicateAction()
        {
            return new SetDigitalOutput(this);
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
                return "Empty Set Digital Output";
            }
            else if (!IsValid)
            {
                return "Invalid Set Digital Output";
            }
            else
            {
                return $"Set Digital Output ({_name}\\{_value})";
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// An empty string. 
        /// </returns>
        public override string ToRAPIDDeclaration(Robot robot)
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
        public override string ToRAPIDInstruction(Robot robot)
        {
            string result = $"SetDO ";

            // Sync and delay cannot be combined. Sync is leading. 
            result += _sync ? "\\Sync, " : (_delay > 0 ? $"\\SDelay:={_delay:0.###}, " : "");
            result += $"{_name}, ";
            result += _value ? "1;" : "0;";

            return result;
        }

        /// <summary>
        /// Creates declarations and instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDGenerator(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.ProgramInstructions.Add("    " + "    " + ToRAPIDInstruction(RAPIDGenerator.Robot));
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
                if (_name == null) { return false; }
                if (_name == "") { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the name of the digital output signal.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or set the delay the change for the amount of time given in seconds.
        /// </summary>
        /// <remarks>
        /// The maximum delay is 2000 seconds.
        /// </remarks>
        public double Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        /// <summary>
        /// Gets or sets the synchronization value.
        /// </summary>
        /// <remarks>
        /// If this argument is used then the program execution will wait 
        /// until the signal is physically set to the specified value.
        /// </remarks>
        public bool Sync
        {
            get { return _sync; }
            set { _sync = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the digital output is active.
        /// </summary>
        public bool Value
        {
            get { return _value; }
            set { _value = value; }
        }
        #endregion
    }
}