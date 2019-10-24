using System;

using Grasshopper.Kernel;
using System.IO;
using System.Linq;
using System.Xml;

using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;
using RobotComponents.Goos;

namespace RobotComponents.Components
{
    public class RemoteConnection : GH_Component
    {
        Controller controller;
        bool ctr = true;
        string msg;
        string cStatus = "Not connected.";
        string uStatus = "No actions.";
        int count = 0;

        /// <summary>
        /// Initializes a new instance of the RemoteConnection class.
        /// </summary>
        public RemoteConnection()
          : base("Remote Connection", "Remote Connection",
              "Establishes a remote connection with the controller to upload an run RAPID code directly on a virtual or real ABB robot controller.",
              "RobotComponents", "Controller Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
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
            pManager.AddTextParameter("Status", "S", "Updates about what is going on here!", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Getting the Controller information
            ControllerGoo controllerGoo = null;
            if (!DA.GetData(0, ref controllerGoo)) { return; }
            controller = controllerGoo.Value;

            bool connect = false;
            bool upload = false;
            bool run = false;
            bool stop = false;
            string RAPID= null;
            string BaseCode= null;

            if (!DA.GetData(1, ref connect)) { return; }
            if (!DA.GetData(2, ref upload)) { return; }
            if (!DA.GetData(3, ref run)) { return; }
            if (!DA.GetData(4, ref stop)) { return; }
            if (!DA.GetData(5, ref RAPID)) { return; }
            if (!DA.GetData(6, ref BaseCode)) { return; }
            base.DestroyIconCache();

            if (connect)
            {
                Connect();

                if (run)
                {
                    Run();
                }

                if (stop)
                {
                    Stop();
                }

                if (upload)
                {
                    Stop();

                    string path = Path.Combine(Util.LibraryPath(), "temp");

                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    Directory.CreateDirectory(path);

                    SaveRapid(path, RAPID, BaseCode);

                    string localDirectory = Path.Combine(path, "RAPID");
                    string str3 = Path.Combine(controller.FileSystem.RemoteDirectory, "RAPID");
                    string filePath = string.Empty;

                    if (!controller.IsVirtual)
                    {
                        controller.AuthenticationSystem.DemandGrant(Grant.WriteFtp);
                        controller.FileSystem.PutDirectory(localDirectory, "RAPID", true);
                        filePath = Path.Combine(str3, "RAPID_T_ROB1.pgf");
                    }
                    else
                    {
                        filePath = Path.Combine(localDirectory, "RAPID_T_ROB1.pgf");
                    }

                    using (Mastership.Request(controller.Rapid))
                    {
                        Task task = controller.Rapid.GetTasks().First<Task>();
                        task.DeleteProgram();
                        if (BaseCode == null)
                        {
                            Module Base = task.GetModule("BASE");
                            Base.Delete();
                        }
                        controller.AuthenticationSystem.DemandGrant(Grant.LoadRapidProgram);
                        task.LoadProgramFromFile(filePath, RapidLoadMode.Replace);
                        this.uStatus = "The RAPID code is succesfully uploaded.";
                    }
                }
            }

            else
            {
                Disconnect();
                if (run || stop || upload)
                {
                    uStatus = "Please connect first.";
                }
            }

            msg = $"The remote connection status:\n\nController: {cStatus}\nActions: {uStatus}";

            DA.SetData(0, msg);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;

                if (ctr)
                {
                    return Properties.Resources.Remote_ON_Icon;
                }
                else
                {
                    return Properties.Resources.Remote_OFF_Icon;
                }
            }
        }

        //  ----- Additional Functions -----
        #region Additional Functions
        public void Connect()
        {
            controller.Logon(UserInfo.DefaultUser);
            this.cStatus = "You are connected.";
            ctr = true;
            if(count == 0)
            {
                uStatus = "All set to go!";
                count = 1;
            }
        }

        public void Disconnect()
        {
            if (controller != null)
            {
                controller.Logoff();
                //controller.Dispose();
                controller = null;
                this.cStatus = "You are disconnected.";
                
                ctr = false;

                if (count == 1)
                {
                    uStatus = "Try to reconnect first.";
                    count = 0;
                }
            }
        }

        public void Run()
        {
            this.uStatus = StartCommand();
        }

        public string StartCommand()
        {
            if (controller.OperatingMode != ControllerOperatingMode.Auto)
            {
                return ("Controller not set in automatic.");
            }
            if (controller.State != ControllerState.MotorsOn)
            {
                return ("Motors not on.");
            }
            using (Mastership.Request(controller.Rapid))
            {
                controller.Rapid.Start(RegainMode.Continue, ExecutionMode.Continuous, ExecutionCycle.Once, StartCheck.CallChain);
            }
            return ("Program started.");
        }

        public void Stop()
        {
            this.uStatus = StopCommand();
        }

        private string StopCommand()
        {
            if (controller.OperatingMode != ControllerOperatingMode.Auto)
            {
                return "Controller not set in automatic mode.";
            }
            using (Mastership.Request(controller.Rapid))
            {
                controller.Rapid.Stop(StopMode.Instruction);
                
            }
            return "Program stopped.";
        }

        public static class Util
        {
            public static string LibraryPath()
            {
                return (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "RobotComponents"));
            }
        }

        public void SaveRapid(string _path, string RAPID, string BaseCode)
        {
            string path = Path.Combine(_path, "RAPID");

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);

            //Saving the program File
            string saveProgram = Path.Combine(path, "RAPID_T_ROB1.xml"); // To do: Update for later use, multi move has multiple robots
            XmlDocument xdoc = new XmlDocument();

            if (BaseCode != null)
            {
                xdoc.LoadXml(@"<?xml version='1.0' encoding='ISO-8859-1' ?><Program><Module>MainModule.mod</Module><Module>BASE.sys</Module></Program>");
            }
            else
            {
                xdoc.LoadXml(@"<?xml version='1.0' encoding='ISO-8859-1' ?><Program><Module>MainModule.mod</Module></Program>");
            }
            xdoc.Save(saveProgram);
            File.Move(saveProgram, Path.ChangeExtension(saveProgram, ".pgf"));

            //saving the RAPID code
            string savePathRapid = Path.Combine(path, "MainModule.txt");
            StreamWriter fileRapid = new StreamWriter(savePathRapid);
            fileRapid.WriteLine(RAPID);
            fileRapid.Close();
            File.Move(savePathRapid, Path.ChangeExtension(savePathRapid, ".mod"));
            if (BaseCode != null)
            {
                //saving the BASE code
                string savePathBase = Path.Combine(path, "BASE.txt");
                StreamWriter fileBase = new StreamWriter(savePathBase);
                fileBase.WriteLine(BaseCode);
                fileBase.Close();
                File.Move(savePathBase, Path.ChangeExtension(savePathBase, ".sys"));
            }
        }

        #endregion
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8bfb75d4-9122-45a3-9f11-8d01fb7ea069"); }
        }
    }
}