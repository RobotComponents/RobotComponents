using System.IO;
using System.Collections.Generic;
using System.Text;

using Rhino.Geometry;

using RobotComponents.Utils;
using RobotComponents.BaseClasses.Definitions;
using RobotComponents.BaseClasses.Kinematics;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// RAPID Generator class, creates RAPID Code from Actions.
    /// </summary>
    public class RAPIDGenerator
    {
        #region fields
        private RobotInfo _robotInfo; // Robot info to construct the code for
        private readonly InverseKinematics _inverseKinematics; // IK used for calculating Axis in Movements
        private List<Action> _actions = new List<Action>(); // List that stores all actions used by the RAPIDGenerator
        private readonly Dictionary<string, SpeedData> _speedDatas = new Dictionary<string, SpeedData>(); // Dictionary that stores all speedDatas used by the RAPIDGenerator
        private readonly Dictionary<string, Movement> _movements = new Dictionary<string, Movement>();  // Dictionary that stores all movement used by the RAPIDGenerator
        private readonly Dictionary<string, Target> _targets = new Dictionary<string, Target>(); // Dictionary that stores all targets used by the RAPIDGenerator
        private string _filePath; // File path to save the code
        private bool _saveToFile; // Bool that indicates if the files should be saved
        private string _RAPIDCode; // The rapid main code
        private string _BASECode; // The rapid base code
        private string _ModuleName; // The module name of the rapid main code
        private bool _firstMovementIsMoveAbs; // Bool that indicates if the first movememtn is an absolute joint movement
        private StringBuilder _stringBuilder;
        private string _currentTool; // The current tool that should be used
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty RAPID generator
        /// </summary>
        public RAPIDGenerator()
        {
        }

        /// <summary>
        /// Initiates an RAPID generator. This constructor does not call the methods that create and write the code. 
        /// </summary>
        /// <param name="moduleName"> The name of module / program. </param>
        /// <param name="actions"> The list with robot actions wherefore the code should be created. </param>
        /// <param name="filePath"> The path where the code files should be saved. </param>
        /// <param name="saveToFile"> A boolean that indicates if the file should be saved. </param>
        /// <param name="robotInfo"> The robot info wherefore the code should be created. </param>
        public RAPIDGenerator(string moduleName, List<Action> actions, string filePath, bool saveToFile, RobotInfo robotInfo)
        {
            _ModuleName = moduleName;
            _robotInfo = robotInfo;
            _actions = actions;
            _filePath = filePath;
            _saveToFile = saveToFile;
            _inverseKinematics = new InverseKinematics(new Target("init", Plane.WorldXY), _robotInfo);
        }

        /// <summary>
        /// Creates a new RAPID generator by duplicating an existing RAPID generator. 
        /// This creates a deep copy of the existing RAPID generator. 
        /// </summary>
        /// <param name="generator"> The RAPID generator that should be duplicated. </param>
        public RAPIDGenerator(RAPIDGenerator generator)
        {
            _ModuleName = generator.ModuleName;
            _robotInfo = generator.RobotInfo.Duplicate();
            _actions = generator.Actions.ConvertAll(action => action.DuplicateAction());
            _filePath = generator.FilePath;
            _saveToFile = generator.SaveToFile;
            _RAPIDCode = generator.RAPIDCode;
            _BASECode = generator.BASECode;
            _firstMovementIsMoveAbs = generator.FirstMovementIsMoveAbs;
            _inverseKinematics = generator.InverseKinematics.Duplicate();
        }


        /// <summary>
        /// Method to duplicate this RAPID generator object.
        /// </summary>
        /// <returns>Returns a deep copy of the RAPID generator object. </returns>
        public RAPIDGenerator Duplicate()
        {
            return new RAPIDGenerator(this);
        }
        #endregion

        #region method
        /// <summary>
        /// Creates the RAPID main codes.
        /// This method also overwrites or creates a file if saved to file is set eqaul to true.
        /// </summary>
        /// <returns> Returns the RAPID main code as a string. </returns>
        public string CreateRAPIDCode()
        {
            // Resets Dictionaries
            _movements.Clear();
            _speedDatas.Clear();
            _targets.Clear();

            // Set current tool
            _currentTool = _robotInfo.Tool.Name;

            // Creates String Builder
            _stringBuilder = new StringBuilder();

            // Creates Main Module
            _stringBuilder.Append("MODULE " + _ModuleName + "@");

            // Add comment lines for tracking which version of RC was used
            Comment version = new Comment("This RAPID code was generated with RobotComponents v" + VersionNumbering.CurrentVersion);
            version.ToRAPIDFunction(this);
            _stringBuilder.Append("@");

            // Creates Vars
            for (int i = 0; i != _actions.Count; i++)
            {
                _actions[i].InitRAPIDVar(this);
            }

            // Create Program
            _stringBuilder.Append("@" + "@" + "\t" + "PROC main()");

            _firstMovementIsMoveAbs = false;
            bool foundFirstMovement = false;

            // Creates Movement Instruction and other Functions
            for (int i = 0; i != _actions.Count; i++)
            {
                _actions[i].ToRAPIDFunction(this);

                // Check if the action is an override robot tool: if so, set new current tool
                if (_actions[i] is OverrideRobotTool overrideRobotTool)
                {
                    // Override the current tool
                    _currentTool = overrideRobotTool.RobotTool.Name;
                }

                // Checks if first movement is MoveAbsJ
                if (foundFirstMovement == false)
                {
                    // Absolute joint movement found in Action.Movement
                    if (_actions[i] is Movement)
                    {
                        if (((Movement)_actions[i]).MovementType == 0)
                        {
                            _firstMovementIsMoveAbs = true;
                        }

                        foundFirstMovement = true;
                    }

                    // Absolute joint movement found as Action.JointMovement
                    else if (_actions[i] is AbsoluteJointMovement)
                    {
                        _firstMovementIsMoveAbs = true;
                        foundFirstMovement = true;
                    }
                }
            }

            // Closes Program
            _stringBuilder.Append("@" + "\t" + "ENDPROC");
            // Closes Module
            _stringBuilder.Append("@" + "@" + "ENDMODULE");

            // Replaces@ with newLines
            _stringBuilder.Replace("@", System.Environment.NewLine);

            // Update field
            _RAPIDCode = _stringBuilder.ToString();

            // Write to file
            if (_saveToFile == true)
            {
                WriteRAPIDCodeToFile();
            }

            // Return
            return _RAPIDCode;
        }

        /// <summary>
        /// Creates the RAPID base code with as default tool0, wobj0 and load0. 
        /// This method also overwrites or creates a file if saved to file is set equal to true.
        /// </summary>
        /// <param name="robotTools"> The robot tools that should be added to the BASE code as a list. </param>
        /// <param name="workObjects"> The work objects that should be added to the BASE code as a list. </param>
        /// <param name="customCode"> Custom user definied base code as list with strings. </param>
        /// <returns> Returns the RAPID base code as a string. </returns>
        public string CreateBaseCode(List<RobotTool> robotTools, List<WorkObject> workObjects, List<string> customCode)
        {
            // Creates Main Module
            string BASECode = "MODULE BASE (SYSMODULE, NOSTEPIN, VIEWONLY)" + "@" + "@";

            // Version number
            BASECode += " ! This RAPID code was generated with RobotComponents v" + VersionNumbering.CurrentVersion + "@" + "@";

            // Creates Comments
            BASECode += " ! System module with basic predefined system data" + "@";
            BASECode += " !************************************************" + "@" + "@";
            BASECode += " ! System data tool0, wobj0 and load0" + "@";
            BASECode += " ! Do not translate or delete tool0, wobj0, load0" + "@";

            // Creates Predefined System Data
            BASECode += " PERS tooldata tool0 := [TRUE, [[0, 0, 0], [1, 0, 0, 0]], [0.001, [0, 0, 0.001], [1, 0, 0, 0], 0, 0, 0]];" + "@";
            BASECode += " PERS wobjdata wobj0 := [FALSE, TRUE, \"\" , [[0, 0, 0], [1, 0, 0, 0]], [[0, 0, 0], [1, 0, 0, 0]]];" + "@";
            BASECode += " PERS loaddata load0 := [0.001, [0, 0, 0.001], [1, 0, 0, 0], 0, 0, 0];" + "@" + "@";

            // Adds Tools Base Code
            if (robotTools.Count != 0 && robotTools != null)
            {
                BASECode += " ! User defined tooldata " + "@";
                BASECode += CreateToolBaseCode(robotTools);
                BASECode += "@";
            }

            // Adds Work Objects Base Code
            if (workObjects.Count != 0 && workObjects != null)
            {
                BASECode += " ! User defined wobjdata " + "@";
                BASECode += CreateWorkObjectBaseCode(workObjects);
                BASECode += "@";
            }

            // Adds Custom code line
            if (customCode.Count != 0 && customCode != null)
            {
                BASECode += " ! User definied custom code lines " + "@";
                for (int i = 0; i != customCode.Count; i++)
                {
                    BASECode += customCode[i];
                    BASECode += "@";
                }
                BASECode += "@";
            }

            // End Module
            BASECode += "ENDMODULE";

            // Replaces @ with newLines
            BASECode = BASECode.Replace("@", System.Environment.NewLine);

            // Update field
            _BASECode = BASECode;

            // Write to file
            if (_saveToFile == true)
            {
                WriteBASECodeToFile();
            }

            // Return
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
                result += "@" + " ";
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
                result += "@" + " ";
            }

            return result;
        }

        /// <summary>
        /// Writes the RAPID main code to a file if a file path is set
        /// </summary>
        public void WriteRAPIDCodeToFile()
        {
            if (_filePath != null && _filePath != "" && _filePath != "null")
            {
                using (StreamWriter writer = new StreamWriter(_filePath + "\\main_T.mod", false))
                {
                    writer.WriteLine(_RAPIDCode);
                }
            }
        }

        /// <summary>
        /// Writes the BASE Code to a file if a file path is set
        /// </summary>
        public void WriteBASECodeToFile()
        {
            if (_filePath != null && _filePath != "" && _filePath != "null")
            {
                using (StreamWriter writer = new StreamWriter(_filePath + "\\BASE.sys", false))
                {
                    writer.WriteLine(_BASECode);
                }
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the RAPID Generator object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (Actions == null) { return false; }
                return true;
            }
        }

        /// <summary>
        /// The Robot Actions as a list. 
        /// </summary>
        public List<Action> Actions
        {
            get { return _actions; }
            set { _actions = value; }
        }

        /// <summary>
        /// The file path where the code will be saved
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        /// <summary>
        /// A boolean that indicates if the code files should be saved. 
        /// </summary>
        public bool SaveToFile
        {
            get { return _saveToFile; }
            set { _saveToFile = value; }
        }

        /// <summary>
        /// The main RAPID code
        /// </summary>
        public string RAPIDCode
        {
            get { return _RAPIDCode; }
        }

        /// <summary>
        /// The RAPID Base code
        /// </summary>
        public string BASECode
        {
            get { return _BASECode; }
        }

        /// <summary>
        /// The robot info that is should be uses to create the code for.
        /// </summary>
        public RobotInfo RobotInfo
        {
            get
            {
                return _robotInfo;
            }
            set
            {
                _robotInfo = value;
                _inverseKinematics.RobotInfo = _robotInfo;
            }
        }

        /// <summary>
        /// The module name of the RAPID main code
        /// </summary>
        public string ModuleName
        {
            get { return _ModuleName; }
            set { _ModuleName = value; }
        }

        /// <summary>
        /// A boolean that indicates if for the first movement an abosulute joint movement is used. 
        /// It is recommended to use for the first movement an absolute joint movement. 
        /// </summary>
        public bool FirstMovementIsMoveAbs
        {
            get { return _firstMovementIsMoveAbs; }
        }

        /// <summary>
        /// Dictionary that stores all SpeedDatas that are used by the RAPID Generator. 
        /// </summary>
        /// 
        public Dictionary<string, SpeedData> SpeedDatas
        {
            get { return _speedDatas; }
        }

        /// <summary>
        /// Dictionary that stores all Movements that are used by the RAPID Generator. 
        /// </summary>
        public Dictionary<string, Movement> Movements
        {
            get { return _movements; }
        }

        /// <summary>
        /// Dictionary that stores all Targets that are used by the RAPID Generator. 
        /// </summary>
        public Dictionary<string, Target> Targets
        {
            get { return _targets; }
        }

        /// <summary>
        /// The inverse kinematics used by the RAPID Generator. 
        /// </summary>s
        public InverseKinematics InverseKinematics
        {
            get { return _inverseKinematics; }
        }

        /// <summary>
        /// Stringbuilder used by the RAPID Generator. 
        /// </summary>
        public StringBuilder StringBuilder
        {
            get { return _stringBuilder; }
        }

        public string CurrentTool
        {
            get { return _currentTool; }
            set { _currentTool = value; }
        }
        #endregion
    }

}
