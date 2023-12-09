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
    /// Represents a Pulse Digital Output instruction. 
    /// </summary>
    /// <remarks>
    /// This action is used to generate a pulse on a digital output signal.
    /// </remarks>
    [Serializable()]
    public class PulseDigitalOutput : Action, IInstruction, ISerializable
    {
        #region fields
        private bool _high = false;
        private double _length = 0.2;
        private string _name;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected PulseDigitalOutput(SerializationInfo info, StreamingContext context)
        {
            // // Version version = (int)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _high = (bool)info.GetValue("High", typeof(bool));
            _length = (double)info.GetValue("Length", typeof(double));
            _name = (string)info.GetValue("Name", typeof(string));
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
            info.AddValue("High", _name, typeof(bool));
            info.AddValue("Length", _name, typeof(double));
            info.AddValue("Name", _name, typeof(string));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Pulse Digital Output class.
        /// </summary>
        public PulseDigitalOutput()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Pulse Digital Output class.
        /// </summary>
        /// <param name="high"> Specifies that the signal value should always be set to high independently of its current state. </param>
        /// <param name="length"> The length of the pulse in seconds. </param>
        /// <param name="name"> The name of the Digital Output signal. </param>
        public PulseDigitalOutput(bool high, double length, string name)
        {
            _high = high;
            _length = length;
            _name = name;
        }

        /// <summary>
        /// Initializes a new instance of the Pulse Digital Output class by duplicating an existing Pulse Digital Output instance. 
        /// </summary>
        /// <param name="pulseDigitalOutput"> The Pulse Digital Output instance to duplicate. </param>
        public PulseDigitalOutput(PulseDigitalOutput pulseDigitalOutput)
        {
            _high = pulseDigitalOutput.High;
            _length = pulseDigitalOutput.Length;
            _name = pulseDigitalOutput.Name;
        }

        /// <summary>
        /// Returns an exact duplicate of this Pulse Digital Output instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Pulse Digital Output instance. 
        /// </returns>
        public PulseDigitalOutput Duplicate()
        {
            return new PulseDigitalOutput(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Pulse Digital Output instance as IInstruction.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Pulse Digital Output instance as an IInstruction. 
        /// </returns>
        public IInstruction DuplicateInstruction()
        {
            return new PulseDigitalOutput(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Pulse Digital Output instance as an Action. 
        /// </summary>
        /// <returns> 
        /// A deep copy of the Pulse Digital Output instance as an Action. 
        /// </returns>
        public override Action DuplicateAction()
        {
            return new PulseDigitalOutput(this);
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
                return "Empty Pulse Digital Output";
            }
            else if (!IsValid)
            {
                return "Invalid Pulse Digital Output";
            }
            else
            {
                return $"Pulse Digital Output ({_name})";
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
            if (_high == false)
            {
                return $"PulseDO \\PLength:={_length:#.###}, {_name};";
            }
            else
            {
                return $"PulseDO \\High, \\PLength:={_length:#.###}, {_name};";
            }
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
                if (_length < 0.001) { return false; }
                if (_length > 2000) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not that the signal value 
        /// should always be set to high (1) independently of its current state.
        /// </summary>
        public bool High
        {
            get { return _high; }
            set { _high = value; }
        }

        /// <summary>
        /// Gets or sets the length of the pulse in seconds.
        /// </summary>
        public double Length
        {
            get { return _length; }
            set { _length = value; }
        }

        /// <summary>
        /// Gets or sets the name of the digital output signal.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion
    }
}