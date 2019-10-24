
namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// CodeLine class, defines a CodeLine in RAPID Code.
    /// </summary>
    /// 

    public class CodeLine : Action
    {
        #region fields
        private string _code;
        #endregion

        #region constructors
        public CodeLine()
        {
        }

        public CodeLine(string code)
        {
            this._code = code;
        }

        public CodeLine Duplicate()
        {
            CodeLine dup = new CodeLine(Code);
            return dup;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        #endregion

        #region method
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            return ("");
        }

        public override string ToRAPIDFunction()
        {
            return ("@" + "\t" + _code); ;
        }
        #endregion
    }

}
