// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Gh.Goos;
using RobotComponents.Gh.Utils;
// ABB Robotics Libs
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;

namespace RobotComponents.Gh.Components.ControllerUtility
{
    /// <summary>
    /// RobotComponents Controller Utility : Setup a remote connection. An inherent from the GH_Component Class.
    /// </summary>
    public class RemoteConnectionComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RemoteConnection class.
        /// </summary>
        public RemoteConnectionComponent()
          : base("Remote Connection", "Remote Connection",
              "Establishes a remote connection with the controller to upload an run RAPID code directly on a virtual or real ABB IRC5 robot controller."
              + System.Environment.NewLine + System.Environment.NewLine +
              "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Controller Utility")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //TODO: Replace generic parameter with an RobotComponents Parameter
            pManager.AddGenericParameter("Robot Controller", "RC", "Robot Controller to connect to as Robot Controller", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Connect", "C", "Create an online connection with the Robot Controller as bool", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Upload", "U", "Upload the RAPID code to the Robot as bool", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run", "R", "Run as bool", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Stop", "S", "Stop/Pause as bool", GH_ParamAccess.item);
            pManager.AddTextParameter("Program Module", "PM", "Program Module code as text", GH_ParamAccess.list);
            pManager.AddTextParameter("System Module", "SM", "System Module code as text", GH_ParamAccess.list);

            pManager[5].Optional = true;
            pManager[6].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "S", "Status of the ABB IRC5 robot controller", GH_ParamAccess.item);
        }

        // Fields
        private Controller _controller;
        private bool _ctr = true;
        private string _msg;
        private string _cStatus = "Not connected.";
        private string _uStatus = "No actions.";
        private int _count = 0;
        private bool _programPointerWarning = false;

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

            // Connect
            if (connect)
            {
                // Setup the connection
                Connect();

                // Run the program when toggled
                if (run)
                {
                    Run();
                }

                // Stop the program when toggled
                if (stop)
                {
                    Stop();
                }

                // Upload the code when toggled
                if (upload)
                {

                    // Reset program pointer warning
                    _programPointerWarning = false;

                    // First stop the current program
                    Stop();

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
                        // Get task
                        Task[] tasks = _controller.Rapid.GetTasks();
                        Task task = tasks[0];

                        // TODO: Make a pick task form? As for pick controller? 
                        // TODO: This can be a solution for multi move with multiple tasks
                        // Task task = controller.Rapid.GetTask(tasks[0].Name) // Get task with specified name

                        // Grant acces
                        _controller.AuthenticationSystem.DemandGrant(Grant.LoadRapidProgram);

                        // Load the new program from the created file
                        for (int i = 0; i < systemCode.Count; i++)
                        {
                            filePathSystem = Path.Combine(directory, "SystemModule_" + i.ToString() + ".sys");
                            task.LoadModuleFromFile(filePathSystem, RapidLoadMode.Replace);
                        }
                        for (int i = 0; i < programCode.Count; i++)
                        {
                            filePathProgram = Path.Combine(directory, "ProgramModule_" + i.ToString() + ".mod");
                            task.LoadModuleFromFile(filePathProgram, RapidLoadMode.Replace);
                        }

                        // Resets the program pointer of this task to the main entry point.
                        if (_controller.OperatingMode == ControllerOperatingMode.Auto)
                        {
                            _controller.AuthenticationSystem.DemandGrant(Grant.ExecuteRapid);

                            try
                            {
                                task.ResetProgramPointer(); // Requires auto mode and execute rapid
                                _programPointerWarning = false;
                            }
                            catch
                            {
                                _programPointerWarning = true;
                            }
                        }

                        // Update action status message
                        if (programCode != null || systemCode != null)
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
                    " in your controller. Probably you defined two main functions or there are other erros in your RAPID code.");
            }

            // Output
            DA.SetData(0, _msg);
        }

        //  Addtional methods
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
        /// Method to start the program.
        /// </summary>
        private void Run()
        {
            _uStatus = StartCommand();
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
                return ("Controller not set in automatic.");
            }

            // Check if the motors are enabled
            if (_controller.State != ControllerState.MotorsOn)
            {
                return ("Motors not on.");
            }

            // Execute the program
            using (Mastership master = Mastership.Request(_controller))
            {
                _controller.Rapid.Start(RegainMode.Continue, ExecutionMode.Continuous, ExecutionCycle.Once, StartCheck.CallChain);

                // Give back the mastership
                master.Release();
            }

            // Return status message
            return ("Program started.");
        }

        /// <summary>
        /// Method to stop the program.
        /// </summary>
        private void Stop()
        {
            _uStatus = StopCommand();
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
        /// <param name="programModules"> The RAPID program module as a list with strings (each complete module is one list item). </param>
        /// <param name="systemModules"> The RAPID system module as a string (each complete module is one list item). </param>
        private void SaveModulesToFile(string path, List<string> programModules, List<string> systemModules)
        {
            // Save the program modules
            for (int i = 0; i < programModules.Count; i++)
            {
                string programFilePath = Path.Combine(path, "ProgramModule_" + i.ToString() + ".mod");
                using (StreamWriter writer = new StreamWriter(programFilePath, false))
                {
                    writer.WriteLine(programModules[i]);
                }
            }

            // Save the system module
            for (int i = 0; i < systemModules.Count; i++)
            {
                string systemFilePath = Path.Combine(path, "SystemModule_" + i.ToString() +".sys");
                using (StreamWriter writer = new StreamWriter(systemFilePath, false))
                {
                    writer.WriteLine(systemModules[i]);
                }
            }
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
        #endregion

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
    }
}