using System;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Override Robot Tool class
    /// </summary>

    public class OverrideRobotTool : Action
    {
        #region fields
        private RobotTool _robotTool;
        private string _toolName;
        private string _toolData;
        #endregion

        #region constructors
        public OverrideRobotTool()
        {
        }

        public OverrideRobotTool(RobotTool robotTool)
        {
            this._robotTool = robotTool;
            this._toolName = _robotTool.Name;
            this._toolData = GetToolData();
        }

        public OverrideRobotTool Duplicate()
        {
            OverrideRobotTool dup = new OverrideRobotTool(RobotTool);
            return dup;
        }
        #endregion

        #region method
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            return _toolData;
        }

        public override string ToRAPIDFunction(string robotToolName)
        {
            return "";
        }

        public string GetToolData()
        {
            return "";
        }

        public string GetToolName()
        {
            return _toolName;
        }
        #endregion


        #region properties
        public bool IsValid
        {
            get
            {
                if (ToolName == null) { return false; }
                return true;
            }
        }
        public string ToolName
        {
            get { return _toolName; }
            set { _toolName = value; }
        }
        public RobotTool RobotTool
        {
            get { return _robotTool; }
            set { _robotTool = value; }
        }
        public string ToolData
        {
            get { return _toolData; }
            set { _toolData = value; }
        }
        #endregion
    }
}
