// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.Actions;
// ABB Libs
using ControllersNS = ABB.Robotics.Controllers;
using RapidDomainNS = ABB.Robotics.Controllers.RapidDomain;
using IOSystemDomainNS = ABB.Robotics.Controllers.IOSystemDomain;
using ConfigurationDomainNS = ABB.Robotics.Controllers.ConfigurationDomain;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.MotionDomain;

namespace RobotComponents.ABB.Controllers
{
    public class Controller
    {
        #region fields
        private static List<Controller> _controllers = new List<Controller>();

        private ControllersNS.Controller _controller;
        private ControllersNS.UserInfo _userInfo = ControllersNS.UserInfo.DefaultUser;
        private string _userName = ControllersNS.UserInfo.DefaultUser.Name;
        private string _password = ControllersNS.UserInfo.DefaultUser.Password;
        private readonly List<string> _logger = new List<string>();

        private Dictionary<string, RobotJointPosition> _robotJointPositions = new Dictionary<string, RobotJointPosition>();
        private Dictionary<string, double[]> _externalJointPositions = new Dictionary<string, double[]>();
        private Dictionary<string, Plane> _robotToolPlanes = new Dictionary<string, Plane>();
        private Dictionary<string, Plane> _externalAxisPlanes = new Dictionary<string, Plane>();

        private List<MechanicalUnit> _mechanicalUnits = new List<MechanicalUnit>();
        private List<MechanicalUnit> _robots = new List<MechanicalUnit>();
        private List<MechanicalUnit> _externalAxes = new List<MechanicalUnit>();
        private Dictionary<string, List<MechanicalUnit>> _mechanicalUnitsPerTask = new Dictionary<string, List<MechanicalUnit>>();
        private List<RapidDomainNS.Task> _tasks = new List<RapidDomainNS.Task>();
        
        #endregion

        #region constructors
        public Controller()
        {
            _controller = null;
        }

        private Controller(ControllersNS.ControllerInfo controllerInfo)
        {
            _controller = ControllersNS.Controller.Connect(controllerInfo, ControllersNS.ConnectionType.Standalone);

            Initiliaze();
        }
        #endregion

        #region static methods
        public static List<Controller> GetControllers()
        {
            NetworkScanner scanner = new NetworkScanner();
            scanner.Scan();

            _controllers.Clear();
            _controllers = scanner.GetControllers().ToList().ConvertAll(item => new Controller(item));

            return _controllers;
        }

        private static string CurrentTime()
        {
            DateTime localDate = DateTime.Now;
            return localDate.ToString();
        }
        #endregion

        #region methods
        public override string ToString()
        {
            if (_controller == null)
            {
                return "Null Controller";
            }
            else if (_controller.IsVirtual == true)
            {
                return "Virtual controller (" + _controller.Name + ")";
            }
            else
            {
                return "Physical controller (" + _controller.Name + ")";
            }
        }

        private void Initiliaze()
        {
            _mechanicalUnits = _controller.MotionSystem.MechanicalUnits.ToList();
            _tasks = _controller.Rapid.GetTasks().ToList();

            for (int i = 0; i < _tasks.Count; i++)
            {
                _mechanicalUnitsPerTask.Add(_tasks[i].Name, new List<MechanicalUnit>());
            }

            for (int i = 0; i < _mechanicalUnits.Count; i++)
            {             
                if (_mechanicalUnits[i].Type == MechanicalUnitType.TcpRobot)
                {
                    _robots.Add(_mechanicalUnits[i]);
                    _robotJointPositions.Add(_mechanicalUnits[i].Name, new RobotJointPosition());
                    _robotToolPlanes.Add(_mechanicalUnits[i].Name, Plane.Unset);
                    _mechanicalUnitsPerTask[_mechanicalUnits[i].Task.Name].Add(_mechanicalUnits[i]);
                }
                else if (_mechanicalUnits[i].Type == MechanicalUnitType.SingleAxis | _mechanicalUnits[i].Type == MechanicalUnitType.MultiAxes)
                {
                    _externalAxes.Add(_mechanicalUnits[i]);
                    _externalJointPositions.Add(_mechanicalUnits[i].Name, Enumerable.Repeat(9e9, _mechanicalUnits[i].NumberOfAxes).ToArray());
                    _externalAxisPlanes.Add(_mechanicalUnits[i].Name, Plane.Unset);
                    _mechanicalUnitsPerTask[_mechanicalUnits[i].Task.Name].Add(_mechanicalUnits[i]);
                }
            }
        }

