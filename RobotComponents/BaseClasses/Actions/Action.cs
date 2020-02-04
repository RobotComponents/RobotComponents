namespace RobotComponents.BaseClasses.Actions
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
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public abstract void InitRAPIDVar(RAPIDGenerator RAPIDGenerator);

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public abstract void ToRAPIDFunction(RAPIDGenerator RAPIDGenerator);
        #endregion

        #region properties

        #endregion
    }
}