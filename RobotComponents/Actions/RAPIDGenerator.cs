// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
// RobotComponents Libs
using RobotComponents.Utils;
using RobotComponents.Definitions;

namespace RobotComponents.Actions
{
    /// <summary>
    /// RAPID Generator class, creates RAPID Code from Actions.
    /// </summary>
    public class RAPIDGenerator
    {
        #region fields
        private Robot _robot; // Robot to construct the code for
        private List<Action> _actions = new List<Action>(); // List that stores all actions used by the RAPIDGenerator
        private readonly Dictionary<string, SpeedData> _speedDatas = new Dictionary<string, SpeedData>(); // Dictionary that stores all speedDatas used by the RAPIDGenerator
        private readonly Dictionary<string, ZoneData> _zoneDatas = new Dictionary<string, ZoneData>(); // Dictionary that stores all zoneDatas used by the RAPIDGenerator
        private readonly Dictionary<string, Target> _robotTargets = new Dictionary<string, Target>(); // Dictionary that stores all the unique robot targets used by the RAPIDGenerator
        private readonly Dictionary<string, JointTarget> _jointTargets = new Dictionary<string, JointTarget>(); // Dictionary that stores all the unique jonit targets used by the RAPIDGenerator
        private string _filePath; // File path to save the code
        private bool _saveToFile; // Bool that indicates if the files should be saved
        private string _programCode; // The rapid program code
        private string _systemCode; // The rapid system code
        private string _programModuleName; // The module name of the rapid program code
        private string _systemModuleName; // The module name of the rapod system code
        private bool _firstMovementIsMoveAbs; // Bool that indicates if the first movememtn is an absolute joint movement
        private StringBuilder _stringBuilder;
        private readonly List<string> _errorText = new List<string>(); // List with collected error messages: for now only checking for absolute joint momvements!
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
        /// <param name="programModuleName"> The name of the program module </param>
        /// <param name="systemModuleName"> The name of the system module </param>
        /// <param name="actions"> The list with robot actions wherefore the code should be created. </param>
        /// <param name="filePath"> The path where the code files should be saved. </param>
        /// <param name="saveToFile"> A boolean that indicates if the file should be saved. </param>
        /// <param name="robot"> The robot info wherefore the code should be created. </param>
        public RAPIDGenerator(string programModuleName, string systemModuleName, List<Action> actions, string filePath, bool saveToFile, Robot robot)
        {
            _programModuleName = programModuleName;
            _systemModuleName = systemModuleName;
            _robot = robot.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
            _actions = actions;
            _filePath = filePath;
            _saveToFile = saveToFile;
        }

        /// <summary>
        /// Creates a new RAPID generator by duplicating an existing RAPID generator. 
        /// This creates a deep copy of the existing RAPID generator. 
        /// </summary>
        /// <param name="generator"> The RAPID generator that should be duplicated. </param>
        public RAPIDGenerator(RAPIDGenerator generator)
        {
            _programModuleName = generator.ProgramModuleName;
            _systemModuleName = generator.SystemModuleName;
            _robot = generator.Robot.Duplicate();
            _actions = generator.Actions.ConvertAll(action => action.DuplicateAction());
            _filePath = generator.FilePath;
            _saveToFile = generator.SaveToFile;
            _programCode = generator.ProgramCode;
            _systemCode = generator.SystemCode;
            _firstMovementIsMoveAbs = generator.FirstMovementIsMoveAbs;
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
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid RAPID Generator";
            }
            else
            {
                return "RAPID Generator";
            }
        }

        /// <summary>
        /// Creates the RAPID program code.
        /// This method also overwrites or creates a file if saved to file is set eqaul to true.
        /// </summary>
        /// <returns> Returns the RAPID program code as a string. </returns>
        public string CreateProgramCode()
        {
            // Reset fields
            _speedDatas.Clear();
            _robotTargets.Clear();
            _errorText.Clear();

            // Save initial tool
            RobotTool initTool = _robot.Tool.Duplicate();

            // Check if the first movement is an Absolute Joint Movement
            _firstMovementIsMoveAbs = CheckFirstMovement();

            // Creates String Builder
            _stringBuilder = new StringBuilder();

            // Creates Main Module
            _stringBuilder.Append("MODULE " + _programModuleName);
            _stringBuilder.Append(Environment.NewLine);
            _stringBuilder.Append(Environment.NewLine);

            // Add comment lines for tracking which version of RC was used
            _stringBuilder.Append("\t" + "! This RAPID code was generated with RobotComponents v" + VersionNumbering.CurrentVersion);
            _stringBuilder.Append(Environment.NewLine);
            _stringBuilder.Append("\t" + "! Visit www.github.com/RobotComponents for more information");
            _stringBuilder.Append(Environment.NewLine);

            // Creates declarations
            for (int i = 0; i != _actions.Count; i++)
            {
                _actions[i].ToRAPIDDeclaration(this);
            }

            // Create Program
            _stringBuilder.Append(Environment.NewLine);
            _stringBuilder.Append(Environment.NewLine);
            _stringBuilder.Append("\t" + "PROC main()");

            // Set back initial tool
            _robot.Tool = initTool;

            // Creates instructions
            for (int i = 0; i != _actions.Count; i++)
            {
                _actions[i].ToRAPIDInstruction(this);
            }

            // Closes Program
            _stringBuilder.Append(Environment.NewLine);
            _stringBuilder.Append("\t" + "ENDPROC");
            // Closes Module
            _stringBuilder.Append(Environment.NewLine);
            _stringBuilder.Append(Environment.NewLine);
            _stringBuilder.Append("ENDMODULE");

            // Update field
            _programCode = _stringBuilder.ToString();

            // Write to file
            if (_saveToFile == true)
            {
                WriteProgramCodeToFile();
            }

            // Return
            return _programCode;
        }

