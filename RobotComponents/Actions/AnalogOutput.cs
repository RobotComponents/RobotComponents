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
    /// Represents a Set Analog Output instruction. 
    /// This action is used to set the value of an analog output signal.
    /// </summary>
    [Serializable()]
    public class AnalogOutput : Action, ISerializable
    {
        #region fields
        private string _name; // the name of the signal to be changed.
        private double _value; // the desired value of the signal
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected AnalogOutput(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
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
            info.AddValue("Version", VersionNumbering.CurrentVersionAsInt, typeof(int));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Value", _value, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Analog Output class.
        /// </summary>
        public AnalogOutput()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Analog Output class.
        /// </summary>
        /// <param name="name"> The name of the Analog Output signal. </param>
        /// <param name="value"> The desired value of the signal. </param>
        public AnalogOutput(string name, double value)
        {
            _name = name;
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the Analog Output class by duplicating an existing Analog Output instance. 
        /// </summary>
        /// <param name="analogOutput"> The Analog Output instance to duplicate. </param>
        public AnalogOutput(AnalogOutput analogOutput)
        {
            _name = analogOutput.Name;
            _value= analogOutput.Value;
        }

        /// <summary>
        /// Returns an exact duplicate of this Analog Output instance.
        /// </summary>
        /// <returns> A deep copy of the Analog Output instance. </returns>
        public AnalogOutput Duplicate()
        {
            return new AnalogOutput(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Analog Output instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Analog Output instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new AnalogOutput(this) as Action;
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
                return "Invalid Analog Output";
            }
            else
            {
                return "Analog  Output (" + this.Name + "\\" + this.Value.ToString() + ")";
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
            return "SetAO " + _name + ", " + _value.ToString() + ";";
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
        /// Gets or sets the name of the Analog Output signal.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the value of the signal.
        /// </summary>
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }
        #endregion
    }
}
