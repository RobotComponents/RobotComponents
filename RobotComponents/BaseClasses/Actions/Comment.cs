using System;

using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// Comment class, defines a Comment in RAPID Code.
    /// A comment is only used to make the program easier to understand. 
    /// It has no effect on the execution of the program.
    /// </summary>
    public class Comment : Action
    {
        #region fields
        private string _comment; // the comment as a string
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty Comment object.
        /// </summary>
        public Comment()
        {
        }

        /// <summary>
        /// A comment constructor inserted into the program to make it easier to understand.
        /// </summary>
        /// <param name="comment">The comment as a text string.</param>
        public Comment(string comment)
        {
            this._comment = comment;
        }

        /// <summary>
        /// A method to duplicate the Comment object.
        /// </summary>
        /// <returns> Returns a deep copy of the Comment object. </returns>
        public Comment Duplicate()
        {
            Comment dup = new Comment(Com);
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
        public override string InitRAPIDVar(RAPIDGenerator RAPIDGenerator)
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
            string tempCode = "";

            string[] lines = _comment.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                tempCode += "@" + "\t" + "! " + lines[i];
            }

            return tempCode;
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the Comment object is valid. 
        /// </summary>
        public bool IsValid
        {
            get 
            {
                if (Com == null) { return false; };
                return true; 
            }
        }

        /// <summary>
        /// Comment line as a string.
        /// </summary>
        public string Com
        {
            get { return _comment; }
            set { _comment = value; }
        }
        #endregion
    }

}
