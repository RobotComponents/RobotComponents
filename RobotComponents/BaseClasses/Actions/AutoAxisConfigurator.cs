using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Auto Axis Configurator Class, sets Auto Axis Configuration to True or False.
    /// </summary>
    public class AutoAxisConfig : Action
    {
        #region fields
        private bool _isActive; // boolean that indicates if the auto axis configuration is active
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty AutoAxisConfiguration object.
        /// </summary>
        public AutoAxisConfig()
        {
        }

        /// <summary>
        /// Defines an Auto Axis configuration.
        /// </summary>
        /// <param name="isActive">Bool that enables (true) or disables (false) the auto axis configuration.</param>
        public AutoAxisConfig(bool isActive)
        {
            this._isActive = isActive;
        }

        /// <summary>
        /// A method to duplicate the AutoAxisConfiguration object.
        /// </summary>
        /// <returns> Returns a deep copy of the AutoAxisConfiguration object. </returns>
        public AutoAxisConfig Duplicate()
        {
            AutoAxisConfig dup = new AutoAxisConfig(IsActive);
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
        /// <summary>
        /// A boolean that indicates if the AutoAxisConfiguration object is valid.
        /// </summary>
        public bool IsValid
        {
            get { return true; }
        }

        /// <summary>
        /// A boolean that indicates if the auto axis configruation is enabled.
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        #endregion
    }

}
