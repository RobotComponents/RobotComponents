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
        private int _type; // the comment type as int value -> 0 for instruction, 1 for declaration
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
            _type = 0;
        }

        /// <summary>
        /// A comment constructor inserted into the program to make it easier to understand.
        /// </summary>
        /// <param name="comment">The comment as a text string.</param>
        /// <param name="type">The comment type as int value. 0 for commenting on instructions and 1 for commenting on declarations.</param>
        public Comment(string comment, int type)
        {
            _comment = comment;
            SetCommentType(type);
        }

        /// <summary>
        /// Creates a new comment by duplicating an existing comment. 
        /// This creates a deep copy of the existing comment. 
        /// </summary>
        /// <param name="comment"> The comment that should be duplicated.</param>
        public Comment(Comment comment)
        {
            _comment = comment.Com;
            _type = comment.Type;
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
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid Comment";
            }
            else
            {
                return "Comment";
            }
        }

        /// <summary>
        /// Sets the comment type. 
        /// </summary>
        /// <param name="type"> The comment type as int value. 0 for commenting on instructions and 1 for commenting on declarations. </param>
        public void SetCommentType(int type)
        {
            if(type == 0 | type == 1)
            {
                _type = type;
            }
            else
            {
                _type = 0;
            }
        }

        /// <summary>
        /// Used to create variable definition code of this action. 
        /// </summary>
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDDeclaration(RobotInfo robotInfo)
        {
            return "! " + _comment;
        }

        /// <summary>
        /// Used to create action instruction code line. 
        /// </summary>
        /// <param name="robotInfo"> Defines the Robot Info were the code is generated for. </param>
        /// <returns> Returns the RAPID code line as a string. </returns>
        public override string ToRAPIDInstruction(RobotInfo robotInfo)
        {
            return "! " + _comment;
        }

        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            if (_type == 1)
            {
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + this.ToRAPIDDeclaration(RAPIDGenerator.RobotInfo));
            }
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="RAPIDGenerator"> Defines the RAPIDGenerator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            if (_type == 0)
            {
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + this.ToRAPIDInstruction(RAPIDGenerator.RobotInfo));
            }
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

        /// <summary>
        /// Comment type as int value.
        /// </summary>
        public int Type
        {
            get { return _type; }
        }
        #endregion
    }

}
