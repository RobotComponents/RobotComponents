// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents a Wait for Digital Input instruction.
    /// This action is used to wait until a digital input is set.
    /// </summary>
    [Serializable()]
    public class WaitDI : Action, ISerializable
    {
        #region fields
        private string _DIName; // The name of the digital input signal
        private bool _value; // The desired state / value of the digtal input signal
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected WaitDI(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _DIName = (string)info.GetValue("Name", typeof(string));
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
            info.AddValue("Version", VersionNumbering.CurrentVersionAsInt, typeof(int));
            info.AddValue("Name", _DIName, typeof(string));
            info.AddValue("Value", _value, typeof(bool));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty WaitDI object.
        /// </summary>
        public WaitDI()
        {
        }

        /// <summary>
        /// Defines a WaitDI object. 
        /// </summary>
        /// <param name="DIName"> The name of the signal. </param>
        /// <param name="value"> The desired state / value of the digtal input signal. </param>
        public WaitDI(string DIName, bool value)
        {
            _DIName = DIName;
            Value = value;
        }

        /// <summary>
        /// Creates a new WaitDI by duplicating an existing WaitDI. 
        /// This creates a deep copy of the existing WaitDI. 
        /// </summary>
        /// <param name="waitDI"> The wait for digital input that should be duplicated. </param>
        public WaitDI(WaitDI waitDI)
        {
            _DIName = waitDI.DIName;
            _value = waitDI.Value;
        }

        /// <summary>
        /// Method to duplicate the WaitDI object.
        /// </summary>
        /// <returns> Returns a deep copy of the WaitDI object. </returns>
        public WaitDI Duplicate()
        {
            return new WaitDI(this);
        }

        /// <summary>
        /// A method to duplicate the WaitDI object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the WaitDI object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new WaitDI(this) as Action;
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
                return "Invalid Wait for Digital Input";
            }
            else
            {
                return "Wait for Digital Input (" + this.DIName + "\\" + this.Value.ToString() + ")";
            }
        }

        /// <summary>
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robot"> Defines the Robot were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            if (_value == true)
            {
                return "WaitDI " + _DIName + ", 1;";
            }
            else
            {
                return "WaitDI " + _DIName + ", 0;";
            }
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>s
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + this.ToRAPIDInstruction(RAPIDGenerator.Robot)); 
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
                if (DIName == null) { return false; }
                if (DIName == "") { return false; }
                return true; 
            }
        }

        /// <summary>
        /// Gets or sets the desired state of the digtal input signal.
        /// </summary>
        public bool Value 
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Gets or sets the name of the digital input signal.
        /// </summary>
        public string DIName 
        { 
            get { return _DIName; }
            set { _DIName = value; }
        }
        #endregion
    }

}

