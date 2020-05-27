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
    /// Code Line class, defines a CodeLine in RAPID Code.
    /// </summary>
    public class CodeLine : Action
    {
        #region fields
        private string _code; // the code line as a string
        private int _type;  // the code line type as int value -> 0 for instruction, 1 for declaration
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty CodeLine object.
        /// </summary>
        public CodeLine()
        {
        }

        /// <summary>
        /// Defines a RAPID code line.
        /// </summary>
        /// <param name="code">The code line as a text string.</param>
        public CodeLine(string code)
        {
            _code = code;
            _type = 0;
        }

        /// <summary>
        /// Defines a RAPID code line.
        /// </summary>
        /// <param name="code">The code line as a text string.</param>
        /// <param name="type">The code type as int value. Use 0 for an instructions and 1 for a declarations.</param>
        public CodeLine(string code, int type)
        {
            _code = code;
            SetCodeLineType(type);
        }

        /// <summary>
        /// Creates a new code line by duplicating an existing code line. 
        /// This creates a deep copy of the existing code line. 
        /// </summary>
        /// <param name="codeLine"> The code line that should be duplicated. </param>
        public CodeLine(CodeLine codeLine)
        {
            _code = codeLine.Code;
            _type = codeLine.Type;
        }

        /// <summary>
        /// A method to duplicate the CodeLine object.
        /// </summary>
        /// <returns> Returns a deep copy of the CodeLine object. </returns>
        public CodeLine Duplicate()
        {
            return new CodeLine(this);
        }

        /// <summary>
        /// A method to duplicate the CodeLine object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the CodeLine object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new CodeLine(this) as Action;
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
                return "Invalid Code Line";
            }
            else
            {
                return "Code Line";
            }
        }


        /// <summary>
        /// Sets the code line type. 
        /// </summary>
        /// <param name="type"> The code line type as int value. 0 for commenting on instructions and 1 for commenting on declarations. </param>
        public void SetCodeLineType(int type)
        {
            if (type == 0 | type == 1)
            {
                _type = type;
            }
            else
            {
                _type = 0;
            }
        }

        /// <summary>
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDDeclaration(RobotInfo robotInfo)
        {
            if (_type == 1)
            {
                return _code;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDInstruction(RobotInfo robotInfo)
        {
            if (_type == 0)
            {
                return _code;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            if (_type == 1)
            {
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + _code);
            }
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            if (_type == 0)
            {
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + _code);
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the CodeLine object is valid.
        /// </summary>
        public override bool IsValid
        {
            get 
            { 
                if (Code == null) { return false; }
                if (Code == "") { return false; }
                if (Type < 0) { return false; }
                if (Type > 1) { return false; }
                return true; 
            }
        }

        /// <summary>
        /// The custom RAPID code line
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// Code type as int value.
        /// </summary>
        public int Type
        {
            get { return _type; }
        }
        #endregion
    }

}
