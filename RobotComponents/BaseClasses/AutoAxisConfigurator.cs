namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Auto Axis Configurator Class, sets Auto Axis Configuration to True or False.
    /// </summary>
    public class AutoAxisConfig : Action
    {
        #region fields
        private bool _isActive;
        #endregion

        #region constructors
        public AutoAxisConfig()
        {
        }

        public AutoAxisConfig(bool isActive)
        {
            this._isActive = isActive;
        }

        public AutoAxisConfig Duplicate()
        {
            AutoAxisConfig dup = new AutoAxisConfig(IsActive);
            return dup;
        }
        #endregion

        #region method
        public override string InitRAPIDVar(RobotInfo robInfo, string RAPIDcode)
        {
            return ("");
        }

        public override string ToRAPIDFunction(string robotToolName)
        {
            if (_isActive == true)
            {
                return ("@" + "\t" + "ConfJ\\off;" + "@" + "\t" + "ConfL\\off;"); ;
            }
            else
            {
                return ("@" + "\t" + "ConfJ\\on;" + "@" + "\t" + "ConfL\\on;"); ;
            }
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get { return true; }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        #endregion
    }

}