        public void ReInitiliaze()
        {
            _mechanicalUnits.Clear();
            _mechanicalUnitsPerTask.Clear();
            _robots.Clear();
            _externalAxes.Clear();
            _robotJointPositions.Clear();
            _externalJointPositions.Clear();
            _robotToolPlanes.Clear();
            _externalAxisPlanes.Clear();
            _tasks.Clear();
            _logger.Clear();

            Initiliaze();

            Log("Controller reinitiliazed.");
        }

        private void Log(string msg)
        {
            _logger.Insert(0, $"{CurrentTime()}: {msg}");
        }

        public void ResetLogger()
        {
            _logger.Clear();
        }

        public bool Logon()
        {
            try
            {
                _controller.Logon(_userInfo);
                Log($"Logon with username {_userName} succeeded.");
                return true;
            }

            catch
            {
                Log($"Logon with username {_userName} failed.");
                return false;
            }
        }

        public bool Logoff()
        {
            try
            {
                _controller.Logoff();
                Log("Logoff succeeded.");
                return true;
            }

            catch
            {
                Log("Logoff failed.");
                return false;
            }
        }

        public bool Dispose()
        {
            try
            {
                if (_controller.Connected == true)
                {
                    _controller.Logoff();
                }

                _controller.Dispose();
                _controller = null;

                return true;
            }

            catch
            {
                Log("Failed to dispose the controller object.");
                return false;
            }
        }

        public Dictionary<string, Plane> GetRobotBaseFrames()
        {
            Dictionary<string, Plane> result = new Dictionary<string, Plane>();

            ConfigurationDomainNS.TypeCollection types = _controller.Configuration.Domains["MOC"].Types;
            ConfigurationDomainNS.Type type = _controller.Configuration.Domains["MOC"].Types[types.IndexOf("ROBOT")];

            /*
            List<ConfigurationDomainNS.Instance> instances = new List<ConfigurationDomainNS.Instance>();
            instances.AddRange(types[types.IndexOf("ROBOT")].GetInstances());
            instances.AddRange(types[types.IndexOf("SINGLE")].GetInstances());

            for (int i = 0; i < instances.Count; i++)
            {
                ConfigurationDomainNS.Instance instance = instances[i];

                string name = (string)instance.GetAttribute("name");

                double x = Convert.ToDouble(instance.GetAttribute("base_frame_pos_x").ToString()) * 1000;
                double y = Convert.ToDouble(instance.GetAttribute("base_frame_pos_y").ToString()) * 1000;
                double z = Convert.ToDouble(instance.GetAttribute("base_frame_pos_z").ToString()) * 1000;

                double a = Convert.ToDouble(instance.GetAttribute("base_frame_orient_u0").ToString());
                double b = Convert.ToDouble(instance.GetAttribute("base_frame_orient_u1").ToString());
                double c = Convert.ToDouble(instance.GetAttribute("base_frame_orient_u2").ToString());
                double d = Convert.ToDouble(instance.GetAttribute("base_frame_orient_u3").ToString());

                Plane plane = RobotComponents.Utils.HelperMethods.QuaternionToPlane(x, y, z, a, b, c, d);

                result.Add(name, plane);
            }
            */

            for (int i = 0; i < _robots.Count; i++)
            {
                try
                {
                    ConfigurationDomainNS.Instance instance = type.GetInstance(_robots[i].Name);

                    string name = (string)instance.GetAttribute("name");

                    double x = Convert.ToDouble(instance.GetAttribute("base_frame_pos_x").ToString()) * 1000;
                    double y = Convert.ToDouble(instance.GetAttribute("base_frame_pos_y").ToString()) * 1000;
                    double z = Convert.ToDouble(instance.GetAttribute("base_frame_pos_z").ToString()) * 1000;

                    double a = Convert.ToDouble(instance.GetAttribute("base_frame_orient_u0").ToString());
                    double b = Convert.ToDouble(instance.GetAttribute("base_frame_orient_u1").ToString());
                    double c = Convert.ToDouble(instance.GetAttribute("base_frame_orient_u2").ToString());
                    double d = Convert.ToDouble(instance.GetAttribute("base_frame_orient_u3").ToString());

                    Plane plane = RobotComponents.Utils.HelperMethods.QuaternionToPlane(x, y, z, a, b, c, d);

                    result.Add(name, plane);
                }
                catch
                {
                    Log($"Could not find robot {_robots[i].Name}.");
                }
            }

            return result;
        }

