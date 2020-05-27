// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
// RobotComponents Libs
using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Wait for Digital Input. This class is used to make the code line comamand WaitDI which is 
    /// is used to wait until a digital input is set.
    /// </summary>
    public class WaitDI : Action
    {
        #region fields
        private string _DIName; // The name of the digital input signal
        private bool _value; // The desired state / value of the digtal input signal
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
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDDeclaration(RobotInfo robotInfo)
        {
            return string.Empty;
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDInstruction(RobotInfo robotInfo)
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
            RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + this.ToRAPIDInstruction(RAPIDGenerator.RobotInfo)); 
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the WaitDI object is valid.
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
        /// The desired state / value of the digtal input signal
        /// </summary>
        public bool Value 
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// The name of the digital input signal
        /// </summary>
        public string DIName 
        { 
            get { return _DIName; }
            set { _DIName = value; }
        }
        #endregion
    }

}

