using System;
using System.IO;
using System.Collections.Generic;

using Rhino.Geometry;

using RobotComponents.BaseClasses;

//using RobotComponents.Components;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// RAPID Generator class, creates RAPID Code from Actions.
    /// </summary>
    public class RAPIDGenerator
    {
        #region fields
        private RobotInfo _robotInfo;
        private List<RobotComponents.BaseClasses.Action> _actions = new List<RobotComponents.BaseClasses.Action>();
        private string _filePath;
        private bool _saveToFile;
        private string _RAPIDCode;
        private string _BASECode;
        private string _ModuleName;
        private Guid _documentGUID;
        private string _roboToolName;
        //private ObjectManager _objectManager;
        private bool _firstMovementIsMoveAbs;
        #endregion

        #region constructors
        public RAPIDGenerator()
        {
        }

        public RAPIDGenerator(string moduleName,  List<RobotComponents.BaseClasses.Action> actions, string filePath, bool saveToFile, RobotInfo robotInfo, Guid documentGUID)
        {
            this._ModuleName = moduleName;
            this._robotInfo = robotInfo;
            this._actions = actions;

            this._filePath = filePath;
            this._saveToFile = saveToFile;

            this._documentGUID = documentGUID;

            this._roboToolName = _robotInfo.Tool.Name;

            if (_saveToFile == true)
            {
                WriteRAPIDCodeToFile();
                WriteBASECodeToFile();
            }
          
        }

        public RAPIDGenerator Duplicate()
        {
            RAPIDGenerator dup = new RAPIDGenerator(ModuleName, Actions, FilePath, SaveToFile, RobotInfo, DocumentGUID);
            return dup;
        }
        #endregion

        #region method
        public string CreateRAPIDCode()
        {
            // Creates Main Module
            string RAPIDCode = "MODULE " + _ModuleName+"@";

            // Creates Tool Name
            string toolName = _roboToolName;

            // Creates Vars
            for (int i = 0; i != _actions.Count; i++)
            {

                string tempCode = _actions[i].InitRAPIDVar(_robotInfo, RAPIDCode);

                // Checks if Var is already in Code
                RAPIDCode += tempCode;
            }

            // Create Program
            RAPIDCode += "@@" + "\t" + "PROC main()";

            _firstMovementIsMoveAbs = false;
            bool foundFirstMovement = false;

            // Creates Movement Instruction and other Functions
            for (int i = 0; i != _actions.Count; i++)
            {
                string rapidStr = _actions[i].ToRAPIDFunction(toolName);

                // Checks if first movement is MoveAbsJ
                if (foundFirstMovement == false)
                {
                    if (_actions[i] is Movement)
                    {
                        if (((Movement)_actions[i]).MovementType == 0)
                        {
                            _firstMovementIsMoveAbs = true;
                        }

                        foundFirstMovement = true;
                    }
                }

                // Checks if action is of Type OverrideRobotTool
                if(_actions[i] is OverrideRobotTool)
                {
                    toolName = ((OverrideRobotTool)_actions[i]).GetToolName();
                }

                RAPIDCode += rapidStr;
            }

            // Closes Program
            RAPIDCode += "@" + "\t" + "ENDPROC";
            // Closes Module
            RAPIDCode += "@@" + "ENDMODULE";

            // Replaces@ with newLines
            RAPIDCode = RAPIDCode.Replace("@", System.Environment.NewLine);

            _RAPIDCode = RAPIDCode;
            return RAPIDCode;
        }

        public string CreateBaseCode(string toolBaseCode)
        {
            // Creates Main Module
            string BASECode = "MODULE BASE (SYSMODULE, NOSTEPIN, VIEWONLY)@@";

            // Creates Comments
            BASECode += " ! System module with basic predefined system data@";
            BASECode += " !************************************************@@";
            BASECode += " ! System data tool0, wobj0 and load0@";
            BASECode += " ! Do not translate or delete tool0, wobj0, load0@";

            // Creates Predefined System Data
            BASECode += " PERS tooldata tool0 := [TRUE, [[0, 0, 0], [1, 0, 0, 0]],@";
            BASECode += "\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "[0.001, [0, 0, 0.001],[1, 0, 0, 0], 0, 0, 0]];@@";

            BASECode += " PERS wobjdata wobj0 := [FALSE, TRUE, \"\" , [[0, 0, 0],[1, 0, 0, 0]],@";
            BASECode += "\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "[[0, 0, 0],[1, 0, 0, 0]]];@@";

            BASECode += " PERS loaddata load0 := [0.001, [0, 0, 0.001],[1, 0, 0, 0], 0, 0, 0];@@";

            // Adds Tool Base Code
            BASECode += toolBaseCode;
  
            // End Module
            BASECode += "ENDMODULE";

            // Replaces@ with newLines
            BASECode = BASECode.Replace("@", System.Environment.NewLine);

            _BASECode = BASECode;
            return BASECode;
        }

        //writes RAPID Code to File on Harddrive
        public void WriteRAPIDCodeToFile()
        {
            using (StreamWriter writer = new StreamWriter(FilePath + "\\main_T.mod", false))
            {
                writer.WriteLine(_RAPIDCode);
            }
        }

        //writes Base Code to File on Harddrive
        public void WriteBASECodeToFile()
        {
            using (StreamWriter writer = new StreamWriter(FilePath + "\\BASE.sys", false))
            {
                writer.WriteLine(_BASECode);
            }
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (Actions == null) { return false; }
                return true;
            }
        }
        public List<RobotComponents.BaseClasses.Action> Actions
        {
            get { return _actions; }
            set { _actions = value; }
        }
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        public bool SaveToFile
        {
            get { return _saveToFile; }
            set { _saveToFile = value; }
        }
        public string RAPIDCode
        {
            get { return _RAPIDCode; }
            set { _RAPIDCode = value; }
        }
        public string BASECode
        {
            get { return _BASECode; }
            set { _BASECode = value; }
        }
        public RobotInfo RobotInfo
        {
            get { return _robotInfo; }
            set { _robotInfo = value; }
        }

        public Guid DocumentGUID { get => _documentGUID; set => _documentGUID = value; }
        public string ModuleName { get => _ModuleName; set => _ModuleName = value; }
        public bool FirstMovementIsMoveAbs { get => _firstMovementIsMoveAbs;}
        #endregion
    }

}
