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
    public class WaitDI : Action, IInstruction, ISerializable
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
        /// Initializes an empty instance of the Wait DI class.
        /// </summary>
        public WaitDI()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Wait DI class.
        /// </summary>
        /// <param name="DIName"> The name of the signal. </param>
        /// <param name="value"> Specifies whether the Digital Input is enabled.</param>
        public WaitDI(string DIName, bool value)
        {
            _DIName = DIName;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the Wait DI class by duplicating an existing Wait DI instance. 
        /// </summary>
        /// <param name="waitDI"> The Wait DI instance to duplicate. </param>
        public WaitDI(WaitDI waitDI)
        {
            _DIName = waitDI.DIName;
            _value = waitDI.Value;
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait DI instance.
        /// </summary>
        /// <returns> A deep copy of the Wait DI instance. </returns>
        public WaitDI Duplicate()
        {
            return new WaitDI(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait DI instance as IInstruction.
        /// </summary>
        /// <returns> A deep copy of the Wait DI instance as an IInstruction. </returns>
        public IInstruction DuplicateInstruction()
        {
            return new WaitDI(this) as IInstruction;
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait DI instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Wait Di instance as an Action. </returns>
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
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> An empty string. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
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
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + this.ToRAPIDInstruction(RAPIDGenerator.Robot)); 
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

