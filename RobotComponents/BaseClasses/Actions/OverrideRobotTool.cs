using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Override Robot Tool class
    /// </summary>
    public class OverrideRobotTool : Action
    {
        #region fields
        private RobotTool _robotTool; // The robot that should be used
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty Override Robot Tool object. 
        /// </summary>
        public OverrideRobotTool()
        {
        }

        /// <summary>
        /// Creates and Override Robot Tool object.
        /// </summary>
        /// <param name="robotTool"> The Robot Tool that should be set. </param>
        public OverrideRobotTool(RobotTool robotTool)
        {
            _robotTool = robotTool;
        }

        /// <summary>
        /// Method to duplicate the Override Robot Tool object.
        /// </summary>
        /// <returns> Returns a deep copy of the Override Robot Tool object.</returns>
        public OverrideRobotTool Duplicate()
        {
            OverrideRobotTool dup = new OverrideRobotTool(RobotTool);
            return dup;
        }
        #endregion

        #region method
        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="robotInfo">Defines the RobotInfo for the action.</param>
        /// <param name="RAPIDcode">Defines the RAPID Code the variable entries are added to.</param>
        /// <returns>Return the RAPID variable code. For this action an empty string. </returns>
        public override string InitRAPIDVar(RAPIDGenerator RAPIDGenerator)
        {
            return "";
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="robotToolName">Defines the robot rool name.</param>
        /// <returns>Returns the RAPID main code. For this action an empty string. </returns>
        public override string ToRAPIDFunction(string robotToolName)
        {
            return "@" + "\t" + "! " + "Default Robot Tool changed to " + _robotTool.Name + ".";
        }

        /// <summary>
        /// Get the name of the set Robot Tool.
        /// </summary>
        /// <returns> The name of the set Robot Tool. </returns>
        public string GetToolName()
        {
            return _robotTool.Name;
        }
        #endregion


        #region properties
        /// <summary>
        /// A boolean that indicates if the Override Robot Tool object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (ToolName == null) { return false; }
                if (ToolName == "") { return false; }
                return true;
            }
        }

        /// <summary>
        /// The Robot Tool that is set.
        /// </summary>
        public RobotTool RobotTool
        {
            get { return _robotTool; }
            set { _robotTool = value; }
        }

        /// <summary>
        /// The name of the set Robot Tool.
        /// </summary>
        public string ToolName
        {
            get { return _robotTool.Name; }
        }
        #endregion
    }
}
