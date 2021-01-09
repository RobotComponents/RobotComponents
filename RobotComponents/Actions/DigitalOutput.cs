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
    /// Represents a Set Digital Output instruction. 
    /// This action is used to set the value (state) of a digital output signal.
    /// </summary>
    [Serializable()]
    public class DigitalOutput : Action, IInstruction, ISerializable
    {
        #region fields
        private string _name; // the name of the signal to be changed.
        private bool _isActive; // the desired value of the signal 0 or 1.
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected DigitalOutput(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _name = (string)info.GetValue("Name", typeof(string));
            _isActive = (bool)info.GetValue("Is Active", typeof(bool));
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
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Is Active", _isActive, typeof(bool));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Digital Output class.
        /// </summary>
        public DigitalOutput()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Digital Output class.
        /// </summary>
        /// <param name="Name"> The name of the Digital Output signal. </param>
        /// <param name="IsActive"> Specifies whether the Digital Output is active. </param>
        public DigitalOutput(string Name, bool IsActive)
        {
            _name = Name;
            _isActive = IsActive;
        }

        /// <summary>
        /// Initializes a new instance of the Digital Output class by duplicating an existing Digital Output instance. 
        /// </summary>
        /// <param name="digitalOutput"> The Digital Output instance to duplicate. </param>
        public DigitalOutput(DigitalOutput digitalOutput)
        {
            _name = digitalOutput.Name;
            _isActive = digitalOutput.IsActive;
        }

        /// <summary>
        /// Returns an exact duplicate of this Digital Output instance.
        /// </summary>
        /// <returns> A deep copy of the Digital Output instance. </returns>
        public DigitalOutput Duplicate()
        {
            return new DigitalOutput(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Digital Output instance as IInstruction.
        /// </summary>
        /// <returns> A deep copy of the Digital Output instance as an IInstruction. </returns>
        public IInstruction DuplicateInstruction()
        {
            return new DigitalOutput(this) as IInstruction;
        }

        /// <summary>
        /// Returns an exact duplicate of this Digital Output instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Digital Output instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new DigitalOutput(this) as Action;
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
                return "Invalid Digital Output";
            }
            else
            {
                return "Digital Output (" + this.Name + "\\" + this.IsActive.ToString() + ")";
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
            if (_isActive == true)
            {
                return "SetDO " + _name + ", 1;";
            }
            else
            {
                return "SetDO " + _name + ", 0;";
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
                if (Name == null) { return false; }
                if (Name == "") { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the name of the Digital Output signal.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Digital Output is active.
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        #endregion
    }
}
