namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Wait for Digital Input. This class is used to make the code line comamand WaitDI which is 
    /// is used to wait until a digital input is set.
    /// </summary>
    public class WaitDI : Action
    {
        #region fields
        private string _DIName; // The name of the digital input signal
        private bool _value; // The desired state / value of the digtal input signal
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty WaitDI object.
        /// </summary>
        public WaitDI()
        {
        }

        /// <summary>
        /// Defines a WaitDI object. 
        /// </summary>
        /// <param name="DIName"> The name of the signal. </param>
        /// <param name="value"> The desired state / value of the digtal input signal. </param>
        public WaitDI(string DIName, bool value)
        {
            this._DIName = DIName;
            this.Value = value;
        }

        /// <summary>
        /// Method to duplicate the WaitDI object.
        /// </summary>
        /// <returns> Returns a deep copy of the WaitDI object. </returns>
        public WaitDI Duplicate()
        {
            WaitDI dup = new WaitDI(DIName, Value);
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

        #region properties
        /// <summary>
        /// A boolean that indicates if the WaitDI object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (DIName == null) { return false; }
                if (DIName == "") { return false; }
                return true; 
            }
        }

        /// <summary>
        /// The desired state / value of the digtal input signal
        /// </summary>
        public bool Value 
        { 
            get => _value; 
            set => _value = value; 
        }

        /// <summary>
        /// The name of the digital input signal
        /// </summary>
        public string DIName 
        { 
            get => _DIName; 
            set => _DIName = value; 
        }
        #endregion
    }

}