        /// <summary>
        /// Creates the RAPID system code with as default tool0, wobj0 and load0 if the system module name is equal to BASE
        /// This method also overwrites or creates a file if saved to file is set equal to true.
        /// </summary>
        /// <param name="robotTools"> The robot tools that should be added to the BASE code as a list. </param>
        /// <param name="workObjects"> The work objects that should be added to the BASE code as a list. </param>
        /// <param name="customCode"> Custom user definied base code as list with strings. </param>
        /// <returns> Returns the RAPID system code as a string. </returns>
        public string CreateSystemCode(List<RobotTool> robotTools, List<WorkObject> workObjects, List<string> customCode)
        {
            // Initialize
            string systemCode = "";

            // First line
            if (_systemModuleName == "BASE")
            {
                systemCode += "MODULE BASE (SYSMODULE, NOSTEPIN, VIEWONLY)";
                systemCode += Environment.NewLine;
                systemCode += Environment.NewLine;
            }
            else
            {
                systemCode += "MODULE " + _systemModuleName + " (SYSMODULE)";
                systemCode += Environment.NewLine;
                systemCode += Environment.NewLine;
            }

            // Version number
            systemCode += "\t" + "! This RAPID code was generated with RobotComponents v" + VersionNumbering.CurrentVersion;
            systemCode += Environment.NewLine;
            systemCode += "\t" + "! Visit www.github.com/RobotComponents for more information";
            systemCode += Environment.NewLine;
            systemCode += Environment.NewLine;

            // Creates Comments
            systemCode += "\t" + "! System module with basic predefined system data";
            systemCode += Environment.NewLine;
            systemCode += "\t" + "!************************************************";
            systemCode += Environment.NewLine;
            systemCode += Environment.NewLine;

            // Creates Predefined System Data: only if it is the BASE module
            if (_systemModuleName == "BASE")
            {
                systemCode += "\t" + "! System data tool0, wobj0 and load0";
                systemCode += Environment.NewLine;
                systemCode += "\t" + "! Do not translate or delete tool0, wobj0, load0";
                systemCode += Environment.NewLine;

                // Creates Predefined System Data
                systemCode += "\t" + "PERS tooldata tool0 := [TRUE, [[0, 0, 0], [1, 0, 0, 0]], [0.001, [0, 0, 0.001], [1, 0, 0, 0], 0, 0, 0]];";
                systemCode += Environment.NewLine;
                systemCode += "\t" + "PERS wobjdata wobj0 := [FALSE, TRUE, \"\" , [[0, 0, 0], [1, 0, 0, 0]], [[0, 0, 0], [1, 0, 0, 0]]];";
                systemCode += Environment.NewLine;
                systemCode += "\t" + "PERS loaddata load0 := [0.001, [0, 0, 0.001], [1, 0, 0, 0], 0, 0, 0];";
                systemCode += Environment.NewLine;
                systemCode += Environment.NewLine;
            }

            // Adds Tools Base Code
            if (robotTools.Count != 0 && robotTools != null)
            {
                systemCode += "\t" + "! User defined tooldata ";
                systemCode += Environment.NewLine;
                systemCode += CreateToolSystemCode(robotTools);
                systemCode += Environment.NewLine;
            }

            // Adds Work Objects Base Code
            if (workObjects.Count != 0 && workObjects != null)
            {
                systemCode += "\t" + "! User defined wobjdata ";
                systemCode += Environment.NewLine;
                systemCode += CreateWorkObjectSystemCode(workObjects);
                systemCode += Environment.NewLine;
            }

            // Adds Custom code line
            if (customCode.Count != 0 && customCode != null)
            {
                systemCode += "\t" + "! User definied custom code lines";
                systemCode += Environment.NewLine;

                for (int i = 0; i != customCode.Count; i++)
                {
                    systemCode += "\t" + customCode[i];
                    systemCode += Environment.NewLine;
                }

                systemCode += Environment.NewLine;
            }

            // End Module
            systemCode += "ENDMODULE";

            // Update field
            _systemCode = systemCode;

            // Write to file
            if (_saveToFile == true)
            {
                WriteSystemCodeToFile();
            }

            // Return
            return systemCode;
        }