        public Dictionary<string, Plane> GetRobotToolPlanes(int system)
        {
            CoordinateSystemType coordinateSystem = (CoordinateSystemType)system;

            for (int i = 0; i < _robots.Count; i++)
            {
                RapidDomainNS.RobTarget robTarget = _robots[i].GetPosition(coordinateSystem);
                    
                Plane plane = RobotComponents.Utils.HelperMethods.QuaternionToPlane(
                    robTarget.Trans.X, robTarget.Trans.Y, robTarget.Trans.Z,
                    robTarget.Rot.Q1, robTarget.Rot.Q2, robTarget.Rot.Q3, robTarget.Rot.Q4);

                _robotToolPlanes[_robots[i].Name] = plane;
            }

            return _robotToolPlanes;
        }

        public Dictionary<string, RobotJointPosition> GetRobotJointPositions()
        {
            for (int i = 0; i < _robots.Count; i++)
            {
                RapidDomainNS.JointTarget jointTarget = _robots[i].GetPosition();

                _robotJointPositions[_robots[i].Name][0] = jointTarget.RobAx.Rax_1;
                _robotJointPositions[_robots[i].Name][1] = jointTarget.RobAx.Rax_2;
                _robotJointPositions[_robots[i].Name][2] = jointTarget.RobAx.Rax_3;
                _robotJointPositions[_robots[i].Name][3] = jointTarget.RobAx.Rax_4;
                _robotJointPositions[_robots[i].Name][4] = jointTarget.RobAx.Rax_5;
                _robotJointPositions[_robots[i].Name][5] = jointTarget.RobAx.Rax_6;
            }

            return _robotJointPositions;
        }

