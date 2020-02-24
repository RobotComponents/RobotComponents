// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.IO;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponentsABB.Goos;
// ABB Robotics Libs
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;

namespace RobotComponentsABB.Components.ControllerUtility
{
    /// <summary>
    /// RobotComponents Controller Utility : Setup a remote connection. An inherent from the GH_Component Class.
    /// </summary>
    public class RemoteConnection : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RemoteConnection class.
        /// </summary>
        public RemoteConnection()
          : base("Remote Connection", "Remote Connection",
              "Establishes a remote connection with the controller to upload an run RAPID code directly on a virtual or real ABB robot controller."
              + System.Environment.NewLine +
              "RobotComponents : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
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
            pManager.AddGenericParameter("Robot Controller", "RC", "Controller to be connected to", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Connect", "C", "Create an online connection with the Robot Controller", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Upload", "U", "Upload your RAPID code to the Robot", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run", "R", "Run", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Stop", "S", "Stop/Pause", GH_ParamAccess.item);
            pManager.AddTextParameter("Program Module", "PM", "Insert here the Program module code as a string", GH_ParamAccess.item);
            pManager.AddTextParameter("System Module", "SM", "Insert here the System module code as a string", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "S", "Updates about what is going on here.", GH_ParamAccess.item);
        }

        // Fields
        private Controller _controller;
        private bool _ctr = true;
        private string _msg;
        private string _cStatus = "Not connected.";
        private string _uStatus = "No actions.";
        private int _count = 0;

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
            string RAPID = null;
            string BaseCode= null;

            // Catch input data
            if (!DA.GetData(0, ref controllerGoo)) { return; }
            if (!DA.GetData(1, ref connect)) { return; }
            if (!DA.GetData(2, ref upload)) { return; }
            if (!DA.GetData(3, ref run)) { return; }
            if (!DA.GetData(4, ref stop)) { return; }
            if (!DA.GetData(5, ref RAPID)) { return; }
            if (!DA.GetData(6, ref BaseCode)) { return; }
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
                    SaveModulesToFile(localDirectory, RAPID, BaseCode);

                    // Directory to save the modules on the controller
                    string controllerDirectory = Path.Combine(_controller.FileSystem.RemoteDirectory, "RAPID");

                    // Module file paths
                    string filePathProgram;
                    string filePathSystem;

                    // Upload to the real physical controller
                    if (_controller.IsVirtual == false)
                    {
                        _controller.AuthenticationSystem.DemandGrant(Grant.WriteFtp);
                        _controller.FileSystem.PutDirectory(localDirectory, "RAPID", true);
                        filePathProgram = Path.Combine(controllerDirectory, "ProgramModule.mod");
                        filePathSystem = Path.Combine(controllerDirectory, "SystemModule.sys");
                    }
                    // Upload to a virtual controller
                    else
                    {
                        filePathProgram = Path.Combine(localDirectory, "ProgramModule.mod");
                        filePathSystem = Path.Combine(localDirectory, "SystemModule.sys");
                    }

                    // The real upload
                    using (Mastership master = Mastership.Request(_controller.Rapid))
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
                        task.LoadModuleFromFile(filePathProgram, RapidLoadMode.Replace);
                        task.LoadModuleFromFile(filePathSystem, RapidLoadMode.Replace);

                        // Resets the program pointer of this task to the main entry point.
                        if (_controller.OperatingMode == ControllerOperatingMode.Auto)
                        {
                            _controller.AuthenticationSystem.DemandGrant(Grant.ExecuteRapid);
                            task.ResetProgramPointer(); // Requires auto mode and execute rapid
                        }

                        // Update action status message
                        _uStatus = "The RAPID code is succesfully uploaded.";

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
            using (Mastership master = Mastership.Request(_controller.Rapid))
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
            using (Mastership master = Mastership.Request(_controller.Rapid))
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
        /// <param name="programModule"> The RAPID program module as a string. </param>
        /// <param name="systemModule"> The RAPID system module as a string. </param>
        private void SaveModulesToFile(string path, string programModule, string systemModule)
        {
            // Save the program module
            if (programModule != null)
            {
                string programFilePath = Path.Combine(path, "ProgramModule.mod");
                using (StreamWriter writer = new StreamWriter(programFilePath, false))
                {
                    writer.WriteLine(programModule);
                }
            }

            // Save the system module
            if (systemModule != null)
            {
                string systemFilePath = Path.Combine(path, "SystemModule.sys");
                using (StreamWriter writer = new StreamWriter(systemFilePath, false))
                {
                    writer.WriteLine(systemModule);
                }
            }
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