namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Digital Output class
    /// </summary>
    public class DigitalOutput : Action
    {
        #region fields
        private string _name;
        private bool _isActive;
        #endregion

        #region constructors
        public DigitalOutput()
        {
        }

        public DigitalOutput(string Name, bool IsActive)
        {
            this._name = Name;
            this._isActive = IsActive;
        }

        public DigitalOutput Duplicate()
        {
            DigitalOutput dup = new DigitalOutput(Name,IsActive);
            return dup;
        }
        #endregion

        #region method
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            return ("");
        }

        public override string ToRAPIDFunction(string robotToolName)
        {
            if (_isActive == true)
            {
                return ("@" + "\t" + "SetDO " + _name + ",  1;");
            }
            else
            {
                return ("@" + "\t" + "SetDO " + _name + ",  0;"); ;
            }
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (Name == null) { return false; };
                return true;
            }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        #endregion
    }
}