        /// <summary>
        /// Gets the System Code for all Robot Tools in the list.
        /// </summary>
        /// <param name="robotTools"> The list with Robot Tools. </param>
        /// <returns> Returns the robot tool system code as a string. </returns>
        private string CreateToolSystemCode(List<RobotTool> robotTools)
        {
            string result = "";

            for (int i = 0; i != robotTools.Count; i++)
            {
                result += "\t" + robotTools[i].GetRSToolData();
                result += Environment.NewLine;
            }

            return result;
        }

        /// <summary>
        /// Gets the System Code for all Robot Tools in the list.
        /// </summary>
        /// <param name="workObjects"> The list with Robot Tools. </param>
        /// <returns> Returns the robot tool system code as a string. </returns>
        private string CreateWorkObjectSystemCode(List<WorkObject> workObjects)
        {
            string result = "";

            for (int i = 0; i != workObjects.Count; i++)
            {
                result += "\t" + workObjects[i].GetWorkObjData();
                result += Environment.NewLine;
            }

            return result;
        }

        /// <summary>
        /// Writes the RAPID program code to a file if a file path is set
        /// </summary>
        public void WriteProgramCodeToFile()
        {
            if (_filePath != null && _filePath != "" && _filePath != "null")
            {
                using (StreamWriter writer = new StreamWriter(_filePath + "\\" + _programModuleName + ".mod", false))
                {
                    writer.WriteLine(_programCode);
                }
            }
        }

        /// <summary>
        /// Writes the RAPID system code to a file if a file path is set
        /// </summary>
        public void WriteSystemCodeToFile()
        {
            if (_filePath != null && _filePath != "" && _filePath != "null")
            {
                using (StreamWriter writer = new StreamWriter(_filePath + "\\" + _systemModuleName + ".sys", false))
                {
                    writer.WriteLine(_systemCode);
                }
            }
        }

        /// <summary>
        /// Checks whether the first movement type is an Absolute Joint Movement
        /// </summary>
        /// <returns> Returns a boolean that indicates if the first movement type is an Absolute Joint Movement. </returns>
        private bool CheckFirstMovement()
        {
            _firstMovementIsMoveAbs = false;

            for (int i = 0; i != _actions.Count; i++)
            {
                if (_actions[i] is Movement movement)
                {
                    if (movement.MovementType == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                else if (_actions[i] is AbsoluteJointMovement)
                {
                    return true;
                }
            }

            // Returns true if no movements were defined
            return true; 
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
        /// The RAPID Program code
        /// </summary>
        public string ProgramCode
        {
            get { return _programCode; }
        }

        /// <summary>
        /// The RAPID System code
        /// </summary>
        public string SystemCode
        {
            get { return _systemCode; }
        }

        /// <summary>
        /// Defines the Robot that is used to create the RAPID code for. 
        /// </summary>
        public Robot Robot
        {
            get { return _robot; }
            set { _robot = value; }
        }

        /// <summary>
        /// The module name of the RAPID program module
        /// </summary>
        public string ProgramModuleName
        {
            get { return _programModuleName; }
            set { _programModuleName = value; }
        }

        /// <summary>
        /// The module name of the RAPID system module
        /// </summary>
        public string SystemModuleName
        {
            get { return _systemModuleName; }
            set { _systemModuleName = value; }
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
        public Dictionary<string, SpeedData> SpeedDatas
        {
            get { return _speedDatas; }
        }


        /// <summary>
        /// Dictionary that stores all ZoneDatas that are used by the RAPID Generator. 
        /// </summary>
        public Dictionary<string, ZoneData> ZoneDatas
        {
            get { return _zoneDatas; }
        }

        /// <summary>
        /// Defines all the unique Robot Targets used in this RAPID Generator
        /// </summary>
        public Dictionary<string, Target> RobotTargets
        {
            get { return _robotTargets; }
        }

        /// <summary>
        /// Defines all the unique Joint Targets used in this RAPID Generator
        /// </summary>
        public Dictionary<string, JointTarget> JointTargets
        {
            get { return _jointTargets; }
        }


        /// <summary>
        /// Stringbuilder used by the RAPID Generator. 
        /// </summary>
        public StringBuilder StringBuilder
        {
            get { return _stringBuilder; }
        }

        /// <summary>
        /// List of strings with collected error messages. 
        /// </summary>
        public List<string> ErrorText
        {
            get { return _errorText; }
        }
        #endregion
    }

}
