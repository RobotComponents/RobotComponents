﻿using System;
using System.IO;
using System.Linq;
using System.Xml;
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
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // To do: replace generic parameter with an RobotComponents Parameter
            pManager.AddGenericParameter("Robot Controller", "RC", "Controller to be connected to", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Connect", "C", "Create an online conncetion with the Robot", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Upload", "U", "Upload your RAPID code to the Robot", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run", "R", "Run", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Stop", "S", "Stop/Pause", GH_ParamAccess.item);
            pManager.AddTextParameter("Main Code", "M", "Insert here the MainCode", GH_ParamAccess.item);
            pManager.AddTextParameter("Base Code", "B", "Insert here the BaseCode", GH_ParamAccess.item);
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
            ControllerGoo controllerGoo = null;
            bool connect = false;
            bool upload = false;
            bool run = false;
            bool stop = false;
            string RAPID= null;
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

                // Stop the program
                if (stop)
                {
                    Stop();
                }

                // Upload the code
                if (upload)
                {
                    // First stop the current program
                    Stop();

                    // Get path
                    string path = Path.Combine(Util.LibraryPath(), "temp");

                    // Check if the parh already exists
                    if (Directory.Exists(path))
                    {
                        // Delete if it already exists
                        Directory.Delete(path, true);
                    }

                    // Create new path / folder
                    Directory.CreateDirectory(path);

                    // Save the RAPID code
                    SaveRapid(path, RAPID, BaseCode);

                    // Get file path / directory to save on the controller
                    string localDirectory = Path.Combine(path, "RAPID"); // Physical
                    string str3 = Path.Combine(_controller.FileSystem.RemoteDirectory, "RAPID"); // Virtual
                    //string filePath;

                    string systemPath;
                    string programPath;

                    // Upload to the virtual controller
                    if (!_controller.IsVirtual)
                    {
                        _controller.AuthenticationSystem.DemandGrant(Grant.WriteFtp);
                        _controller.FileSystem.PutDirectory(localDirectory, "RAPID", true);
                        //filePath = Path.Combine(str3, "RAPID_T_ROB1.pgf");

                        systemPath = Path.Combine(str3, "BASE.sys"); // Change to RC_System?
                        programPath = Path.Combine(str3, "MainModule.mod"); // Change to RC_Program?
                    }
                    // Upload to a physical controller
                    else
                    {
                        //filePath = Path.Combine(localDirectory, "RAPID_T_ROB1.pgf");

                        systemPath = Path.Combine(localDirectory, "BASE.sys"); // Change to RC_System?
                        programPath = Path.Combine(localDirectory, "MainModule.mod"); // Change to RC_Program
                    }

                    // The real upload
                    using (Mastership.Request(_controller.Rapid))
                    {
                        // Get current task
                        Task task = _controller.Rapid.GetTasks().First<Task>();

                        // Delete current task
                        //task.DeleteProgram();

                        // Reset current BASE code if no new base code is provided
                        //if (BaseCode == null)
                        //{
                        //    Module Base = task.GetModule("BASE");
                        //    Base.Delete();
                        //}

                        // Grant acces
                        _controller.AuthenticationSystem.DemandGrant(Grant.LoadRapidProgram);

                        // Load the new program from the created file
                        //task.LoadProgramFromFile(filePath, RapidLoadMode.Replace);
                        task.LoadModuleFromFile(systemPath, RapidLoadMode.Replace);
                        task.LoadModuleFromFile(programPath, RapidLoadMode.Replace);

                        // Update action status message
                        _uStatus = "The RAPID code is succesfully uploaded.";
                    }
                }
            }

            // Disconnect
            else
            {
                Disconnect();
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

        // Additional methods
        #region additional methods
        /// <summary>
        /// Method to connect to the controller. 
        /// </summary>
        public void Connect()
        {
            // Log on 
            _controller.Logon(UserInfo.DefaultUser);

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
        public void Disconnect()
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
        public void Run()
        {
            _uStatus = StartCommand();
        }

        /// <summary>
        /// Method to run the program and returns the action message. 
        /// </summary>
        /// <returns> The action message / status. </returns>
        public string StartCommand()
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
            using (Mastership.Request(_controller.Rapid))
            {
                _controller.Rapid.Start(RegainMode.Continue, ExecutionMode.Continuous, ExecutionCycle.Once, StartCheck.CallChain);
            }

            // Return status message
            return ("Program started.");
        }

        /// <summary>
        /// Method to stop the program.
        /// </summary>
        public void Stop()
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
            using (Mastership.Request(_controller.Rapid))
            {
                _controller.Rapid.Stop(StopMode.Instruction);
            }

            // Return status message
            return "Program stopped.";
        }

        /// <summary>
        /// Method to get the library path of the user
        /// </summary>
        public static class Util
        {
            public static string LibraryPath()
            {
                return (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "RobotComponents"));
            }
        }

        /// <summary>
        /// Saves the RAPID code to a local folder on the used device. 
        /// </summary>
        /// <param name="_path"> The directory where to save the RAPID code. </param>
        /// <param name="RAPID"> The RAPID main code. </param>
        /// <param name="BaseCode"> The RAPID base code. </param>
        public void SaveRapid(string _path, string RAPID, string BaseCode)
        {
            // Create the path: sub folder
            string path = Path.Combine(_path, "RAPID");

            // Check if the directory / folder already exists
            if (Directory.Exists(path))
            {
                // Delete if it already exists
                Directory.Delete(path, true);
            }

            // Create new folder
            Directory.CreateDirectory(path);

            // Create an empty xml document for saving the program files
            //string saveProgram = Path.Combine(path, "RAPID_T_ROB1.xml");
            //XmlDocument xdoc = new XmlDocument();

            //if (BaseCode != null)
            //{
            //    xdoc.LoadXml(@"<?xml version='1.0' encoding='ISO-8859-1' ?><Program><Module>MainModule.mod</Module><Module>BASE.sys</Module></Program>");
            //}
            //else
            //{
            //    xdoc.LoadXml(@"<?xml version='1.0' encoding='ISO-8859-1' ?><Program><Module>MainModule.mod</Module></Program>");
            //}

            // Save xml doc and change extension
            //xdoc.Save(saveProgram);
            //File.Move(saveProgram, Path.ChangeExtension(saveProgram, ".pgf"));

            // Save the main code
            if (RAPID != null)
            {
                // Make an empy txt file for the main code
                string savePathRapid = Path.Combine(path, "MainModule.txt");

                // Save the txt file
                StreamWriter fileRapid = new StreamWriter(savePathRapid);

                // Write the main code to the file
                fileRapid.WriteLine(RAPID);

                // Close the file
                fileRapid.Close();

                // Save the file at the right location with the correct extension
                File.Move(savePathRapid, Path.ChangeExtension(savePathRapid, ".mod"));
            }

            // Save the base code
            if (BaseCode != null)
            {
                // Make an empy txt file for the base code
                string savePathBase = Path.Combine(path, "BASE.txt");

                // Save the txt file
                StreamWriter fileBase = new StreamWriter(savePathBase);

                // Write the base code to the file
                fileBase.WriteLine(BaseCode);

                // Close the file
                fileBase.Close();

                // Save the file at the right location with the correct extension
                File.Move(savePathBase, Path.ChangeExtension(savePathBase, ".sys"));
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