using System.IO;
using System.Collections.Generic;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// RAPID Generator class, creates RAPID Code from Actions.
    /// </summary>
    public class RAPIDGenerator
    {
        #region fields
        private RobotInfo _robotInfo;
        private List<Action> _actions = new List<Action>();
        private string _filePath;
        private bool _saveToFile;
        private string _RAPIDCode;
        private string _BASECode;
        private string _ModuleName;
        private bool _firstMovementIsMoveAbs;
        #endregion

        #region constructors
        public RAPIDGenerator()
        {
        }

        public RAPIDGenerator(string moduleName, List<Action> actions, string filePath, bool saveToFile, RobotInfo robotInfo)
        {
            this._ModuleName = moduleName;
            this._robotInfo = robotInfo;
            this._actions = actions;

            this._filePath = filePath;
            this._saveToFile = saveToFile;

            if (_saveToFile == true)
            {
                WriteRAPIDCodeToFile();
                WriteBASECodeToFile();
            }

        }

        public RAPIDGenerator Duplicate()
        {
            RAPIDGenerator dup = new RAPIDGenerator(ModuleName, Actions, FilePath, SaveToFile, RobotInfo);
            return dup;
        }
        #endregion

        #region method
        public string CreateRAPIDCode()
        {
            // Creates Main Module
            string RAPIDCode = "MODULE " + _ModuleName + "@";

            // Creates Tool Name
            string toolName = _robotInfo.Tool.Name;

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
                if (_actions[i] is OverrideRobotTool)
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

        public string CreateBaseCode(List<RobotTool> robotTools, List<WorkObject> workObjects)
        {
            // Creates Main Module
            string BASECode = "MODULE BASE (SYSMODULE, NOSTEPIN, VIEWONLY)@@";

            // Creates Comments
            BASECode += " ! System module with basic predefined system data@";
            BASECode += " !************************************************@@";
            BASECode += " ! System data tool0, wobj0 and load0@";
            BASECode += " ! Do not translate or delete tool0, wobj0, load0@";

            // Creates Predefined System Data
            BASECode += " PERS tooldata tool0 := [TRUE, [[0, 0, 0], [1, 0, 0, 0]], [0.001, [0, 0, 0.001], [1, 0, 0, 0], 0, 0, 0]];@";
            BASECode += " PERS wobjdata wobj0 := [FALSE, TRUE, \"\" , [[0, 0, 0], [1, 0, 0, 0]], [[0, 0, 0], [1, 0, 0, 0]]];@";
            BASECode += " PERS loaddata load0 := [0.001, [0, 0, 0.001], [1, 0, 0, 0], 0, 0, 0];@@";

            // Adds Tools Base Code
            if (robotTools.Count != 0)
            {
                BASECode += " ! User defined tooldata @";
                BASECode += CreateToolBaseCode(robotTools);
                BASECode += "@ ";
            }

            // Adds Work Objects Base Code
            if (workObjects.Count != 0)
            {
                BASECode += " ! User defined wobjdata @";
                BASECode += CreateWorkObjectBaseCode(workObjects);
                BASECode += "@ ";
            }

            // End Module
            BASECode += "ENDMODULE";

            // Replaces @ with newLines
            BASECode = BASECode.Replace("@", System.Environment.NewLine);

            _BASECode = BASECode;
            return BASECode;
        }

        /// <summary>
        /// Gets the Base Code for all Robot Tools in the list.
        /// </summary>
        /// <param name="robotTools"> The list with Robot Tools. </param>
        /// <returns> Returns the robot tool base code as a string. </returns>
        private string CreateToolBaseCode(List<RobotTool> robotTools)
        {
            string result = " ";

            for (int i = 0; i != robotTools.Count; i++)
            {
                result += robotTools[i].GetRSToolData();
                result += System.Environment.NewLine + " ";
            }

            return result;
        }

        /// <summary>
        /// Gets the Base Code for all Robot Tools in the list.
        /// </summary>
        /// <param name="workObjects"> The list with Robot Tools. </param>
        /// <returns> Returns the robot tool base code as a string. </returns>
        private string CreateWorkObjectBaseCode(List<WorkObject> workObjects)
        {
            string result = " ";

            for (int i = 0; i != workObjects.Count; i++)
            {
                result += workObjects[i].GetWorkObjData();
                result += System.Environment.NewLine + " ";
            }

            return result;
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

        public List<Action> Actions
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

        public string ModuleName
        {
            get { return _ModuleName; }
            set { _ModuleName = value; }
        }

        public bool FirstMovementIsMoveAbs
        {
            get { return _firstMovementIsMoveAbs; }
        }
        #endregion
    }

}
