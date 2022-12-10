// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Goos;
using RobotComponents.ABB.Gh.Utils;
using RobotComponents.ABB.Gh.Forms;
// ABB Robotics Libs
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;

namespace RobotComponents.ABB.Gh.Components.ControllerUtility
{
    /// <summary>
    /// RobotComponents Controller Utility : Setup a remote connection. An inherent from the GH_Component Class.
    /// </summary>
    public class RemoteConnectionComponent : GH_Component
    {
        #region fields  
        private Controller _controller;
        private bool _ctr = true;
        private string _msg;
        private string _cStatus = "Not connected.";
        private string _uStatus = "No actions.";
        private int _count = 0;
        private bool _programPointerWarning = false;
        private static Task[] _tasks = new Task[0];
        private Task _task;
        private int _pickedIndex = -1;
        private bool _fromMenu = false;
        #endregion

        /// <summary>
        /// Initializes a new instance of the RemoteConnection class.
        /// </summary>
        public RemoteConnectionComponent()
          : base("Remote Connection", "Remote Connection",
              "Establishes a remote connection with the controller to upload an run RAPID code directly on a virtual or real ABB IRC5 robot controller."
              + System.Environment.NewLine + System.Environment.NewLine +
              "Robot Components: v" + RobotComponents.ABB.Utils.VersionNumbering.CurrentVersion,
              "Robot Components", "Controller Utility")
        {
            this.Message = "-";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot Controller", "RC", "Robot Controller to connect to as Robot Controller", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Connect", "C", "Create an online connection with the Robot Controller as bool", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Upload", "U", "Upload the RAPID code to the Robot as bool", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Run", "R", "Run as bool", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Stop", "S", "Stop/Pause as bool", GH_ParamAccess.item, false);
            pManager.AddTextParameter("Program Module", "PM", "Program Module code as a list with code lines", GH_ParamAccess.list);
            pManager.AddTextParameter("System Module", "SM", "System Module code as as list with code lines", GH_ParamAccess.list);

            pManager[5].Optional = true;
            pManager[6].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "S", "Status of the ABB IRC5 robot controller", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            GH_Controller controllerGoo = null;
            bool connect = false;
            bool upload = false;
            bool run = false;
            bool stop = false;
            List<string> programCode = new List<string>();
            List<string> systemCode= new List<string>();

            // Catch input data
            if (!DA.GetData(0, ref controllerGoo)) { return; }
            if (!DA.GetData(1, ref connect)) { return; }
            if (!DA.GetData(2, ref upload)) { return; }
            if (!DA.GetData(3, ref run)) { return; }
            if (!DA.GetData(4, ref stop)) { return; }
            if (!DA.GetDataList(5, programCode)) { programCode = new List<string>() { }; }
            if (!DA.GetDataList(6, systemCode)) { systemCode = new List<string>() { }; }
            base.DestroyIconCache();

            // Get controller value
            _controller = controllerGoo.Value;

            // Get task (from serialized index)
            if (_task == null & _pickedIndex >= 0 & connect == true)
            {
                _tasks = _controller.Rapid.GetTasks();

                if (_tasks.Length > _pickedIndex)
                {
                    _task = _tasks[_pickedIndex];
                    this.Message = _task.Name;
                    this.ExpirePreview(true);
                }
            }

            // Pick task from form
            if ((_task == null & connect == true) | _fromMenu)
            {
                _task = GetTask();
                this.Message = _task.Name;
                this.ExpirePreview(true);
            }

            // Connect
            if (connect)
            {
                // Setup the connection
                Connect();

                // Run the program when toggled
                if (run)
                {
                    _uStatus = StartCommand();
                }

                // Stop the program when toggled
                if (stop)
                {
                    _uStatus = StopCommand();
                }

                // Upload the code when toggled
                if (upload)
                {
                    // Reset program pointer warning
                    _programPointerWarning = false;

                    // First stop the current program
                    _uStatus = StopCommand();

                    // Get path for temporary saving of the module files on the local harddrive of the user
                    // NOTE: This is not a path on the controller, but on the pc of the user
                    string localDirectory = Path.Combine(DocumentsFolderPath(), "RobotComponents", "temp");

                    // Check if the directory already exists
                    if (Directory.Exists(localDirectory))
                    {
                        // Delete if it already exists
                        Directory.Delete(localDirectory, true);
                    }

                    // Create new directory
                    Directory.CreateDirectory(localDirectory);

                    // Save the RAPID code to the created directory / local folder
                    SaveModulesToFile(localDirectory, programCode, systemCode);

                    // Directory to save the modules on the controller
                    string controllerDirectory = Path.Combine(_controller.FileSystem.RemoteDirectory, "RAPID");

                    // Module file paths
                    string filePathProgram;
                    string filePathSystem;
                    string directory;

                    // Upload to the real physical controller
                    if (_controller.IsVirtual == false)
                    {
                        _controller.AuthenticationSystem.DemandGrant(Grant.WriteFtp);
                        _controller.FileSystem.PutDirectory(localDirectory, "RAPID", true);
                        directory = controllerDirectory;
                    }
                    // Upload to a virtual controller
                    else
                    {
                        directory = localDirectory;
                    }

                    // The real upload
                    using (Mastership master = Mastership.Request(_controller))
                    {
                        // Grant acces
                        _controller.AuthenticationSystem.DemandGrant(Grant.LoadRapidProgram);

                        // Load the new program from the created file
                        if (systemCode.Count != 0)
                        {
                            filePathSystem = Path.Combine(directory, "SystemModule.sys");
                            _task.LoadModuleFromFile(filePathSystem, RapidLoadMode.Replace);
                        }
                        if (programCode.Count != 0)
                        {
                            filePathProgram = Path.Combine(directory, "ProgramModule.mod");
                            _task.LoadModuleFromFile(filePathProgram, RapidLoadMode.Replace);
                        }

                        // Resets the program pointer of this task to the main entry point.
                        if (_controller.OperatingMode == ControllerOperatingMode.Auto)
                        {
                            _controller.AuthenticationSystem.DemandGrant(Grant.ExecuteRapid);

                            try
                            {
                                _task.ResetProgramPointer(); // Requires auto mode and execute rapid
                                _programPointerWarning = false;
                            }
                            catch
                            {
                                _programPointerWarning = true;
                            }
                        }

                        // Update action status message
                        if (programCode.Count != 0 || systemCode.Count != 0)
                        {
                            _uStatus = "The RAPID code is succesfully uploaded.";
                        }
                        else
                        {
                            _uStatus = "The RAPID is not uploaded since there is no code defined.";
                        }

                        // Give back the mastership
                        master.Release();
                    }

                    // Delete the temporary files
                    if (Directory.Exists(localDirectory))
                    {
                        Directory.Delete(localDirectory, true);
                    }
                }
            }

            // Disconnect
            else
            {
                // Disconnect
                Disconnect();

                // Update the satus message when a command wants to be executed without having a connection.
                if (run || stop || upload)
                {
                    _uStatus = "Please connect first.";
                }
            }

            // Output message
            _msg = $"The remote connection status:\n\nController: {_cStatus}\nActions: {_uStatus}";

            if (_programPointerWarning == true)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The program pointer could not be reset. Check the program modules that are defined" +
                    " in your controller. Probably you defined two main functions or there are other errors in your RAPID code.");
            }

