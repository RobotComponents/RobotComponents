using RobotComponents.BaseClasses.Definitions;

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
            this._code = code;
        }

        /// <summary>
        /// A method to duplicate the CodeLine object.
        /// </summary>
        /// <returns> Returns a deep copy of the CodeLine object. </returns>
        public CodeLine Duplicate()
        {
            CodeLine dup = new CodeLine(Code);
            return dup;
        }
        #endregion

        #region method
        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="robotInfo">Defines the RobotInfo for the action.</param>
        /// <param name="RAPIDcode">Defines the RAPID Code the variable entries are added to.</param>
        /// <returns>Return the RAPID variable code.</returns>
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            return ("");
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="robotToolName">Defines the robot rool name.</param>
        /// <returns>Returns the RAPID main code.</returns>
        public override string ToRAPIDFunction(string robotToolName)
        {
            return ("@" + "\t" + _code); ;
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the CodeLine object is valid.
        /// </summary>
        public bool IsValid
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
