using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Timer class, defines waiting time between two actions. This command is used to wait a given amount of time.
    /// </summary>
    public class Timer : Action
    {
        #region fields
        private double _duration; // the time expressed in seconds
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty Timer object. 
        /// </summary>
        public Timer()
        {
        }

        /// <summary>
        /// Constructor to create a wait time object. 
        /// </summary>
        /// <param name="Duration"> The time, expressed in seconds, that program execution is to wait. </param>
        public Timer(double Duration)
        {
            this._duration = Duration;
        }

        /// <summary>
        /// Method to duplicate the Timer object.
        /// </summary>
        /// <returns> Returns a deep copy of the Timer object.</returns>
        public Timer Duplicate()
        {
            Timer dup = new Timer(Duration);
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
        public override void InitRAPIDVar(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="robotToolName">Defines the robot rool name.</param>
        /// <returns>Returns the RAPID main code.</returns>
        public override void ToRAPIDFunction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.StringBuilder.Append("@" + "\t" + "WaitTime " + _duration + ";"); 
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the Timer object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (Duration < 0) { return false; }
                else { return true; }
            }
        }

        /// <summary>
        /// The time, expressed in seconds, that program execution is to wait. Min. value 0 seconds. Max. value no limit. Resolution 0.001 seconds.
        /// </summary>
        public double Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }
        #endregion
    }

}
