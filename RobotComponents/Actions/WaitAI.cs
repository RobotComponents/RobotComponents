// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.Enumerations;
using RobotComponents.Definitions;
using RobotComponents.Utils;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents a Wait for Analo Input instruction.
    /// This action is used to wait until a value of a analog input is set.
    /// </summary>
    [Serializable()]
    public class WaitAI : Action, IInstruction, ISerializable
    {
        #region fields
        private string _name; // The name of the analog input signal
        private double _value; // The desired state / value of the analog input signal
        private InequalitySymbol _inequalitySymbol; // Defines less than and greater than
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected WaitAI(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _name = (string)info.GetValue("Name", typeof(string));
            _value = (double)info.GetValue("Value", typeof(double));
            _inequalitySymbol = (InequalitySymbol)info.GetValue("Inequality Symbol", typeof(InequalitySymbol));
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
            info.AddValue("Inequality Symbol", _value, typeof(InequalitySymbol));
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
        public WaitAI(string name, double value, InequalitySymbol inequalitySymbol)
        {
            _name = name;
            _value = value;
            _inequalitySymbol = inequalitySymbol;
        }

        /// <summary>
        /// Initializes a new instance of the Wait AI class by duplicating an existing Wait AI instance. 
        /// </summary>
        /// <param name="WaitAI"> The Wait AI instance to duplicate. </param>
        public WaitAI(WaitAI WaitAI)
        {
            _name = WaitAI.Name;
            _value = WaitAI.Value;
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait AI instance.
        /// </summary>
        /// <returns> A deep copy of the Wait AI instance. </returns>
        public WaitAI Duplicate()
        {
            return new WaitAI(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait AI instance as IInstruction.
        /// </summary>
        /// <returns> A deep copy of the Wait AI instance as an IInstruction. </returns>
        public IInstruction DuplicateInstruction()
        {
            return new WaitAI(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Wait AI instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Wait AI instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new WaitAI(this);
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (_name == null)
            {
                return "Empty Wait for Analog Input";
            }
            if (!this.IsValid)
            {
                return "Invalid Wait for Analog Input";
            }
            else
            {
                return "Wait for Analog Input (" + _name + "\\" + _value.ToString() + ")";
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
            return "WaitAI " + _name + ", \\" + Enum.GetName(typeof(InequalitySymbol), _inequalitySymbol) + ", " + _value + ";";
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
            RAPIDGenerator.ProgramInstructions.Add("    " + "    " + this.ToRAPIDInstruction(RAPIDGenerator.Robot)); 
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
        #endregion
    }

}

