namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Timer class, defines waiting time between two actions. 
    /// </summary>
    public class Timer : Action
    {
        #region fields
        private double _duration;
        #endregion

        #region constructors
        public Timer()
        {
        }

        public Timer(double Duration)
        {
            this._duration = Duration;
        }

        public Timer Duplicate()
        {
            Timer dup = new Timer(Duration);
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
            return ("@" + "\t" + "WaitTime " + _duration + ";"); ;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (Duration == 0) { return false; }
                else { return true; }
            }
        }
        public double Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }
        #endregion
    }

}
