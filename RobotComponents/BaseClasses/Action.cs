namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Action class, abstract main class for all actions.
    /// </summary>
    public abstract class Action
    {
        #region fields

        #endregion

        #region constructors
        public Action()
        {

        }
        #endregion

        #region methods
        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="robInfo">Defines the RobotInfo for the action.</param>
        /// <param name="RAPIDCode">Defines the RAPID Code the variable entries are added to.</param>
        /// <returns></returns>
        public abstract string InitRAPIDVar(RobotInfo robInfo, string RAPIDCode);

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="robotToolName">Defines the robot rool name.</param>
        /// <returns></returns>
        public abstract string ToRAPIDFunction(string robotToolName);
        #endregion

        #region properties
        #endregion
    }
}