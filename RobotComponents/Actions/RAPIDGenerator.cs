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
using RobotComponents.Enumerations;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents the RAPID Generator.
    /// This is class is used to generate the RAPID program and system module from a given set of actions.
    /// </summary>
    public class RAPIDGenerator
    {
        #region fields
        private Robot _robot; // Robot to construct the code for
        private List<Action> _actions = new List<Action>(); // List that stores all actions used by the RAPIDGenerator
        private readonly Dictionary<string, SpeedData> _speedDatas = new Dictionary<string, SpeedData>(); // Dictionary that stores all speedDatas used by the RAPIDGenerator
        private readonly Dictionary<string, ZoneData> _zoneDatas = new Dictionary<string, ZoneData>(); // Dictionary that stores all zoneDatas used by the RAPIDGenerator
        private readonly Dictionary<string, ITarget> _targets = new Dictionary<string, ITarget>(); // Dictionary that stores all the unique targets used by the RAPIDGenerator
        private readonly Dictionary<string, RobotTool> _robotTools = new Dictionary<string, RobotTool>(); // Dictionary that stores all the unique robo ttools used by the RAPIDGenerator
        private readonly Dictionary<string, WorkObject> _workObjects = new Dictionary<string, WorkObject>(); // Dictionary that stores all the unique work objects used by the RAPIDGenerator
        private string _filePath; // File path to save the code
        private bool _saveToFile; // Bool that indicates if the files should be saved
        private string _programCode; // The rapid program code
        private string _systemCode; // The rapid system code
        private string _programModuleName; // The module name of the rapid program code
        private string _systemModuleName; // The module name of the rapod system code
        private bool _firstMovementIsMoveAbsJ; // Bool that indicates if the first movemement is an absolute joint movement
        private StringBuilder _stringBuilder;
        private readonly List<string> _errorText = new List<string>(); // List with collected error messages: for now only checking for absolute joint momvements!
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the RAPID Generator class.
        /// </summary>
        public RAPIDGenerator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RAPID Generator class.
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
        /// Initializes a new instance of the RAPDI Generator class by duplicating an existing RAPID Generator instance. 
        /// </summary>
        /// <param name="generator"> The RAPID Generator instance to duplicate. </param>
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
            _firstMovementIsMoveAbsJ = generator.FirstMovementIsMoveAbsJ;
        }


        /// <summary>
        /// Returns an exact duplicate of this RAPID Generator instance.
        /// </summary>
        /// <returns> A deep copy of the RAPID Generator instance. </returns>
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
            _targets.Clear();
            _zoneDatas.Clear();
            _robotTools.Clear();
            _workObjects.Clear();
            _errorText.Clear();

            // Save initial tool and add to used tools
            RobotTool initTool = _robot.Tool.Duplicate();
            _robotTools.Add(_robot.Tool.Name, _robot.Tool);

            // Check if the first movement is an Absolute Joint Movement
            _firstMovementIsMoveAbsJ = CheckFirstMovement();

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
        /// Creates the RAPID system code with as default tool0, wobj0 and load0 if the system module name is equal to BASE.
        /// It adds the robot tools and work objects that are collected by this RAPID generator. 
        /// For this you have to call the CreateProgamCode first. 
        /// This method also overwrites or creates a file if saved to file is set equal to true.
        /// </summary>
        /// <param name="customCode"> Custom user definied base code as list with strings. </param>
        /// <returns> Returns the RAPID system code as a string. </returns>
        public string CreateSystemCode(List<string> customCode)
        {
            List<RobotTool> robotTools = new List<RobotTool>();
            List<WorkObject> workObjects = new List<WorkObject>();

            foreach (KeyValuePair<string, RobotTool> entry in _robotTools)
            {
                if (entry.Value.Name != "tool0" & entry.Value.Name != "" & entry.Value.Name != null)
                {
                    robotTools.Add(entry.Value);
                }
            }

            foreach (KeyValuePair<string, WorkObject> entry in _workObjects)
            {
                if (entry.Value.Name != "wobj0" & entry.Value.Name != "" & entry.Value.Name != null)
                {
                    workObjects.Add(entry.Value);
                }
            }

            return CreateSystemCode(robotTools, workObjects, customCode);
        }

        /// <summary>
        /// Creates the RAPID system code with as default tool0, wobj0 and load0 if the system module name is equal to BASE.
        /// It is adds the robot tools, work objects and custom code lines from the given lists. 
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
            systemCode += "\t" + "! ***********************************************";
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
            _firstMovementIsMoveAbsJ = false;

            for (int i = 0; i != _actions.Count; i++)
            {
                if (_actions[i] is Movement movement)
                {
                    if (movement.MovementType == MovementType.MoveAbsJ)
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
        /// Gets a value indicating whether or not the object is valid.
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
        /// Gets or sets the Actions. 
        /// </summary>
        public List<Action> Actions
        {
            get { return _actions; }
            set { _actions = value; }
        }

        /// <summary>
        /// Gets or sets the file path for saving the program and system module.
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the program and system module should be saved to a file.
        /// </summary>
        public bool SaveToFile
        {
            get { return _saveToFile; }
            set { _saveToFile = value; }
        }

        /// <summary>
        /// Gest he RAPID code of the program module.
        /// </summary>
        public string ProgramCode
        {
            get { return _programCode; }
        }

        /// <summary>
        /// Gets the RAPID code of the system module.
        /// </summary>
        public string SystemCode
        {
            get { return _systemCode; }
        }

        /// <summary>
        /// Gets or sets the Robot. 
        /// </summary>
        public Robot Robot
        {
            get { return _robot; }
            set { _robot = value; }
        }

        /// <summary>
        /// Gets or sets the name of the RAPID program module.
        /// </summary>
        public string ProgramModuleName
        {
            get { return _programModuleName; }
            set { _programModuleName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the RAPID system module.
        /// </summary>
        public string SystemModuleName
        {
            get { return _systemModuleName; }
            set { _systemModuleName = value; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the first movement is an Absolute Joint Movement.
        /// </summary>
        public bool FirstMovementIsMoveAbsJ
        {
            get { return _firstMovementIsMoveAbsJ; }
        }

        /// <summary>
        /// Gets the collection with unique Speed Datas used to create the RAPID program module. 
        /// </summary>
        public Dictionary<string, SpeedData> SpeedDatas
        {
            get { return _speedDatas; }
        }


        /// <summary>
        /// Gets the collection with unique Zone Datas used to create the RAPID program module. 
        /// </summary>
        public Dictionary<string, ZoneData> ZoneDatas
        {
            get { return _zoneDatas; }
        }

        /// <summary>
        /// Gets the collection with unique Targets used to create the RAPID program module. 
        /// </summary>
        public Dictionary<string, ITarget> Targets
        {
            get { return _targets; }
        }

        /// <summary>
        /// Gets the collection with unique Robot Tools used to create the RAPID program module. 
        /// </summary>
        public Dictionary<string, RobotTool> RobotTools
        {
            get { return _robotTools; }
        }

        /// <summary>
        /// Gets the collection with unique Work Objects used to create the RAPID program module. 
        /// </summary>
        public Dictionary<string, WorkObject> WorkObjects
        {
            get { return _workObjects; }
        }
      
        /// <summary>
        /// Gets the mutable Stringbuilder used to generate to the RAPID program module. 
        /// </summary>
        public StringBuilder StringBuilder
        {
            get { return _stringBuilder; }
        }

        /// <summary>
        /// Gets the collected error messages. 
        /// </summary>
        public List<string> ErrorText
        {
            get { return _errorText; }
        }
        #endregion
    }
}
