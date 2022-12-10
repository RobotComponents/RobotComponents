// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.IO;
using System.Collections.Generic;
// RobotComponents Libs
using RobotComponents.ABB.Utils;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;

namespace RobotComponents.ABB.Actions
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
        private readonly Dictionary<string, IJointPosition> _jointPositions = new Dictionary<string, IJointPosition>(); // Dictionary that stores all the unique joint positions used by the RAPIDGenerator
        private readonly Dictionary<string, ITarget> _targets = new Dictionary<string, ITarget>(); // Dictionary that stores all the unique targets used by the RAPIDGenerator
        private readonly Dictionary<string, RobotTool> _robotTools = new Dictionary<string, RobotTool>(); // Dictionary that stores all the unique robo ttools used by the RAPIDGenerator
        private readonly Dictionary<string, WorkObject> _workObjects = new Dictionary<string, WorkObject>(); // Dictionary that stores all the unique work objects used by the RAPIDGenerator
        private readonly Dictionary<string, TaskList> _taskLists = new Dictionary<string, TaskList>(); // Dictionary that stores all the unique task lists used by the RAPIDGenerator
        private readonly Dictionary<string, ISyncident> _syncidents = new Dictionary<string, ISyncident>(); // Dictionary that stores all the unique sync ids used by the RAPIDGenerator
        private string _filePath; // File path to save the code
        private bool _saveToFile; // Bool that indicates if the files should be saved
        private string _programModuleName; // The module name of the rapid program code
        private string _systemModuleName; // The module name of the rapid system code
        private string _procedureName; // The name of the rapid procedure
        private bool _firstMovementIsMoveAbsJ; // Bool that indicates if the first movemement is an absolute joint movement
        private readonly List<string> _programModule = new List<string>(); // Program module as a list with code lines
        private readonly List<string> _programDeclarations = new List<string>(); // List with RAPID code declarations
        private readonly List<string> _programDeclarationComments = new List<string>(); // List with RAPID code declarations
        private readonly List<string> _programDeclarationCustomCodeLines = new List<string>(); // List with RAPID code declarations
        private readonly List<string> _programDeclarationsMultiMove = new List<string>(); // List with multi move RAPID code declarations
        private readonly List<string> _programInstructions = new List<string>(); // List with RAPID code instructions
        private readonly List<string> _systemModule = new List<string>(); // System module as a list with code lines
        private readonly List<string> _errorText = new List<string>(); // List with collected error messages: for now only checking for absolute joint momvements!
        private bool _synchronizedMovements = false; // Indicates if the movements are synchronized
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the RAPID Generator class.
        /// </summary>
        public RAPIDGenerator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RAPID Generator class with a main procedure.
        /// </summary>
        /// <param name="robot"> The robot info wherefore the code should be created. </param>
        /// <param name="actions"> The list with robot actions wherefore the code should be created. </param>
        public RAPIDGenerator(Robot robot, IList<Action> actions)
        {
            _robot = robot.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
            _actions = new List<Action>(actions);
            _programModuleName = "MainModule";
            _systemModuleName = "BASE";
            _procedureName = "main";
            _filePath = "";
            _saveToFile = false;
        }

        /// <summary>
        /// Initializes a new instance of the RAPID Generator class with a main procedure.
        /// </summary>
        /// <param name="robot"> The robot info wherefore the code should be created. </param>
        /// <param name="actions"> The list with robot actions wherefore the code should be created. </param>
        /// <param name="programModuleName"> The name of the program module </param>
        /// <param name="systemModuleName"> The name of the system module </param>
        /// <param name="procedureName"> The name of the RAPID procedure </param>
        public RAPIDGenerator(Robot robot, IList<Action> actions, string programModuleName, string systemModuleName, string procedureName)
        {
            _robot = robot.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
            _actions = new List<Action>(actions);
            _programModuleName = programModuleName;
            _systemModuleName = systemModuleName;
            _procedureName = procedureName;
            _filePath = "";
            _saveToFile = false;
        }

        /// <summary>
        /// Initializes a new instance of the RAPID Generator class with a main procedure.
        /// </summary>
        /// <param name="robot"> The robot info wherefore the code should be created. </param>
        /// <param name="actions"> The list with robot actions wherefore the code should be created. </param>
        /// <param name="programModuleName"> The name of the program module </param>
        /// <param name="systemModuleName"> The name of the system module </param>
        /// <param name="procedureName"> The name of the RAPID procedure </param>
        /// <param name="filePath"> The path where the code files should be saved. </param>
        /// <param name="saveToFile"> A boolean that indicates if the file should be saved. </param>
        public RAPIDGenerator(Robot robot, IList<Action> actions, string programModuleName, string systemModuleName, string procedureName, string filePath, bool saveToFile)
        {
            _robot = robot.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
            _actions = new List<Action>(actions);
            _programModuleName = programModuleName;
            _systemModuleName = systemModuleName;
            _procedureName = procedureName;
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
            _procedureName = generator.ProcedureName;
            _robot = generator.Robot.Duplicate();
            _actions = generator.Actions.ConvertAll(action => action.DuplicateAction());
            _filePath = generator.FilePath;
            _saveToFile = generator.SaveToFile;
            _programModule = generator.ProgramModule.ConvertAll(line => line);
            _systemModule = generator.SystemModule.ConvertAll(line => line);
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
            if (!IsValid)
            {
                return "Invalid RAPID Generator";
            }
            else
            {
                return "RAPID Generator";
            }
        }

        /// <summary>
        /// Returns the RAPID program code.
        /// This method also overwrites or creates a file if the property 'SaveToFile is set equal to true.
        /// </summary>
        /// <returns> The RAPID program code as a list with code lines. </returns>
        public List<string> CreateProgramModule()
        {
            // Reset fields
            _programModule.Clear();
            _programDeclarations.Clear();
            _programDeclarationComments.Clear();
            _programDeclarationCustomCodeLines.Clear();
            _programDeclarationsMultiMove.Clear();
            _programInstructions.Clear();
            _speedDatas.Clear();
            _jointPositions.Clear();
            _targets.Clear();
            _zoneDatas.Clear();
            _robotTools.Clear();
            _workObjects.Clear();
            _taskLists.Clear();
            _syncidents.Clear();
            _errorText.Clear();

            // Save initial tool and add to used tools
            RobotTool initTool = _robot.Tool.Duplicate();
            _robotTools.Add(_robot.Tool.Name, _robot.Tool);

            // Check if the first movement is an Absolute Joint Movement
            _firstMovementIsMoveAbsJ = CheckFirstMovement(_actions);

            // Creates Main Module
            _programModule.Add($"MODULE {_programModuleName}");
            _programModule.Add("    ");

            // Add comment lines for tracking which version of RC was used
            _programModule.Add("    " + $"! This RAPID code was generated with RobotComponents v{VersionNumbering.CurrentVersion} (GPL v3)");
            _programModule.Add("    " + "! Visit www.github.com/RobotComponents for more information");
            _programModule.Add("    ");

            // Creates declarations
            for (int i = 0; i != _actions.Count; i++)
            {
                _actions[i].ToRAPIDDeclaration(this);
            }

            // Comments
            if (_programDeclarationComments.Count != 0)
            {
                _programModule.AddRange(_programDeclarationComments);
                _programModule.Add("    ");
            }

            // Custom Code Lines
            if (_programDeclarationCustomCodeLines.Count != 0)
            {
                _programModule.Add("    " + "! User definied code lines");
                _programModule.AddRange(_programDeclarationCustomCodeLines);
                _programModule.Add("    ");
            }

            // Multi move declarations
            if (_programDeclarationsMultiMove.Count != 0)
            {
                _programModule.Add("    " + "! Declarations for multi move programming");
                _programModule.AddRange(_programDeclarationsMultiMove);
                _programModule.Add("    ");
            }

            // Multi move declarations on top
            if (_programDeclarations.Count != 0)
            {
                _programDeclarations.Sort();
                _programModule.Add("    " + "! Declarations generated by Robot Components");
                _programModule.AddRange(_programDeclarations);
                _programModule.Add("    ");
            }

            // Multi move programming
            _synchronizedMovements = false;
            int syncID = 10;

            // Set back initial tool
            _robot.Tool = initTool;

            // Creates instructions
            for (int i = 0; i != _actions.Count; i++)
            {
                if (_synchronizedMovements == true && _actions[i] is Movement movement)
                {
                    movement.SyncID = syncID;
                    _actions[i].ToRAPIDInstruction(this);
                    movement.SyncID = -1;
                    syncID += 10;
                }
                else
                {
                    _actions[i].ToRAPIDInstruction(this);
                }
            }

            if (_programInstructions.Count != 0)
            {
                // Create Program
                _programModule.Add("    " + $"PROC {_procedureName}()");

                // Add instructions
                _programModule.AddRange(_programInstructions);

                // Closes Program
                _programModule.Add("    " + "ENDPROC");
                _programModule.Add("    ");
            }

            // Closes Module
            _programModule.Add("ENDMODULE");

            #region For v2.0: Adds the tooldata and wobjdata to the program module (remove SYS module)
            bool addTooldata = false;
            bool addWobjdata = false;

            // Add data
            int index = 5;

            if (addTooldata == true)
            {
                // Create tooldata
                List<string> tooldata = new List<string>();

                foreach (KeyValuePair<string, RobotTool> entry in _robotTools)
                {
                    if (entry.Value.Name != "tool0" && entry.Value.Name != null && entry.Value.Name != "")
                    {
                        tooldata.Add("    " + $"{entry.Value.ToRAPIDDeclaration()}");
                    }
                }

                if (tooldata.Count != 0)
                {
                    _programModule.Insert(index, "    " + "! User defined tooldata");
                    index += 1;
                    _programModule.InsertRange(index, tooldata);
                    index += tooldata.Count;
                    _programModule.Insert(index, "    ");
                    index += 1;
                }
            }

            if (addWobjdata == true)
            {
                // Create wobjdata
                List<string> wobjdata = new List<string>();

                foreach (KeyValuePair<string, WorkObject> entry in _workObjects)
                {
                    if (entry.Value.Name != "wobj0" && entry.Value.Name != null && entry.Value.Name != "")
                    {
                        wobjdata.Add("    " + $"{entry.Value.ToRAPIDDeclaration()}");
                    }
                }

                if (wobjdata.Count != 0)
                {
                    _programModule.Insert(index, "    " + "! User defined wobjdata");
                    index += 1;
                    _programModule.InsertRange(index, wobjdata);
                    index += wobjdata.Count;
                    _programModule.Insert(index, "    ");
                }
            }

            #endregion

            // Write to file
            if (_saveToFile == true)
            {
                WriteProgramCodeToFile();
            }

            // Return
            return _programModule;
        }

        /// <summary>
        /// Returns the RAPID system code with as default tool0, wobj0 and load0 if the system module name is equal to BASE.
        /// It adds the robot tools and work objects that are collected by this RAPID generator. 
        /// For this you have to call the methode 'CreateProgamCode' first. 
        /// This method also overwrites or creates a file if the property 'SaveToFile' is set equal to true.
        /// </summary>
        /// <param name="customCode"> Custom user definied base code as a list with strings. </param>
        /// <returns> The RAPID system code as a list with code lines. </returns>
        public List<string> CreateSystemModule(IList<string> customCode = null)
        {
            _systemModule.Clear();

            List<RobotTool> robotTools = new List<RobotTool>();
            List<WorkObject> workObjects = new List<WorkObject>();

            foreach (KeyValuePair<string, RobotTool> entry in _robotTools)
            {
                if (entry.Value.Name != "tool0" && entry.Value.Name != null && entry.Value.Name != "")
                {
                    robotTools.Add(entry.Value);
                }
            }

            foreach (KeyValuePair<string, WorkObject> entry in _workObjects)
            {
                if (entry.Value.Name != "wobj0" && entry.Value.Name != null && entry.Value.Name != "")
                {
                    workObjects.Add(entry.Value);
                }
            }

            return CreateSystemModule(robotTools, workObjects, customCode);
        }

        /// <summary>
        /// Returns the RAPID system code with as default tool0, wobj0 and load0 if the system module name is equal to BASE.
        /// It is adds the robot tools, work objects and custom code lines from the given lists. 
        /// This method also overwrites or creates a file if the property 'SaveToFile' is set equal to true.
        /// </summary>
        /// <param name="robotTools"> The robot tools that should be added to the system code as a list. </param>
        /// <param name="workObjects"> The work objects that should be added to the system code as a list. </param>
        /// <param name="customCode"> Custom user definied base code as list with strings. </param>
        /// <returns> The RAPID system code as a list with code lines. </returns>
        public List<string> CreateSystemModule(IList<RobotTool> robotTools, IList<WorkObject> workObjects, IList<string> customCode = null)
        {
            _systemModule.Clear();

            // First line
            if (_systemModuleName == "BASE")
            {
                _systemModule.Add("MODULE BASE (SYSMODULE, NOSTEPIN, VIEWONLY)");
                _systemModule.Add("    "); 
            }
            else
            {
                _systemModule.Add($"MODULE {_systemModuleName} (SYSMODULE)");
                _systemModule.Add("    ");
            }

            // Version number
            _systemModule.Add("    " + $"! This RAPID code was generated with RobotComponents v{VersionNumbering.CurrentVersion} (GPL v3)");
            _systemModule.Add("    " + "! Visit www.github.com/RobotComponents for more information");
            _systemModule.Add("    ");

            // Creates Comments
            _systemModule.Add("    " + "! System module with basic predefined system data");
            _systemModule.Add("    " + "! ***********************************************");
            _systemModule.Add("    ");

            // Creates Predefined System Data: only if it is the BASE module
            if (_systemModuleName == "BASE")
            {
                _systemModule.Add("    " + "! System data tool0, wobj0 and load0");
                _systemModule.Add("    " + "! Do not translate or delete tool0, wobj0, load0");

                // Creates Predefined System Data
                _systemModule.Add("    " + "PERS tooldata tool0 := [TRUE, [[0, 0, 0], [1, 0, 0, 0]], [0.001, [0, 0, 0.001], [1, 0, 0, 0], 0, 0, 0]];");
                _systemModule.Add("    " + "PERS wobjdata wobj0 := [FALSE, TRUE, \"\" , [[0, 0, 0], [1, 0, 0, 0]], [[0, 0, 0], [1, 0, 0, 0]]];");
                _systemModule.Add("    " + "PERS loaddata load0 := [0.001, [0, 0, 0.001], [1, 0, 0, 0], 0, 0, 0];");
                _systemModule.Add("    ");
            }

            // Adds Tools Base Code
            if (robotTools != null && robotTools.Count != 0)
            {
                _systemModule.Add("    " + "! User defined tooldata");
                _systemModule.AddRange(CreateToolSystemCode(robotTools));
                _systemModule.Add("    ");
            }

            // Adds Work Objects Base Code
            if (workObjects != null && workObjects.Count != 0)
            {
                _systemModule.Add("    " + "! User defined wobjdata");
                _systemModule.AddRange(CreateWorkObjectSystemCode(workObjects));
                _systemModule.Add("    ");
            }

            // Adds Custom code line
            if (customCode != null && customCode.Count != 0)
            {
                _systemModule.Add("    " + "! User definied custom code lines");

                for (int i = 0; i != customCode.Count; i++)
                {
                    _systemModule.Add("    " + $"{customCode[i]}");
                }

                _systemModule.Add("    ");
            }

            // End Module
            _systemModule.Add("ENDMODULE");

            // Write to file
            if (_saveToFile == true)
            {
                WriteSystemCodeToFile();
            }

            // Return
            return _systemModule;
        }

        /// <summary>
        /// Returns the System Code for the given Robot Tools.
        /// </summary>
        /// <param name="robotTools"> The Robot Tools. </param>
        /// <returns> The robot tool system code as a list with code lines. </returns>
        private List<string> CreateToolSystemCode(IList<RobotTool> robotTools)
        {
            List<string> result = new List<string>() { };

            for (int i = 0; i != robotTools.Count; i++)
            {
                result.Add("    " + $"{robotTools[i].ToRAPIDDeclaration()}");
            }

            return result;
        }

        /// <summary>
        /// Returns the System Code for all given Work Objects.
        /// </summary>
        /// <param name="workObjects"> The Work Objects. </param>
        /// <returns> The Work Object system code as a list with code lines. </returns>
        private List<string> CreateWorkObjectSystemCode(IList<WorkObject> workObjects)
        {
            List<string> result = new List<string>() { };

            for (int i = 0; i != workObjects.Count; i++)
            {
                result.Add("    " + $"{workObjects[i].ToRAPIDDeclaration()}");
            }

            return result;
        }

        /// <summary>
        /// Writes the RAPID program code to a file if a file path is set.
        /// </summary>
        public void WriteProgramCodeToFile()
        {
            if (_filePath != null && _filePath != "" && _filePath != "null")
            {
                using (StreamWriter writer = new StreamWriter($"{_filePath}\\{_programModuleName}.mod", false))
                {
                    for (int i = 0; i != _programModule.Count; i++)
                    {
                        writer.WriteLine(_programModule[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Writes the RAPID system code to a file if a file path is set.
        /// </summary>
        public void WriteSystemCodeToFile()
        {
            if (_filePath != null && _filePath != "" && _filePath != "null")
            {
                using (StreamWriter writer = new StreamWriter($"{_filePath}\\{_systemModuleName}.sys", false))
                {
                    for (int i = 0; i != _systemModule.Count; i++)
                    {
                        writer.WriteLine(_systemModule[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the first movement type is an absolute joint movement.
        /// </summary>
        /// <returns> Specifies whether the first movement type is an absolute joint movement. </returns>
        private bool CheckFirstMovement(IList<Action> actions)
        {
            List<Action> ungrouped = new List<Action>() { };

            for (int i = 0; i != actions.Count; i++)
            {
                if (actions[i] is ActionGroup group)
                {
                    ungrouped.AddRange(group.Ungroup());
                }
                else
                {
                    ungrouped.Add(actions[i]);
                }
            }

            for (int i = 0; i != ungrouped.Count; i++)
            {
                if (ungrouped[i] is Movement movement)
                {
                    if (movement.MovementType == MovementType.MoveAbsJ)
                    {
                        return true;
                    }
                    else
                    {
                        _errorText.Add("The first movement is not set as an absolute joint movement.");
                        return false;
                    }
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
                if (_actions == null) { return false; }
                if (_robot == null) { return false; }
                if (_robot.IsValid == false) { return false; }
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
        /// Gest he RAPID code of the program module as a list with code lines.
        /// </summary>
        public List<string> ProgramModule
        {
            get { return _programModule; }
        }

        /// <summary>
        /// Gets the RAPID code of the system module as a list with code lines.
        /// </summary>
        public List<string> SystemModule
        {
            get { return _systemModule; }
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
        /// Gets or sets the name of the RAPID procedure.
        /// </summary>
        public string ProcedureName
        {
            get { return _procedureName; }
            set { _procedureName = value; }
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
        /// Gets the collection with unique Joint Positions used to create the RAPID program module. 
        /// </summary>
        public Dictionary<string, IJointPosition> JointPositions
        {
            get { return _jointPositions; }
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
        /// Gets the collection with unique Task Lists used to create the RAPID program module. 
        /// </summary>
        public Dictionary<string, TaskList> TaskLists
        {
            get { return _taskLists; }
        }

        /// <summary>
        /// Gets the collection with unique syncidents used to create the RAPID program module. 
        /// </summary>
        public Dictionary<string, ISyncident> Syncidents
        {
            get { return _syncidents; }
        }

        /// <summary>
        /// Gets the collected error messages. 
        /// </summary>
        public List<string> ErrorText
        {
            get { return _errorText; }
        }

        /// <summary>
        /// Gets the program declarations as list with RAPID code lines.
        /// </summary>
        public List<string> ProgramDeclarations
        {
            get { return _programDeclarations; }
        }

        /// <summary>
        /// Gets the program declarations commments as list with RAPID code lines.
        /// </summary>
        public List<string> ProgramDeclarationComments
        {
            get { return _programDeclarationComments; }
        }

        /// <summary>
        /// Gets the program declarations custom code lines as list with RAPID code lines.
        /// </summary>
        public List<string> ProgramDeclarationCustomCodeLines
        {
            get { return _programDeclarationCustomCodeLines; }
        }

        /// <summary>
        /// Gets the program declarations for multi move programming as a list with RAPID code lines.
        /// </summary>
        public List<string> ProgramDeclarationsMultiMove
        {
            get { return _programDeclarationsMultiMove; }
        }

        /// <summary>
        /// Gets the program instructions as a list with RAPID code lines.
        /// </summary>
        public List<string> ProgramInstructions
        {
            get { return _programInstructions; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the movements are synchronized. 
        /// </summary>
        public bool SynchronizedMovements
        {
            get { return _synchronizedMovements; }
            set { _synchronizedMovements = value; }
        }
        #endregion

        #region OBSOLETE
        /// <summary>
        /// Initializes a new instance of the RAPID Generator class with a main procedure.
        /// </summary>
        /// <param name="programModuleName"> The name of the program module </param>
        /// <param name="systemModuleName"> The name of the system module </param>
        /// <param name="actions"> The list with robot actions wherefore the code should be created. </param>
        /// <param name="filePath"> The path where the code files should be saved. </param>
        /// <param name="saveToFile"> A boolean that indicates if the file should be saved. </param>
        /// <param name="robot"> The robot info wherefore the code should be created. </param>
        [Obsolete("This constructor is OBSOLETE and will be removed in version 2.", false)]
        public RAPIDGenerator(string programModuleName, string systemModuleName, IList<Action> actions, string filePath, bool saveToFile, Robot robot)
        {
            _programModuleName = programModuleName;
            _systemModuleName = systemModuleName;
            _procedureName = "main";
            _robot = robot.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
            _actions = new List<Action>(actions);
            _filePath = filePath;
            _saveToFile = saveToFile;
        }
        #endregion
    }
}
