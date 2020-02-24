// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Code Line class, defines a CodeLine in RAPID Code.
    /// </summary>
    public class CodeLine : Action
    {
        #region fields
        private string _code;
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
        }

        /// <summary>
        /// Creates a new code line by duplicating an existing code line. 
        /// This creates a deep copy of the existing code line. 
        /// </summary>
        /// <param name="codeLine"> The code line that should be duplicated. </param>
        public CodeLine(CodeLine codeLine)
        {
            _code = codeLine.Code;
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
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void InitRAPIDVar(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDFunction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.StringBuilder.Append("@" + "\t" + _code); 
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
        #endregion
    }

}
