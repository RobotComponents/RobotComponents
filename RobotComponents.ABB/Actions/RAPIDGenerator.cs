// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.IO;
using System.Collections.Generic;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Actions.Instructions;

namespace RobotComponents.ABB.Actions
{
    /// <summary>
    /// Represents the RAPID Generator.
    /// </summary>
    /// <remarks>
    /// This is class is used to generate the RAPID module from a given set of actions.
    /// </remarks>
    public class RAPIDGenerator
    {
        #region fields
        private Robot _robot;
        private List<Action> _actions = new List<Action>(); 

        // Dictionaries that collect all declarations that have a variable name defined
        private readonly Dictionary<string, SpeedData> _speedDatas = new Dictionary<string, SpeedData>();
        private readonly Dictionary<string, ConfigurationData> _configurationDatas = new Dictionary<string, ConfigurationData>(); 
        private readonly Dictionary<string, ZoneData> _zoneDatas = new Dictionary<string, ZoneData>(); 
        private readonly Dictionary<string, RobotTool> _robotTools = new Dictionary<string, RobotTool>(); 
        private readonly Dictionary<string, LoadData> _loadDatas = new Dictionary<string, LoadData>(); 
        private readonly Dictionary<string, WorkObject> _workObjects = new Dictionary<string, WorkObject>(); 
        private readonly Dictionary<string, TaskList> _taskLists = new Dictionary<string, TaskList>();
        private readonly Dictionary<string, IJointPosition> _jointPositions = new Dictionary<string, IJointPosition>();
        private readonly Dictionary<string, ITarget> _targets = new Dictionary<string, ITarget>();
        private readonly Dictionary<string, ISyncident> _syncidents = new Dictionary<string, ISyncident>();

        // Collections with different types of RAPID code lines
        private readonly List<string> _programDeclarations = new List<string>();
        private readonly List<string> _programDeclarationsLoadData = new List<string>();
        private readonly List<string> _programDeclarationsToolData = new List<string>();
        private readonly List<string> _programDeclarationsWorkObjectData = new List<string>();
        private readonly List<string> _programDeclarationsComments = new List<string>(); 
        private readonly List<string> _programDeclarationsCustom = new List<string>(); 
        private readonly List<string> _programDeclarationsMultiMove = new List<string>();
        private readonly List<string> _programInstructions = new List<string>();

        // The RAPID module
        private readonly List<string> _module = new List<string>();
        private string _moduleName;
        private string _procedureName;

        // Checks
        private readonly List<string> _errorText = new List<string>();
        private bool _isFirstMovementMoveAbsJ;
        private bool _isSynchronized = false;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the RAPID Generator class.
        /// </summary>
        public RAPIDGenerator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RAPID Generator class with a main routine.
        /// </summary>
        /// <param name="robot"> The robot info wherefore the code should be created. </param>
        /// <param name="actions"> The list with robot actions wherefore the code should be created. </param>
        public RAPIDGenerator(Robot robot, IList<Action> actions)
        {
            _robot = robot.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
            _actions = new List<Action>(actions);
            _moduleName = "MainModule";
            _procedureName = "main";
        }

        /// <summary>
        /// Initializes a new instance of the RAPID Generator class with custom names.
        /// </summary>
        /// <param name="robot"> The robot info wherefore the code should be created. </param>
        /// <param name="actions"> The list with robot actions wherefore the code should be created. </param>
        /// <param name="moduleName"> The name of the program module </param>
        /// <param name="routineName"> The name of the RAPID procedure </param>
        public RAPIDGenerator(Robot robot, IList<Action> actions, string moduleName, string routineName)
        {
            _robot = robot.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
            _actions = new List<Action>(actions);
            _moduleName = moduleName;
            _procedureName = routineName;
        }

