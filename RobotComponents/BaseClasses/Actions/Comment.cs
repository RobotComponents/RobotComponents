// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
// RobotComponents Libs
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
            _comment = comment;
        }

        /// <summary>
        /// Creates a new comment by duplicating an existing comment. 
        /// This creates a deep copy of the existing comment. 
        /// </summary>
        /// <param name="comment"> The comment that should be duplicated. </param>
        public Comment(Comment comment)
        {
            _comment = comment.Com;
        }

        /// <summary>
        /// A method to duplicate the Comment object.
        /// </summary>
        /// <returns> Returns a deep copy of the Comment object. </returns>
        public Comment Duplicate()
        {
            return new Comment(this);
        }

        /// <summary>
        /// A method to duplicate the Comment object to an Action object. 
        /// </summary>
        /// <returns> Returns a deep copy of the Comment object as an Action object. </returns>
        public override Action DuplicateAction()
        {
            return new Comment(this) as Action;
        }
        #endregion

        #region method
        /// <summary>
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string InitRAPIDVar(RobotInfo robotInfo)
        {
            return string.Empty;
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDFunction(RobotInfo robotInfo)
        {
            return "! " + _comment;
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void InitRAPIDVar(RAPIDGenerator RAPIDGenerator)
        {
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDFunction(RAPIDGenerator RAPIDGenerator)
        {
            RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + this.ToRAPIDFunction(RAPIDGenerator.RobotInfo));
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the Comment object is valid. 
        /// </summary>
        public override bool IsValid
        {
            get 
            {
                if (Com == null) { return false; }
                if (Com == "") { return false; }
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