        public Dictionary<string, double[]> GetExternalJointPositions()
        {
            for (int i = 0; i < _externalAxes.Count; i++)
            {
                RapidDomainNS.JointTarget jointTarget = _mechanicalUnits[i].GetPosition();

                for (int j = 0; j < _externalAxes[i].NumberOfAxes; j++)
                {
                    switch (j)
                    {
                        case 0: _externalJointPositions[_externalAxes[i].Name][j] = jointTarget.ExtAx.Eax_a < 9e8 ? jointTarget.ExtAx.Eax_a : _externalJointPositions[_externalAxes[i].Name][j]; break;
                        case 1: _externalJointPositions[_externalAxes[i].Name][j] = jointTarget.ExtAx.Eax_b < 9e8 ? jointTarget.ExtAx.Eax_b : _externalJointPositions[_externalAxes[i].Name][j]; break;
                        case 2: _externalJointPositions[_externalAxes[i].Name][j] = jointTarget.ExtAx.Eax_c < 9e8 ? jointTarget.ExtAx.Eax_c : _externalJointPositions[_externalAxes[i].Name][j]; break;
                        case 3: _externalJointPositions[_externalAxes[i].Name][j] = jointTarget.ExtAx.Eax_d < 9e8 ? jointTarget.ExtAx.Eax_d : _externalJointPositions[_externalAxes[i].Name][j]; break;
                        case 4: _externalJointPositions[_externalAxes[i].Name][j] = jointTarget.ExtAx.Eax_e < 9e8 ? jointTarget.ExtAx.Eax_e : _externalJointPositions[_externalAxes[i].Name][j]; break;
                        case 5: _externalJointPositions[_externalAxes[i].Name][j] = jointTarget.ExtAx.Eax_f < 9e8 ? jointTarget.ExtAx.Eax_f : _externalJointPositions[_externalAxes[i].Name][j]; break;

                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            return _externalJointPositions;
        }

        public List<string> GetAnalogOutputNames()
        {
            IOSystemDomainNS.SignalCollection signals = this.GetAnalogOutputs(); 
            return signals.ToList().ConvertAll(signal => signal.Name);
        }

        public List<string> GetDigitalOutputNames()
        {
            IOSystemDomainNS.SignalCollection signals = this.GetDigitalOutputs();
            return signals.ToList().ConvertAll(signal => signal.Name);
        }

        public List<string> GetAnalogInputNames()
        {
            IOSystemDomainNS.SignalCollection signals = this.GetAnalogInputs();
            return signals.ToList().ConvertAll(signal => signal.Name);
        }

        public List<string> GetDigitalInputNames()
        {
            IOSystemDomainNS.SignalCollection signals = this.GetDigitalInputs();
            return signals.ToList().ConvertAll(signal => signal.Name);
        }

        public IOSystemDomainNS.SignalCollection GetAnalogOutputs()
        {
            IOSystemDomainNS.SignalCollection signals = _controller.IOSystem.GetSignals(filter: IOSystemDomainNS.IOFilterTypes.Output | IOSystemDomainNS.IOFilterTypes.Analog);
            IOSystemDomainNS.SignalCollection result = new IOSystemDomainNS.SignalCollection();

            for (int i = 0; i < signals.Count; i++)
            {
                if (_controller.Configuration.Read("EIO", "EIO_SIGNAL", signals[i].Name, "Access") != "ReadOnly")
                {
                    result.Add(signals[i]);
                }
            }

            return result;
        }

        public IOSystemDomainNS.SignalCollection GetDigitalOutputs()
        {
            IOSystemDomainNS.SignalCollection signals = _controller.IOSystem.GetSignals(filter: IOSystemDomainNS.IOFilterTypes.Output | IOSystemDomainNS.IOFilterTypes.Digital);
            IOSystemDomainNS.SignalCollection result = new IOSystemDomainNS.SignalCollection();

            for (int i = 0; i < signals.Count; i++)
            {
                if (_controller.Configuration.Read("EIO", "EIO_SIGNAL", signals[i].Name, "Access") != "ReadOnly")
                {
                    result.Add(signals[i]);
                }
            }

            return result;
        }

        public IOSystemDomainNS.SignalCollection GetAnalogInputs()
        {
            IOSystemDomainNS.SignalCollection signals = _controller.IOSystem.GetSignals(filter: IOSystemDomainNS.IOFilterTypes.Input | IOSystemDomainNS.IOFilterTypes.Analog);
            IOSystemDomainNS.SignalCollection result = new IOSystemDomainNS.SignalCollection();

            for (int i = 0; i < signals.Count; i++)
            {
                if (_controller.Configuration.Read("EIO", "EIO_SIGNAL", signals[i].Name, "Access") != "ReadOnly")
                {
                    result.Add(signals[i]);
                }
            }

            return result;
        }

        public IOSystemDomainNS.SignalCollection GetDigitalInputs()
        {
            IOSystemDomainNS.SignalCollection signals = _controller.IOSystem.GetSignals(filter: IOSystemDomainNS.IOFilterTypes.Input | IOSystemDomainNS.IOFilterTypes.Digital);
            IOSystemDomainNS.SignalCollection result = new IOSystemDomainNS.SignalCollection();

            for (int i = 0; i < signals.Count; i++)
            {
                if (_controller.Configuration.Read("EIO", "EIO_SIGNAL", signals[i].Name, "Access") != "ReadOnly")
                {
                    result.Add(signals[i]);
                }
            }

            return result;
        }

        public bool SetSignal(string name, float value)
        {
            // TODO: Check Acces level
            // TODO: Check if value is in limits

            try
            {
                IOSystemDomainNS.Signal signal = PickSignal(name);
                signal.Value = value;
                return true;
            }
            catch
            {
                Log($"Could not set the value of signal {name}.");
                return false;
            }
        }

        public void SetUserInfo(string name, string password = "")
        {
            _userName = name;
            _password = password;

            _userInfo = new ControllersNS.UserInfo(_userName, _password);
            Log($"Username set to {_userName}");
        }

        public void SetDefaultUser()
        {
            _userInfo = ControllersNS.UserInfo.DefaultUser;

            _userName = _userInfo.Name;
            _password = _userInfo.Password;

            Log("User Info set to DefaultUser.");
        }

        public IOSystemDomainNS.Signal PickSignal(string name)
        {
            return _controller.IOSystem.GetSignal(name);
        }

        public Signal GetSignal(string name)
        {
            return new Signal(_controller.IOSystem.GetSignal(name));
        }

        public bool UploadModule(string taskName, List<string> module, out string status)
        {
            status = "";

            #region pick task
            RapidDomainNS.Task task;

            try
            {
                task = _controller.Rapid.GetTask(taskName);
            }
            catch
            {
                status = "Could not pick the task from the controller: Invalid task name.";
                Log(status);
                return false;
            }
            #endregion

            #region write temporary file
            string tempDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Robot Components", "temp");

            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
                
            Directory.CreateDirectory(tempDirectory);
            
            if (module.Count != 0)
            {
                string programFilePath = Path.Combine(tempDirectory, "temp.mod");
                using (StreamWriter writer = new StreamWriter(programFilePath, false))
                {
                    for (int i = 0; i < module.Count; i++)
                    {
                        writer.WriteLine(module[i]);
                    }
                }
            }
            else
            {
                status = "Upload failed: No module defined.";
                Log(status);
                return false;
            }
            #endregion

            // Directory to save the modules on the controller
            string controllerDirectory = Path.Combine(_controller.FileSystem.RemoteDirectory, "RAPID");

            // Module file paths
            string filePath;
            string directory;

            // Stop the program before upload
            StopProgram(out _);

            // Upload to the real physical controller
            if (_controller.IsVirtual == false)
            {
                _controller.AuthenticationSystem.DemandGrant(ControllersNS.Grant.WriteFtp);
                _controller.FileSystem.PutDirectory(tempDirectory, "RAPID", true);
                directory = controllerDirectory;
            }
            // Upload to a virtual controller
            else
            {
                directory = tempDirectory;
            }

            // The real upload
            using (ControllersNS.Mastership master = ControllersNS.Mastership.Request(_controller))
            {
                // Grant acces
                _controller.AuthenticationSystem.DemandGrant(ControllersNS.Grant.LoadRapidProgram);

                // Load the new program from the created file
                if (module.Count != 0)
                {
                    filePath = Path.Combine(directory, "temp.mod");
                    task.LoadModuleFromFile(filePath, RapidDomainNS.RapidLoadMode.Replace);
                }

                // Resets the program pointer of this task to the main entry point.
                if (_controller.OperatingMode == ControllersNS.ControllerOperatingMode.Auto)
                {
                    _controller.AuthenticationSystem.DemandGrant(ControllersNS.Grant.ExecuteRapid);

                    try
                    {
                        task.ResetProgramPointer(); // Requires auto mode and execute rapid
                        //_programPointerWarning = false;
                    }
                    catch
                    {
                        //_programPointerWarning = true;
                    }
                }

                // Update action status message
                if (module.Count != 0)
                {
                    //_uStatus = "The RAPID code is succesfully uploaded.";
                }
                else
                {
                    //_uStatus = "The RAPID is not uploaded since there is no code defined.";
                }

                // Give back the mastership
                master.Release();
            }

            // Delete the temporary files
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }

            return false; // Returns true on success
        }

        public bool ResetProgramPointers(out string status)
        {
            bool succeeded = false;
            status = "";

            using (ControllersNS.Mastership master = ControllersNS.Mastership.Request(_controller))
            {
                if (_controller.OperatingMode == ControllersNS.ControllerOperatingMode.Auto)
                {
                    _controller.AuthenticationSystem.DemandGrant(ControllersNS.Grant.ExecuteRapid);

                    try
                    {
                        RapidDomainNS.Task[] tasks = _controller.Rapid.GetTasks();

                        for (int i = 0; i < tasks.Length; i++)
                        {
                            tasks[i].ResetProgramPointer();
                        }

                        status = "Reset of all program pointers succeeded.";
                        Log(status);

                        succeeded = true;

                    }
                    catch (Exception e)
                    {
                        status = "Could not reset all program pointers.";
                        Log(status);
                        Log(e.Message);

                        succeeded = false;
                    }
                    finally
                    {
                        master.Release();
                    }
                }

            }

            return succeeded;
        }

        public bool ResetProgramPointer(string taskName, out string status)
        {
            bool succeeded = false;
            status = "";

            using (ControllersNS.Mastership master = ControllersNS.Mastership.Request(_controller))
            {
                if (_controller.OperatingMode == ControllersNS.ControllerOperatingMode.Auto)
                {
                    _controller.AuthenticationSystem.DemandGrant(ControllersNS.Grant.ExecuteRapid);

                    try
                    {
                        RapidDomainNS.Task task = _controller.Rapid.GetTask(taskName);
                        task.ResetProgramPointer();

                        status = "Reset of program pointer succeeded.";
                        Log(status);

                        succeeded = true;

                    }
                    catch (Exception e)
                    {
                        status = "Could not reset the program pointer.";
                        Log(status);
                        Log(e.Message);

                        succeeded = false;
                    }
                    finally
                    {
                        master.Release();
                    }
                }

            }

            return succeeded;
        }

        public bool RunProgram(out string status)
        {
            if (_controller.OperatingMode != ControllersNS.ControllerOperatingMode.Auto)
            {
                status = "Could not start the program. The controller is not set in automatic mode.";
                Log(status);
                return false;
            }

            else if (_controller.State != ControllersNS.ControllerState.MotorsOn)
            {
                status = "Could not start the program. The motors are not on.";
                Log(status);
                return false;
            }

            else
            {
                using (ControllersNS.Mastership master = ControllersNS.Mastership.Request(_controller))
                {
                    _controller.Rapid.Start(RapidDomainNS.RegainMode.Continue, RapidDomainNS.ExecutionMode.Continuous, RapidDomainNS.ExecutionCycle.Once, RapidDomainNS.StartCheck.CallChain);
                    master.Release();
                }

                status = "Program started.";
                Log(status);
                
                return true;
            }
        }

        public bool StopProgram(out string status)
        {
            if (_controller.OperatingMode != ControllersNS.ControllerOperatingMode.Auto)
            {
                status = "Could not stop the program. The controller is not set in automatic mode.";
                Log(status);
                return false;
            }
            else
            {
                using (ControllersNS.Mastership master = ControllersNS.Mastership.Request(_controller))
                {
                    _controller.Rapid.Stop(RapidDomainNS.StopMode.Instruction);
                    master.Release();
                }

                status = "Program stopped.";
                Log(status);
                return true;
            }
        }

        public string ReadConfigurationDatabase(string domain, string type, string instance, string attribute)
        {
            string[] path = new string[4]{ domain, type, instance, attribute };
            return _controller.Configuration.Read(path);
        }

        private bool WriteConfigurationDataBase(string domain, string type, string instance, string attribute, string value)
        {
            // TODO

            return false; // Returns true on success
        }
        #endregion

        #region static properties
        public static List<Controller> Controllers
        {
            get { return _controllers; }
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (_controller == null) { return false; }
                return true;
            }
        }

        public ControllersNS.Controller ControllerInstanceABB
        {
            get { return _controller; }
        }

        public List<RapidDomainNS.Task> Tasks
        {
            get { return _tasks; }
        }

        public string Name
        {
            get { return _controller.Name; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public List<string> Logger
        {
            get { return _logger; }
        }
        #endregion
    }
}