        /// <summary>
        /// Initializes a new instance of the RAPID Generator class by duplicating an existing RAPID Generator instance. 
        /// </summary>
        /// <param name="generator"> The RAPID Generator instance to duplicate. </param>
        public RAPIDGenerator(RAPIDGenerator generator)
        {
            _module = generator.Module.ConvertAll(line => line);
            _moduleName = generator.ModuleName;
            _procedureName = generator.ProcedureName;
            _robot = generator.Robot.Duplicate();
            _actions = generator.Actions.ConvertAll(action => action.DuplicateAction());
            _isFirstMovementMoveAbsJ = generator.IsFirstMovementMoveAbsJ;

            // OBSOLETE
            _filePath = generator._filePath;
            _saveToFile = generator._saveToFile;
            _systemModule = generator._systemModule.ConvertAll(line => line);
            _systemModuleName = generator._systemModuleName;
        }

        /// <summary>
        /// Returns an exact duplicate of this RAPID Generator instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the RAPID Generator instance. 
        /// </returns>
        public RAPIDGenerator Duplicate()
        {
            return new RAPIDGenerator(this);
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> 
        /// A string that represents the current object. 
        /// </returns>
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
        /// Returns the RAPID module.
        /// </summary>
        /// <param name="addTooldata"> Specifies if the tooldata should be added to the RAPID module. </param>
        /// <param name="addWobjdata"> Specifies if the wobjdata should be added to the RAPID module. </param>
        /// <param name="addLoaddata"> Specifies if the loaddata should be added to the RAPID module. </param>
        /// <returns> 
        /// The RAPID module as a list with code lines.
        /// </returns>
        public List<string> CreateModule(bool addTooldata = true, bool addWobjdata = true, bool addLoaddata = true)
        {
            // Reset the fields            
            _programDeclarations.Clear();
            _programDeclarationsToolData.Clear();
            _programDeclarationsLoadData.Clear();
            _programDeclarationsWorkObjectData.Clear();
            _programDeclarationsComments.Clear();
            _programDeclarationsCustom.Clear();
            _programDeclarationsMultiMove.Clear();
            _programInstructions.Clear();
            
            _configurationDatas.Clear();
            _speedDatas.Clear();
            _jointPositions.Clear();
            _targets.Clear();
            _zoneDatas.Clear();
            _robotTools.Clear();
            _loadDatas.Clear();
            _workObjects.Clear();
            _taskLists.Clear();
            _syncidents.Clear();

            _module.Clear();
            _errorText.Clear();
            _isSynchronized = false;
            _isFirstMovementMoveAbsJ = false;

            #region get data
            // Save initial tool and add to used tools
            RobotTool initTool = _robot.Tool.Duplicate();
            _robotTools.Add(_robot.Tool.Name, _robot.Tool);
            _loadDatas.Add(_robot.Tool.LoadData.Name, _robot.Tool.LoadData);

            // Check if the first movement is an Absolute Joint Movement
            _isFirstMovementMoveAbsJ = CheckFirstMovement(_actions);

            // Creates the declaration code lines
            for (int i = 0; i != _actions.Count; i++)
            {
                _actions[i].ToRAPIDDeclaration(this);
            }

            // Create the instruction code lines
            _robot.Tool = initTool;
            int syncID = 10;

            for (int i = 0; i != _actions.Count; i++)
            {
                if (_isSynchronized == true && _actions[i] is Movement movement)
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
            #endregion

            #region write the module
            _module.Add($"MODULE {_moduleName}");
            _module.Add("    ");
            _module.Add("    " + $"! This RAPID code was generated with RobotComponents v{VersionNumbering.CurrentVersion} (LGPL v3)");
            _module.Add("    " + "! Visit www.github.com/RobotComponents for more information");
            _module.Add("    ");

            // Add the comments
            if (_programDeclarationsComments.Count != 0)
            {
                _module.AddRange(_programDeclarationsComments);
                _module.Add("    ");
            }

            // Add loaddata
            if (addLoaddata == true && _programDeclarationsLoadData.Count != 0)
            {
                _programDeclarationsLoadData.Sort();
                _module.Add("    " + "! User defined loaddata");
                _module.AddRange(_programDeclarationsLoadData);
                _module.Add("    ");
            }

            // Add the tooldata
            if (addTooldata == true && _programDeclarationsToolData.Count != 0)
            {
                _programDeclarationsToolData.Sort();
                _module.Add("    " + "! User defined tooldata");
                _module.AddRange(_programDeclarationsToolData);
                _module.Add("    ");
            }

            // Add the wobjdata
            if (addWobjdata == true && _programDeclarationsWorkObjectData.Count != 0)
            {
                _programDeclarationsWorkObjectData.Sort();
                _module.Add("    " + "! User defined wobjdata");
                _module.AddRange(_programDeclarationsWorkObjectData);
                _module.Add("    ");
            }

            // Add the custom Code Lines
            if (_programDeclarationsCustom.Count != 0)
            {
                _module.Add("    " + "! User definied code lines");
                _module.AddRange(_programDeclarationsCustom);
                _module.Add("    ");
            }

            // Add the multi move declarations
            if (_programDeclarationsMultiMove.Count != 0)
            {
                _programDeclarationsMultiMove.Sort();
                _module.Add("    " + "! Declarations for multi move programming");
                _module.AddRange(_programDeclarationsMultiMove);
                _module.Add("    ");
            }

            // Add all other declarations
            if (_programDeclarations.Count != 0)
            {
                _programDeclarations.Sort();
                _module.Add("    " + "! Declarations generated by Robot Components");
                _module.AddRange(_programDeclarations);
                _module.Add("    ");
            }

            // Add the instructions
            if (_programInstructions.Count != 0)
            {
                // Create Program
                _module.Add("    " + $"PROC {_procedureName}()");

                // Add instructions
                _module.AddRange(_programInstructions);

                // Closes Program
                _module.Add("    " + "ENDPROC");
                _module.Add("    ");
            }

            // Close / end
            _module.Add("ENDMODULE");
            #endregion

            return _module;
        }

        /// <summary>
        /// Writes the RAPID module to a file.
        /// </summary>
        /// <param name="path"> The path. </param>
        /// <returns> 
        /// True on success, false on failure. 
        /// </returns>
        public bool WriteModuleToFile(string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter($"{path}\\{_moduleName}.mod", false))
                {
                    for (int i = 0; i != _module.Count; i++)
                    {
                        writer.WriteLine(_module[i]);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether the first movement type is an absolute joint movement.
        /// </summary>
        /// <remarks>
        /// If the no movements were defined the method returns true.
        /// </remarks>
        /// <param name="actions"> The list with actions to check. </param>
        /// <returns> 
        /// Specifies whether the first movement type is an absolute joint movement. 
        /// </returns>
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
        /// Gets or sets the Robot. 
        /// </summary>
        public Robot Robot
        {
            get { return _robot; }
            set { _robot = value; }
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
        /// Gets the RAPID module as a list with code lines.
        /// </summary>
        public List<string> Module
        {
            get { return _module; }
        }

        /// <summary>
        /// Gets or sets the name of the RAPID module.
        /// </summary>
        public string ModuleName
        {
            get { return _moduleName; }
            set { _moduleName = value; }
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
        /// Gets the collected error messages. 
        /// </summary>
        public List<string> ErrorText
        {
            get { return _errorText; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the first movement is an Absolute Joint Movement.
        /// </summary>
        public bool IsFirstMovementMoveAbsJ
        {
            get { return _isFirstMovementMoveAbsJ; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the movements are synchronized. 
        /// </summary>
        /// <remarks>
        /// Value is set inside the SyncMoveOn and SyncMoveOff instructions.
        /// </remarks>
        public bool IsSynchronized
        {
            get { return _isSynchronized; }
            set { _isSynchronized = value; }
        }

        /// <summary>
        /// Gets the collection with unique Configuration Datas used to create the RAPID program module. 
        /// </summary>
        public Dictionary<string, ConfigurationData> ConfigurationDatas
        {
            get { return _configurationDatas; }
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
        /// Gets the collection with unique Load Datas used to create the RAPID program module. 
        /// </summary>
        public Dictionary<string, LoadData> LoadDatas
        {
            get { return _loadDatas; }
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
            get { return _programDeclarationsComments; }
        }

        /// <summary>
        /// Gets the program declarations custom code lines as list with RAPID code lines.
        /// </summary>
        public List<string> ProgramDeclarationCustomCodeLines
        {
            get { return _programDeclarationsCustom; }
        }

        /// <summary>
        /// Gets the program declarations for multi move programming as a list with RAPID code lines.
        /// </summary>
        public List<string> ProgramDeclarationsMultiMove
        {
            get { return _programDeclarationsMultiMove; }
        }

        /// <summary>
        /// Gets the RAPID tooldata code lines as list with RAPID code lines.
        /// </summary>
        public List<string> ProgramDeclarationsToolData
        {
            get { return _programDeclarationsToolData; }
        }

        /// <summary>
        /// Gets the RAPID loaddata code lines as list with RAPID code lines.
        /// </summary>
        public List<string> ProgramDeclarationsLoadData
        {
            get { return _programDeclarationsLoadData; }
        }

        /// <summary>
        /// Gets the RAPID wobjdata code lines as list with RAPID code lines.
        /// </summary>
        public List<string> ProgramDeclarationsWorkObjectData
        {
            get { return _programDeclarationsWorkObjectData; }
        }

        /// <summary>
        /// Gets the program instructions as a list with RAPID code lines.
        /// </summary>
        public List<string> ProgramInstructions
        {
            get { return _programInstructions; }
        }
        #endregion

        #region OBSOLETE
        private readonly List<string> _systemModule = new List<string>(); // System module as a list with code lines
        private string _filePath = ""; // File path to save the code
        private bool _saveToFile = false; // Bool that indicates if the files should be saved
        private string _systemModuleName = "BASE"; // The module name of the rapid system code

        /// <summary>
        /// Initializes a new instance of the RAPID Generator class with a main procedure.
        /// </summary>
        /// <param name="robot"> The robot info wherefore the code should be created. </param>
        /// <param name="actions"> The list with robot actions wherefore the code should be created. </param>
        /// <param name="programModuleName"> The name of the program module </param>
        /// <param name="systemModuleName"> The name of the system module </param>
        /// <param name="procedureName"> The name of the RAPID procedure </param>
        [Obsolete("This constructor is OBSOLETE and will be removed in v2 or later.", false)]
        public RAPIDGenerator(Robot robot, IList<Action> actions, string programModuleName, string systemModuleName, string procedureName)
        {
            _robot = robot.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
            _actions = new List<Action>(actions);
            _moduleName = programModuleName;
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
        [Obsolete("This constructor is OBSOLETE and will be removed in v2 or later.", false)]
        public RAPIDGenerator(Robot robot, IList<Action> actions, string programModuleName, string systemModuleName, string procedureName, string filePath, bool saveToFile)
        {
            _robot = robot.Duplicate(); // Since we might swap tools and therefore change the robot tool we make a deep copy
            _actions = new List<Action>(actions);
            _moduleName = programModuleName;
            _systemModuleName = systemModuleName;
            _procedureName = procedureName;
            _filePath = filePath;
            _saveToFile = saveToFile;
        }

        /// <summary>
        /// Returns the RAPID program code.
        /// </summary>
        /// <remarks>
        /// This method also overwrites or creates a file if the property 'SaveToFile is set equal to true.
        /// </remarks>
        /// <returns> 
        /// The RAPID program code as a list with code lines. 
        /// </returns>
        [Obsolete("This method is OBSOLETE and will be removed in v2 or later.", false)]
        public List<string> CreateProgramModule()
        {
            // Reset fields
            _module.Clear();
            _programDeclarations.Clear();
            _programDeclarationsComments.Clear();
            _programDeclarationsCustom.Clear();
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
            _isFirstMovementMoveAbsJ = CheckFirstMovement(_actions);

            // Creates Main Module
            _module.Add($"MODULE {_moduleName}");
            _module.Add("    ");

            // Add comment lines for tracking which version of RC was used
            _module.Add("    " + $"! This RAPID code was generated with RobotComponents v{VersionNumbering.CurrentVersion} (LGPL v3)");
            _module.Add("    " + "! Visit www.github.com/RobotComponents for more information");
            _module.Add("    ");

            // Creates declarations
            for (int i = 0; i != _actions.Count; i++)
            {
                _actions[i].ToRAPIDDeclaration(this);
            }

            // Comments
            if (_programDeclarationsComments.Count != 0)
            {
                _module.AddRange(_programDeclarationsComments);
                _module.Add("    ");
            }

            // Custom Code Lines
            if (_programDeclarationsCustom.Count != 0)
            {
                _module.Add("    " + "! User definied code lines");
                _module.AddRange(_programDeclarationsCustom);
                _module.Add("    ");
            }

            // Multi move declarations
            if (_programDeclarationsMultiMove.Count != 0)
            {
                _module.Add("    " + "! Declarations for multi move programming");
                _module.AddRange(_programDeclarationsMultiMove);
                _module.Add("    ");
            }

            // All other declarations
            if (_programDeclarations.Count != 0)
            {
                _programDeclarations.Sort();
                _module.Add("    " + "! Declarations generated by Robot Components");
                _module.AddRange(_programDeclarations);
                _module.Add("    ");
            }

            // Multi move programming
            _isSynchronized = false;
            int syncID = 10;

            // Set back initial tool
            _robot.Tool = initTool;

            // Creates instructions
            for (int i = 0; i != _actions.Count; i++)
            {
                if (_isSynchronized == true && _actions[i] is Movement movement)
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
                _module.Add("    " + $"PROC {_procedureName}()");

                // Add instructions
                _module.AddRange(_programInstructions);

                // Closes Program
                _module.Add("    " + "ENDPROC");
                _module.Add("    ");
            }

            // Closes Module
            _module.Add("ENDMODULE");

            // Write to file
            if (_saveToFile == true)
            {
                WriteProgramCodeToFile();
            }

            // Return
            return _module;
        }

        /// <summary>
        /// Returns the RAPID system code with as default tool0, wobj0 and load0 if the system module name is equal to BASE.
        /// </summary>
        /// <remarks>
        /// It adds the robot tools and work objects that are collected by this RAPID generator. 
        /// For this you have to call the methode 'CreateProgamCode' first. 
        /// This method also overwrites or creates a file if the property 'SaveToFile' is set equal to true.
        /// </remarks>
        /// <param name="customCode"> Custom user definied base code as a list with strings. </param>
        /// <returns> 
        /// The RAPID system code as a list with code lines. 
        /// </returns>
        [Obsolete("This method is OBSOLETE and will be removed in v2 or later.", false)]
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
        /// </summary>
        /// <remarks>
        /// It is adds the robot tools, work objects and custom code lines from the given lists. 
        /// This method also overwrites or creates a file if the property 'SaveToFile' is set equal to true.
        /// </remarks>
        /// <param name="robotTools"> The robot tools that should be added to the system code as a list. </param>
        /// <param name="workObjects"> The work objects that should be added to the system code as a list. </param>
        /// <param name="customCode"> Custom user definied base code as list with strings. </param>
        /// <returns> 
        /// The RAPID system code as a list with code lines. 
        /// </returns>
        [Obsolete("This method is OBSOLETE and will be removed in v2.", false)]
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
            _systemModule.Add("    " + $"! This RAPID code was generated with RobotComponents v{VersionNumbering.CurrentVersion} (LGPL v3)");
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
        /// <returns> 
        /// The robot tool system code as a list with code lines. 
        /// </returns>
        [Obsolete("This method is OBSOLETE and will be removed in v2 or later.", false)]
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
        /// <returns> 
        /// The Work Object system code as a list with code lines. 
        /// </returns>
        [Obsolete("This method is OBSOLETE and will be removed in v2 or later.", false)]
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
        [Obsolete("This method is OBSOLETE and will be removed in v2 or later.", false)]
        public void WriteProgramCodeToFile()
        {
            if (_filePath != null && _filePath != "" && _filePath != "null")
            {
                using (StreamWriter writer = new StreamWriter($"{_filePath}\\{_moduleName}.mod", false))
                {
                    for (int i = 0; i != _module.Count; i++)
                    {
                        writer.WriteLine(_module[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Writes the RAPID system code to a file if a file path is set.
        /// </summary>
        [Obsolete("This method is OBSOLETE and will be removed in v2 or later.", false)]
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
        /// Gets or sets the name of the RAPID program module.
        /// </summary>
        [Obsolete("This property is OBSOLETE and will be removed in v2 or later.", false)]
        public string ProgramModuleName
        {
            get { return _moduleName; }
            set { _moduleName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the RAPID system module.
        /// </summary>
        [Obsolete("This property is OBSOLETE and will be removed in v2 or later.", false)]
        public string SystemModuleName
        {
            get { return _systemModuleName; }
            set { _systemModuleName = value; }
        }

        /// <summary>
        /// Gest he RAPID code of the program module as a list with code lines.
        /// </summary>
        [Obsolete("This property is OBSOLETE and will be removed in v2 or later.", false)]
        public List<string> ProgramModule
        {
            get { return _module; }
        }

        /// <summary>
        /// Gets the RAPID code of the system module as a list with code lines.
        /// </summary>
        [Obsolete("This property is OBSOLETE and will be removed in v2 or later.", false)]
        public List<string> SystemModule
        {
            get { return _systemModule; }
        }

        /// <summary>
        /// Gets or sets the file path for saving the program and system module.
        /// </summary>
        [Obsolete("This property is OBSOLETE and will be removed in v2 or later.", false)]
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the program and system module should be saved to a file.
        /// </summary>
        [Obsolete("This property is OBSOLETE and will be removed in v2 or later.", false)]
        public bool SaveToFile
        {
            get { return _saveToFile; }
            set { _saveToFile = value; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the first movement is an Absolute Joint Movement.
        /// </summary>
        [Obsolete("This property is OBSOLETE and will be removed in v3. Use IsFirstMovementMoveAbsJ instead.", false)]
        public bool FirstMovementIsMoveAbsJ
        {
            get { return _isFirstMovementMoveAbsJ; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the movements are synchronized. 
        /// </summary>
        /// <remarks>
        /// Value is set inside the SyncMoveOn and SyncMoveOff instructions.
        /// </remarks>
        [Obsolete("This property is OBSOLETE and will be removed in v3. Use IsSynchronized instead.", false)]
        public bool SynchronizedMovements
        {
            get { return _isSynchronized; }
            set { _isSynchronized = value; }
        }

        /// <summary>
        /// Gets the RAPID tooldata code lines.
        /// </summary>
        [Obsolete("This property is OBSOLETE and will be removed in v3. Use ProgramDeclarationsToolData instead.", false)]
        public List<string> Tooldata
        {
            get { return _programDeclarationsToolData; }
        }

        /// <summary>
        /// Gets the RAPID wobjdata code lines.
        /// </summary>
        [Obsolete("This property is OBSOLETE and will be removed in v3. Use ProgramDeclarationsWorkObjectData instead.", false)]
        public List<string> Wobjdata
        {
            get { return _programDeclarationsWorkObjectData; }
        }
        #endregion
    }
}
