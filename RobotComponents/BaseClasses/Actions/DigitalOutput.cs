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
            this._name = Name;
            this._isActive = IsActive;
        }

        /// <summary>
        /// A method to duplicate the DigitalOutput object.
        /// </summary>
        /// <returns> Returns a deep copy of the DigitalOutput object. </returns>
        public DigitalOutput Duplicate()
        {
            DigitalOutput dup = new DigitalOutput(Name,IsActive);
            return dup;
        }
        #endregion

        #region method
        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        /// <returns>Return the RAPID variable code.</returns>
        public override void InitRAPIDVar(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        /// <returns>Returns the RAPID main code.</returns>
        public override void ToRAPIDFunction(RAPIDGenerator RAPIDGenerator)
        {
            if (_isActive == true)
            {
                RAPIDGenerator.StringBuilder.Append("@" + "\t" + "SetDO " + _name + ",  1;");
            }
            else
            {
                RAPIDGenerator.StringBuilder.Append("@" + "\t" + "SetDO " + _name + ",  0;"); ;
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the DigitalOutput object is valid. 
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (Name == null) { return false; }; 
                if (Name == "") { return false; };
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
