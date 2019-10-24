using System;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Comment class, defines a Comment in RAPID Code.
    /// </summary>
    /// 

    public class Comment : Action
    {
        #region fields
        private string _comment;
        #endregion

        #region constructors
        public Comment()
        {
        }

        public Comment(string comment)
        {
            this._comment = comment;
        }

        public Comment Duplicate()
        {
            Comment dup = new Comment(Com);
            return dup;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public string Com
        {
            get { return _comment; }
            set { _comment = value; }
        }
        #endregion

        #region method
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            return ("");
        }

        public override string ToRAPIDFunction()
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
    }

}
