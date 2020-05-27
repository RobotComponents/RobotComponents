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
    /// Digital Output class. Is used to change the value of a digital output signal.
    /// </summary>
    public class DigitalOutput : Action
    {
        #region fields
        private string _name; // the name of the signal to be changed.
        private bool _isActive; // the desired value of the signal 0 or 1.
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty DigitalOutput object.
        /// </summary>
        public DigitalOutput()
        {
        }

        /// <summary>
        /// Defines a digital ouput signal and the desired value / state.
        /// </summary>
        /// <param name="Name">The name of the digital output signal to be changed.</param>
        /// <param name="IsActive">The desired value / stage of the digital output signal 0 (false) or 1 (true).</param>
        public DigitalOutput(string Name, bool IsActive)
        {
            _name = Name;
            _isActive = IsActive;
        }

        /// <summary>
        /// Creates a new digital output by duplicating an existing digital output. 
        /// This creates a deep copy of the existing digital output. 
        /// </summary>
        /// <param name="digitalOutput"> The digital output that should be duplicated. </param>
        public DigitalOutput(DigitalOutput digitalOutput)
        {
            _name = digitalOutput.Name;
            _isActive = digitalOutput.IsActive;
        }

        /// <summary>
        /// A method to duplicate the DigitalOutput object.
        /// </summary>
        /// <returns> Returns a deep copy of the DigitalOutput object. </returns>
        public DigitalOutput Duplicate()
        {
            return new DigitalOutput(this);
        }

        /// <summary>
        /// A method to duplicate the DigitalOutput object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the DigitalOutput object as an Action object. </returns>
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
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + this.ToRAPIDInstruction(RAPIDGenerator.RobotInfo));
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the DigitalOutput object is valid. 
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
        /// The name of the digital output signal to be changed.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The desired value / stage of the digital output signal 0 (false) or 1 (true).
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        #endregion
    }
}
