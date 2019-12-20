
namespace RobotComponents.BaseClasses
{
    public class WaitDI : Action
    {
        #region fields
        private string _DIName;
        private bool _value;
        #endregion
        #region constructors
        public WaitDI()
        {
        }

        public WaitDI(string DIName, bool value)
        {
            this._DIName = DIName;
            this.Value = value;
        }

        public WaitDI Duplicate()
        {
            WaitDI dup = new WaitDI(DIName, Value);
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

        public bool Value { get => _value; set => _value = value; }
        public string DIName { get => _DIName; set => _DIName = value; }
        #endregion

        #region method
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            return ("");
        }

        public override string ToRAPIDFunction(string robotToolName)
        {
            string value;

            if(_value == true)
            {
                value = "1";
            }
            else
            {
                value = "0";
            }

            return ("@" + "\t" + "WaitDI " + _DIName +", " + value + ";"); ;
        }
        #endregion
    }

}