            // Output
            DA.SetData(0, _msg);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                if (_ctr)
                {
                    return Properties.Resources.Remote_ON_Icon;
                }
                else
                {
                    return Properties.Resources.Remote_OFF_Icon;
                }
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8bfb75d4-9122-45a3-9f11-8d01fb7ea069"); }
        }
        #endregion

        #region menu item
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Pick Task", MenuItemClick);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Documentation", MenuItemClickComponentDoc, Properties.Resources.WikiPage_MenuItem_Icon);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Documentation" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickComponentDoc(object sender, EventArgs e)
        {
            string url = Documentation.ComponentWeblinks[this.GetType()];
            Documentation.OpenBrowser(url);
        }

        /// <summary>
        /// Registers the event when the custom menu item is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClick(object sender, EventArgs e)
        {
            _fromMenu = true;
            ExpireSolution(true);
            _fromMenu = false;
        }
        #endregion

        #region pick task
        /// <summary>
        /// Get the task
        /// </summary>
        /// <returns> The picked task. </returns>
        private Task GetTask()
        {
            // Initiate and clear variables
            _tasks = _controller.Rapid.GetTasks();

            // Automatically pick the first task when one task is available. 
            if (_tasks.Length == 1)
            {
                _pickedIndex = 0;
            }

            // Display the form and let the user pick a task when more then one task is available. 
            else if (_tasks.Length > 1)
            {
                // Display the form and return the index of the picked controller. 
                _pickedIndex = DisplayForm(_tasks);

                // Return a null value when the picked index is incorrect. 
                if (_pickedIndex < 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No task picked from menu!");
                    return null;
                }
            }

            // Return a null value when no task was found
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No task found!");
                return null;
            }

            // Select the picked task
            return _tasks[_pickedIndex];
        }

        /// <summary>
        /// This method displays the form and returns the index number of the picked task.
        /// </summary>
        /// <param name="tasks"> A list with task names. </param>
        /// <returns> The index number of the picked task. </returns>
        private int DisplayForm(Task[] tasks)
        {
            // Create the form with all the available task names
            PickTaskForm frm = new PickTaskForm(tasks.ToList().ConvertAll(item => item.Name));

            // Display the form
            Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnEditor(frm, false);
            frm.ShowDialog();

            // Return the index number of the picked task
            return PickTaskForm.TaskIndex;
        }

        /// <summary>
        /// List with all the ABB tasks in the controller
        /// </summary>
        public static Task[] Tasks
        {
            get { return _tasks; }
        }
        #endregion

        #region serialization
        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            byte[] array = RobotComponents.ABB.Utils.HelperMethods.ObjectToByteArray(_pickedIndex);
            writer.SetByteArray("Picked Task Index", array);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            try
            { 
                byte[] array = reader.GetByteArray("Picked Task Index");
                _pickedIndex = (int)RobotComponents.ABB.Utils.HelperMethods.ByteArrayToObject(array);
            }
            catch 
            { 
                _pickedIndex = -1; 
            }

            return base.Read(reader);
        }
        #endregion

        #region additional methods
        /// <summary>
        /// Method to connect to the controller. 
        /// </summary>
        private void Connect()
        {
            // Log on 
            _controller.Logon(UserInfo.DefaultUser); //TODO: Make user login

            // Update controller status message
            _cStatus = "You are connected.";

            // Update controller connection status
            _ctr = true;

            // Update action status message
            if (_count == 0)
            {
                _uStatus = "All set to go.";
                _count = 1;
            }
        }

        /// <summary>
        /// Method to disconnect the current controller
        /// </summary>
        private void Disconnect()
        {
            // Only disconnect when there is a connection
            if (_controller != null)
            {
                // Logoff
                _controller.Logoff();

                // Set a null controller
                _controller = null;

                // Update controller status message
                _cStatus = "You are disconnected.";

                // Update controller connection status
                _ctr = false;

                // Update action message
                if (_count == 1)
                {
                    _uStatus = "Try to reconnect first.";
                    _count = 0;
                }
            }
        }

        /// <summary>
        /// Method to run the program and returns the action message. 
        /// </summary>
        /// <returns> The action message / status. </returns>
        private string StartCommand()
        {
            // Check the mode of the controller
            if (_controller.OperatingMode != ControllerOperatingMode.Auto)
            {
                return "Controller not set in automatic.";
            }

            // Check if the motors are enabled
            if (_controller.State != ControllerState.MotorsOn)
            {
                return "Motors not on.";
            }

            // Execute the program
            using (Mastership master = Mastership.Request(_controller))
            {
                _controller.Rapid.Start(RegainMode.Continue, ExecutionMode.Continuous, ExecutionCycle.Once, StartCheck.CallChain);

                // Give back the mastership
                master.Release();
            }

            // Return status message
            return "Program started.";
        }

        /// <summary>
        /// Method to stop the program and returns the action message. 
        /// </summary>
        /// <returns> The action message / status. </returns>
        private string StopCommand()
        {
            // Check the mode of the controller
            if (_controller.OperatingMode != ControllerOperatingMode.Auto)
            {
                return "Controller not set in automatic mode.";
            }

            // Stop the program
            using (Mastership master = Mastership.Request(_controller))
            {
                _controller.Rapid.Stop(StopMode.Instruction);

                // Give back the mastership
                master.Release();
            }

            // Return status message
            return "Program stopped.";
        }

        /// <summary>
        /// Gets the local documents folder of the user
        /// </summary>
        private static string DocumentsFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        /// <summary>
        /// Save to the RAPID program and sytem modules to the given file path.
        /// </summary>
        /// <param name="path"> The directory where to save the RAPID modules. </param>
        /// <param name="programModule"> The RAPID program module as a list with code lines. </param>
        /// <param name="systemModule"> The RAPID system module as a list with code lines </param>
        private void SaveModulesToFile(string path, List<string> programModule, List<string> systemModule)
        {
            // Save the program modules
            if (programModule.Count != 0)
            {
                string programFilePath = Path.Combine(path, "ProgramModule.mod");
                using (StreamWriter writer = new StreamWriter(programFilePath, false))
                {
                    for (int i = 0; i < programModule.Count; i++)
                    {
                        writer.WriteLine(programModule[i]);
                    }
                }

            }

            // Save the system module
            if (systemModule.Count != 0)
            {
                string systemFilePath = Path.Combine(path, "SystemModule.sys");
                using (StreamWriter writer = new StreamWriter(systemFilePath, false))
                {
                    for (int i = 0; i < systemModule.Count; i++)
                    {
                        writer.WriteLine(systemModule[i]);
                    }
                }
            }
        }
        #endregion

    }
}