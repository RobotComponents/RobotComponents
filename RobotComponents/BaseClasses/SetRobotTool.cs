using System;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// ToolChange class, defines the Movement to a RobTarget.
    /// </summary>
    /// 

    public class SetRobotTool : Action
    {
        #region fields
        private RobotTool _robotTool;
        private string _toolName;
        private string _toolData;
        private Guid _documentGUID;
        private ObjectManager _objectManager;
        #endregion

        #region constructors
        public SetRobotTool()
        {
        }

        public SetRobotTool(RobotTool robotTool, Guid documentGUID)
        {
            this._robotTool = robotTool;
            this._toolName = _robotTool.Name;
            this._toolData = GetToolData();
            this._documentGUID = documentGUID;

            // Checks if ObjectManager for this document already exists. If not it creates a new one
            if (!DocumentManager.ObjectManagers.ContainsKey(_documentGUID))
            {
                DocumentManager.ObjectManagers.Add(_documentGUID, new ObjectManager());
            }

            // Gets ObjectManager of this document
            _objectManager = DocumentManager.ObjectManagers[_documentGUID];
        }

        public SetRobotTool Duplicate()
        {
            SetRobotTool dup = new SetRobotTool(RobotTool, DocumentGUID);
            return dup;
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

        public Guid DocumentGUID { get => _documentGUID; set => _documentGUID = value; }
        public ObjectManager ObjectManager { get => _objectManager; set => _objectManager = value; }
        #endregion

        #region method
        public override string InitRAPIDVar(RobotInfo robotInfo, string RAPIDcode)
        {
            return _toolData;
        }

        public override string ToRAPIDFunction()
        {
            _objectManager.CurrentTool = _toolName;
            return "";
        }

        public string GetToolData()
        {
            return "";
        }
        #endregion

    }



}
