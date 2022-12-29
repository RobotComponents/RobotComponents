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
using RobotComponents.ABB.Utils;
using RobotComponents.ABB.Actions.Declarations;
// ABB Libs
using ControllersNS = ABB.Robotics.Controllers;
using RapidDomainNS = ABB.Robotics.Controllers.RapidDomain;
using IOSystemDomainNS = ABB.Robotics.Controllers.IOSystemDomain;
using ConfigurationDomainNS = ABB.Robotics.Controllers.ConfigurationDomain;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.MotionDomain;

namespace RobotComponents.ABB.Controllers
{
    /// <summary>
    /// Represents the Controller class. 
    /// This class is a wrapper around the ABB Controller class. 
    /// </summary>
    public class Controller
    {
        #region fields
        private static List<Controller> _controllers = new List<Controller>();

        private ControllersNS.Controller _controller;
        private ControllersNS.UserInfo _userInfo = ControllersNS.UserInfo.DefaultUser;
        private string _userName = ControllersNS.UserInfo.DefaultUser.Name;
        private string _password = ControllersNS.UserInfo.DefaultUser.Password;
        private readonly List<string> _logger = new List<string>();

        private List<RapidDomainNS.Task> _tasks = new List<RapidDomainNS.Task>();

        private readonly Dictionary<string, RobotJointPosition> _robotJointPositions = new Dictionary<string, RobotJointPosition>();
        private readonly Dictionary<string, double[]> _externalJointPositions = new Dictionary<string, double[]>();
        private readonly Dictionary<string, Plane> _robotToolPlanes = new Dictionary<string, Plane>();
        private readonly Dictionary<string, Plane> _externalAxisPlanes = new Dictionary<string, Plane>();

        private readonly Dictionary<string, JointTarget> _jointTargets = new Dictionary<string, JointTarget>();
        private readonly Dictionary<string, RobotTarget> _robotTargets = new Dictionary<string, RobotTarget>();

        private List<MechanicalUnit> _mechanicalUnits = new List<MechanicalUnit>();
        private readonly List<MechanicalUnit> _robots = new List<MechanicalUnit>();
        private readonly List<MechanicalUnit> _externalAxes = new List<MechanicalUnit>();

        private readonly Dictionary<string, List<MechanicalUnit>> _mechanicalUnitsPerTask = new Dictionary<string, List<MechanicalUnit>>();
        private readonly Dictionary<string, List<MechanicalUnit>> _robotsPerTask = new Dictionary<string, List<MechanicalUnit>>();
        private readonly Dictionary<string, List<MechanicalUnit>> _externalAxesPerTask = new Dictionary<string, List<MechanicalUnit>>();

        private readonly List<Signal> _analogInputs = new List<Signal>();
        private readonly List<Signal> _analogOutputs = new List<Signal>();
        private readonly List<Signal> _digitalInputs = new List<Signal>();
        private readonly List<Signal> _digitalOutputs = new List<Signal>();

        private bool _isEmtpy = true;
        #endregion

        #region constructors
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Controller()
        {
            _controller = null;
            _isEmtpy = true;
        }

        /// <summary>
        /// Constructs a Controller instance from an ABB Controller Info instance. 
        /// </summary>
        /// <param name="controllerInfo"> The ABB Controller Info instance. </param>
        private Controller(ControllersNS.ControllerInfo controllerInfo)
        {
            _controller = ControllersNS.Controller.Connect(controllerInfo, ControllersNS.ConnectionType.Standalone);
            _isEmtpy = false;

            Initiliaze();
        }
        #endregion

        #region static methods
        /// <summary>
        /// Returns a list with ABB Controllers that are found in the network. 
        /// </summary>
        /// <returns> A list with controlers. </returns>
        public static List<Controller> GetControllers()
        {
            NetworkScanner scanner = new NetworkScanner();
            scanner.Scan();

            _controllers.Clear();
            _controllers = scanner.GetControllers().ToList().ConvertAll(item => new Controller(item));

            return _controllers;
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (_controller == null)
            {
                return "Empty Controller";
            }
            else if (_controller.IsVirtual == true)
            {
                return "Virtual controller (" + _controller.SystemName + ")";
            }
            else
            {
                return "Physical controller (" + _controller.SystemName + ")";
            }
        }

        /// <summary>
        /// Initialize the fiels of this Constroller instance. 
        /// </summary>
        private void Initiliaze()
        {
            if (_isEmtpy == true)
            {
                Log("Could not initialize the controller class. There is no ABB controller defined.");
            }

            #region mechanical units
            _mechanicalUnits = _controller.MotionSystem.MechanicalUnits.ToList();
            _tasks = _controller.Rapid.GetTasks().ToList();

            for (int i = 0; i < _tasks.Count; i++)
            {
                _mechanicalUnitsPerTask.Add(_tasks[i].Name, new List<MechanicalUnit>());
                _robotsPerTask.Add(_tasks[i].Name, new List<MechanicalUnit>());
                _externalAxesPerTask.Add(_tasks[i].Name, new List<MechanicalUnit>());

                _jointTargets.Add(_tasks[i].Name, new JointTarget());
                _robotTargets.Add(_tasks[i].Name, new RobotTarget());
            }

            for (int i = 0; i < _mechanicalUnits.Count; i++)
            {
                if (_mechanicalUnits[i].Type == MechanicalUnitType.TcpRobot)
                {
                    _robots.Add(_mechanicalUnits[i]);
                    _robotJointPositions.Add(_mechanicalUnits[i].Name, new RobotJointPosition());
                    _robotToolPlanes.Add(_mechanicalUnits[i].Name, Plane.Unset);
                    _mechanicalUnitsPerTask[_mechanicalUnits[i].Task.Name].Add(_mechanicalUnits[i]);
                    _robotsPerTask[_mechanicalUnits[i].Task.Name].Add(_mechanicalUnits[i]);
                }
                else if (_mechanicalUnits[i].Type == MechanicalUnitType.SingleAxis | _mechanicalUnits[i].Type == MechanicalUnitType.MultiAxes)
                {
                    _externalAxes.Add(_mechanicalUnits[i]);
                    _externalJointPositions.Add(_mechanicalUnits[i].Name, Enumerable.Repeat(9e9, _mechanicalUnits[i].NumberOfAxes).ToArray());
                    _externalAxisPlanes.Add(_mechanicalUnits[i].Name, Plane.Unset);
                    _mechanicalUnitsPerTask[_mechanicalUnits[i].Task.Name].Add(_mechanicalUnits[i]);
                    _externalAxesPerTask[_mechanicalUnits[i].Task.Name].Add(_mechanicalUnits[i]);
                }
            }
            #endregion

            #region signals
            _analogInputs.AddRange(GetAnalogInputs());
            _analogOutputs.AddRange(GetAnalogOutputs());
            _digitalInputs.AddRange(GetDigitalInputs());
            _digitalOutputs.AddRange(GetDigitalOutputs());
            #endregion

            SetDefaultUser();
        }

        /// <summary>
        /// Reintiliazes this controller instance. 
        /// </summary>
        public void ReInitiliaze()
        {
            _logger.Clear();

            _tasks.Clear();

            _robotJointPositions.Clear();
            _externalJointPositions.Clear();
            _robotToolPlanes.Clear();
            _externalAxisPlanes.Clear();

            _jointTargets.Clear();
            _robotTargets.Clear();

            _mechanicalUnits.Clear();
            _robots.Clear();
            _externalAxes.Clear();
            
            _mechanicalUnitsPerTask.Clear();
            _robotsPerTask.Clear();
            _externalAxesPerTask.Clear();

            _analogInputs.Clear();
            _analogOutputs.Clear();
            _digitalInputs.Clear();
            _digitalOutputs.Clear();

            Initiliaze();

            Log("Controller reinitiliazed.");
        }

        /// <summary>
        /// Writes a message to the logger. 
        /// </summary>
        /// <param name="msg"> The message to write to the logger. </param>
        private void Log(string msg)
        {
            _logger.Insert(0, $"{DateTime.Now}: {msg}");
        }

        /// <summary>
        /// Resets the logger.
        /// </summary>
        public void ResetLogger()
        {
            _logger.Clear();
        }

        /// <summary>
        /// Logon to the set user. 
        /// </summary>
        /// <returns> True on success, false on failure. </returns>
        public bool Logon()
        {
            if (_isEmtpy == true)
            {
                Log("Logon failed. The controller is empty.");
                return false;
            }

            try
            {
                _controller.Logon(_userInfo);
                Log($"Logon with username {_userName} succeeded.");
                return true;
            }

            catch (Exception e)
            {
                Log($"Logon with username {_userName} failed.");
                Log($"{e.Message}.");
                return false;
            }
        }

        /// <summary>
        /// Logs off the current user.
        /// </summary>
        /// <returns> True on success, false on failure. </returns>
        public bool Logoff()
        {
            if (_isEmtpy == true)
            {
                Log("Logoff failed. The controller is empty.");
                return false;
            }

            try
            {
                _controller.Logoff();
                Log("Logoff succeeded.");
                return true;
            }

            catch (Exception e)
            {
                Log("Logoff failed.");
                Log($"{e.Message}.");
                return false;
            }
        }

        /// <summary>
        /// Disposes the current controller object inside this instance.
        /// </summary>
        /// <returns> True on success, false on failure. </returns>
        public bool Dispose()
        {
            if (_isEmtpy == true)
            {
                Log($"Failed to dispose the controller object. The controller is empty.");
                return false;
            }

            try
            {
                if (_controller.Connected == true)
                {
                    _controller.Logoff();
                }

                _controller.Dispose();
                _controller = null;
                _isEmtpy = true;

                return true;
            }

            catch (Exception e)
            {
                Log($"Failed to dispose the controller object.");
                Log($"{e.Message}.");
                return false;
            }
        }

        /// <summary>
        /// Returns the robot base frames.
        /// </summary>
        /// <returns> A dictionary with as key the name of the robot and as value the base frame. </returns>
        public Dictionary<string, Plane> GetRobotBaseFrames()
        {
            Dictionary<string, Plane> result = new Dictionary<string, Plane>();

            if (_isEmtpy == true)
            {
                Log($"Could not get the robot base frames. The controller is empty.");
                return result;
            }

            ConfigurationDomainNS.TypeCollection types = _controller.Configuration.Domains["MOC"].Types;
            ConfigurationDomainNS.Type type = _controller.Configuration.Domains["MOC"].Types[types.IndexOf("ROBOT")];

            for (int i = 0; i < _robots.Count; i++)
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

                Plane plane = HelperMethods.QuaternionToPlane(x, y, z, a, b, c, d);

                result.Add(name, plane);
            }

            return result;
        }

        /// <summary>
        /// Returns the current robot tool planes.
        /// </summary>
        /// <returns> A dictionary with as key the name of the robot and as value the current tool planes. </returns>
        public Dictionary<string, Plane> GetRobotToolPlanes(int system)
        {
            if (_isEmtpy == true)
            {
                Log($"Could not get the robot tool planes. The controller is empty.");
                return _robotToolPlanes;
            }

            CoordinateSystemType coordinateSystem = (CoordinateSystemType)system;

            for (int i = 0; i < _robots.Count; i++)
            {
                RapidDomainNS.RobTarget robTarget = _robots[i].GetPosition(coordinateSystem);
                   
                _robotToolPlanes[_robots[i].Name] = HelperMethods.QuaternionToPlane(
                    robTarget.Trans.X,
                    robTarget.Trans.Y,
                    robTarget.Trans.Z,
                    robTarget.Rot.Q1,
                    robTarget.Rot.Q2,
                    robTarget.Rot.Q3,
                    robTarget.Rot.Q4); 
            }

            return _robotToolPlanes;
        }

        /// <summary>
        /// Returns the current external axis planes.
        /// </summary>
        /// <returns> A dictionary with as key the name of the external axis and as value the current plane. </returns>
        public Dictionary<string, Plane> GetExternalAxisPlanes(int system)
        {
            if (_isEmtpy == true)
            {
                Log($"Could not get the external axis planes. The controller is empty.");
                return _externalAxisPlanes;
            }

            CoordinateSystemType coordinateSystem = (CoordinateSystemType)system;

            for (int i = 0; i < _externalAxes.Count; i++)
            {
                RapidDomainNS.RobTarget robTarget = _externalAxes[i].GetPosition(coordinateSystem);

                Plane plane = HelperMethods.QuaternionToPlane(
                    robTarget.Trans.X,
                    robTarget.Trans.Y,
                    robTarget.Trans.Z,
                    robTarget.Rot.Q1,
                    robTarget.Rot.Q2,
                    robTarget.Rot.Q3,
                    robTarget.Rot.Q4);

                _externalAxisPlanes[_externalAxes[i].Name] = plane;
            }

            return _externalAxisPlanes;
        }

        /// <summary>
        /// Returns the current robot joint positions.
        /// </summary>
        /// <returns> A dictionary with as key the name of the robot and as value the current robot joint position. </returns>
        public Dictionary<string, RobotJointPosition> GetRobotJointPositions()
        {
            if (_isEmtpy == true)
            {
                Log($"Could not get the robot joint positions. The controller is empty.");
                return _robotJointPositions;
            }

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

        /// <summary>
        /// Returns the current external joint positions.
        /// </summary>
        /// <returns> A dictionary with as key the name of the external axis and as value the current external joint position. </returns>
        public Dictionary<string, double[]> GetExternalJointPositions()
        {
            if (_isEmtpy == true)
            {
                Log($"Could not get the external joint positions. The controller is empty.");
                return _externalJointPositions;
            }

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

        /// <summary>
        /// Returns the current joint targets. 
        /// </summary>
        /// <returns> A dictionary with as key the name of the task and as value the current joint target.</returns>
        private Dictionary<string, JointTarget> GetJointTargets()
        {
            //TODO: This metod is private since it needs to be tested first. 

            if (_isEmtpy == true)
            {
                Log($"Could not get the joint targets. The controller is empty.");
                return _jointTargets;
            }

            for (int i = 0; i < _tasks.Count; i++)
            {
                RapidDomainNS.JointTarget jointTarget = _tasks[i].GetJointTarget();

                _jointTargets[_tasks[i].Name].RobotJointPosition[0] = jointTarget.RobAx.Rax_1;
                _jointTargets[_tasks[i].Name].RobotJointPosition[1] = jointTarget.RobAx.Rax_2;
                _jointTargets[_tasks[i].Name].RobotJointPosition[2] = jointTarget.RobAx.Rax_3;
                _jointTargets[_tasks[i].Name].RobotJointPosition[3] = jointTarget.RobAx.Rax_4;
                _jointTargets[_tasks[i].Name].RobotJointPosition[4] = jointTarget.RobAx.Rax_5;
                _jointTargets[_tasks[i].Name].RobotJointPosition[5] = jointTarget.RobAx.Rax_6;

                _jointTargets[_tasks[i].Name].ExternalJointPosition[0] = jointTarget.ExtAx.Eax_a < 9e8 ? jointTarget.ExtAx.Eax_a : _jointTargets[_tasks[i].Name].ExternalJointPosition[0];
                _jointTargets[_tasks[i].Name].ExternalJointPosition[1] = jointTarget.ExtAx.Eax_b < 9e8 ? jointTarget.ExtAx.Eax_b : _jointTargets[_tasks[i].Name].ExternalJointPosition[1];
                _jointTargets[_tasks[i].Name].ExternalJointPosition[2] = jointTarget.ExtAx.Eax_c < 9e8 ? jointTarget.ExtAx.Eax_c : _jointTargets[_tasks[i].Name].ExternalJointPosition[2];
                _jointTargets[_tasks[i].Name].ExternalJointPosition[3] = jointTarget.ExtAx.Eax_d < 9e8 ? jointTarget.ExtAx.Eax_d : _jointTargets[_tasks[i].Name].ExternalJointPosition[3];
                _jointTargets[_tasks[i].Name].ExternalJointPosition[4] = jointTarget.ExtAx.Eax_e < 9e8 ? jointTarget.ExtAx.Eax_e : _jointTargets[_tasks[i].Name].ExternalJointPosition[4];
                _jointTargets[_tasks[i].Name].ExternalJointPosition[5] = jointTarget.ExtAx.Eax_f < 9e8 ? jointTarget.ExtAx.Eax_f : _jointTargets[_tasks[i].Name].ExternalJointPosition[5];
            }

            return _jointTargets;
        }

        /// <summary>
        /// Returns the current robot targets. 
        /// </summary>
        /// <returns> A dictionary with as key the name of the task and as value the current robot target.</returns>
        private Dictionary<string, RobotTarget> GetRobotTargets()
        {
            //TODO: This metod is private since it needs to be tested first. 

            if (_isEmtpy == true)
            {
                Log($"Could not get the robot targets. The controller is empty.");
                return _robotTargets;
            }

            for (int i = 0; i < _tasks.Count; i++)
            {
                // if (_tasks[i].Enabled == true) ? 
                // Check _tasks[i].TaskType ? 

                RapidDomainNS.RobTarget robotTarget = _tasks[i].GetRobTarget();

                _robotTargets[_tasks[i].Name].Plane = HelperMethods.QuaternionToPlane(
                    robotTarget.Trans.X,
                    robotTarget.Trans.Y,
                    robotTarget.Trans.Z,
                    robotTarget.Rot.Q1,
                    robotTarget.Rot.Q2,
                    robotTarget.Rot.Q3,
                    robotTarget.Rot.Q4);

                _robotTargets[_tasks[i].Name].AxisConfig = robotTarget.Robconf.Cfx;

                _robotTargets[_tasks[i].Name].ExternalJointPosition[0] = robotTarget.Extax.Eax_a < 9e8 ? robotTarget.Extax.Eax_a : _robotTargets[_tasks[i].Name].ExternalJointPosition[0];
                _robotTargets[_tasks[i].Name].ExternalJointPosition[1] = robotTarget.Extax.Eax_b < 9e8 ? robotTarget.Extax.Eax_b : _robotTargets[_tasks[i].Name].ExternalJointPosition[1];
                _robotTargets[_tasks[i].Name].ExternalJointPosition[2] = robotTarget.Extax.Eax_c < 9e8 ? robotTarget.Extax.Eax_c : _robotTargets[_tasks[i].Name].ExternalJointPosition[2];
                _robotTargets[_tasks[i].Name].ExternalJointPosition[3] = robotTarget.Extax.Eax_d < 9e8 ? robotTarget.Extax.Eax_d : _robotTargets[_tasks[i].Name].ExternalJointPosition[3];
                _robotTargets[_tasks[i].Name].ExternalJointPosition[4] = robotTarget.Extax.Eax_e < 9e8 ? robotTarget.Extax.Eax_e : _robotTargets[_tasks[i].Name].ExternalJointPosition[4];
                _robotTargets[_tasks[i].Name].ExternalJointPosition[5] = robotTarget.Extax.Eax_f < 9e8 ? robotTarget.Extax.Eax_f : _robotTargets[_tasks[i].Name].ExternalJointPosition[5];
            }

            return _robotTargets;
        }

        /// <summary>
        /// Returns the analog output signals. 
        /// </summary>
        /// <returns> A list with analog output signals. </returns>
        private List<Signal> GetAnalogOutputs()
        {
            if (_isEmtpy == true)
            {
                Log($"Could not get the analog outputs. The controller is empty.");
                return new List<Signal>();
            }

            IOSystemDomainNS.SignalCollection signals = _controller.IOSystem.GetSignals(filter: IOSystemDomainNS.IOFilterTypes.Output | IOSystemDomainNS.IOFilterTypes.Analog);
            List<Signal> result = new List<Signal>();

            for (int i = 0; i < signals.Count; i++)
            {
                result.Add(new Signal(signals[i], _controller));
            }

            return result;
        }

        /// <summary>
        /// Returns the digital output signals. 
        /// </summary>
        /// <returns> A list with digital output signals. </returns>
        private List<Signal> GetDigitalOutputs()
        {
            if (_isEmtpy == true)
            {
                Log($"Could not get the digital outputs. The controller is empty.");
                return new List<Signal>();
            }
            
            IOSystemDomainNS.SignalCollection signals = _controller.IOSystem.GetSignals(filter: IOSystemDomainNS.IOFilterTypes.Output | IOSystemDomainNS.IOFilterTypes.Digital);
            List<Signal> result = new List<Signal>();

            for (int i = 0; i < signals.Count; i++)
            {
                result.Add(new Signal(signals[i], _controller));
            }

            return result;
        }

        /// <summary>
        /// Returns the analog inputs. 
        /// </summary>
        /// <returns> A list with analog inputs. </returns>
        private List<Signal> GetAnalogInputs()
        {
            if (_isEmtpy == true)
            {
                Log($"Could not get the analog inputs. The controller is empty.");
                return new List<Signal>();
            }

            IOSystemDomainNS.SignalCollection signals = _controller.IOSystem.GetSignals(filter: IOSystemDomainNS.IOFilterTypes.Input | IOSystemDomainNS.IOFilterTypes.Analog);
            List<Signal> result = new List<Signal>();

            for (int i = 0; i < signals.Count; i++)
            {
                result.Add(new Signal(signals[i], _controller));
            }

            return result;
        }

        /// <summary>
        /// Returns the digital inputs.
        /// </summary>
        /// <returns> A list with digital inputs. </returns>
        private List<Signal> GetDigitalInputs()
        {
            if (_isEmtpy == true)
            {
                Log($"Could not get the digital inputs. The controller is empty.");
                return new List<Signal>();
            }

            IOSystemDomainNS.SignalCollection signals = _controller.IOSystem.GetSignals(filter: IOSystemDomainNS.IOFilterTypes.Input | IOSystemDomainNS.IOFilterTypes.Digital);
            List<Signal> result = new List<Signal>();

            for (int i = 0; i < signals.Count; i++)
            {
                result.Add(new Signal(signals[i], _controller));
            }

            return result;
        }

        /// <summary>
        /// Sets the uner name and password. 
        /// </summary>
        /// <param name="name"> The user name. </param>
        /// <param name="password"> The password. </param>
        public void SetUserInfo(string name, string password = "")
        {
            if (_isEmtpy == true)
            {
                Log($"Could not set the user {name}. The controller is empty.");
            }

            _userName = name;
            _password = password;

            _userInfo = new ControllersNS.UserInfo(_userName, _password);
            Log($"Username set to {_userName}");
        }

        /// <summary>
        /// Sets the default user info. 
        /// </summary>
        public void SetDefaultUser()
        {
            if (_isEmtpy == true)
            {
                Log("Could not set the default user. The controller is empty.");
            }

            _userName = ControllersNS.UserInfo.DefaultUser.Name;
            _password = ControllersNS.UserInfo.DefaultUser.Password;

            Log("User Info set to DefaultUser.");
        }

        /// <summary>
        /// Returns the analog input signal from the controller. 
        /// </summary>
        /// <param name="name"> The name of the signal. </param>
        /// <param name="index"> The index number of the signal. The index is -1 if no signal was found. </param>
        /// <returns> The analog input signal. Returns an empty signal if no signal was found. </returns>
        public Signal GetAnalogInput(string name, out int index)
        {
            if (_isEmtpy == true)
            {
                index = -1;
                Log($"Could not get the signal {name}. The controller is empty.");
                return new Signal();
            }

            index = _analogInputs.FindIndex(item => item.Name == name);

            if (index == -1)
            {
                Log($"Could not get the signal {name}. Signal not found.");
                return new Signal();
            }
            else
            {
                return _analogInputs[index];
            }
        }

        /// <summary>
        /// Returns the analog output signal from the controller. 
        /// </summary>
        /// <param name="name"> The name of the signal. </param>
        /// <param name="index"> The index number of the signal. The index is -1 if no signal was found. </param>
        /// <returns> The analog output signal. Returns an empty signal if no signal was found. </returns>
        public Signal GetAnalogOutput(string name, out int index)
        {
            if (_isEmtpy == true)
            {
                index = -1;
                Log($"Could not get the signal {name}. The controller is empty.");
                return new Signal();
            }

            index = _analogOutputs.FindIndex(item => item.Name == name);

            if (index == -1)
            {
                Log($"Could not get the signal {name}. Signal not found.");
                return new Signal();
            }
            else
            {
                return _analogOutputs[index];
            }
        }

        /// <summary>
        /// Returns the digital input signal from the controller. 
        /// </summary>
        /// <param name="name"> The name of the signal. </param>
        /// <param name="index"> The index number of the signal. The index is -1 if no signal was found. </param>
        /// <returns> The digital input signal. Returns an empty signal if no signal was found. </returns>
        public Signal GetDigitalInput(string name, out int index)
        {
            if (_isEmtpy == true)
            {
                index = -1;
                Log($"Could not get the signal {name}. The controller is empty.");
                return new Signal();
            }

            index = _digitalInputs.FindIndex(item => item.Name == name);

            if (index == -1)
            {
                Log($"Could not get the signal {name}. Signal not found.");
                return new Signal();
            }
            else
            {
                return _digitalInputs[index];
            }
        }

        /// <summary>
        /// Returns the digital output signal from the controller. 
        /// </summary>
        /// <param name="name"> The name of the signal. </param>
        /// <param name="index"> The index number of the signal. The index is -1 if no signal was found. </param>
        /// <returns> The digital output signal. Returns an empty signal if no signal was found. </returns>
        public Signal GetDigitalOutput(string name, out int index)
        {
            if (_isEmtpy == true)
            {
                index = -1;
                Log($"Could not get the signal {name}. The controller is empty.");
                return new Signal();
            }

            index = _digitalOutputs.FindIndex(item => item.Name == name);

            if (index == -1)
            {
                Log($"Could not get the signal {name}. Signal not found.");
                return new Signal();
            }
            else
            {
                return _digitalOutputs[index];
            }
        }

        /// <summary>
        /// Uploads a module to the controller. 
        /// </summary>
        /// <param name="taskName"> The task to upload to. </param>
        /// <param name="module"> The module to upload. </param>
        /// <param name="status"> The status message. </param>
        /// <returns> True on success, false on failure. </returns>
        public bool UploadModule(string taskName, List<string> module, out string status)
        {
            status = "";

            if (_isEmtpy == true)
            {
                status = "Could not upload the module. The controller is empty.";
                Log(status);
                return false;
            }

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
            StopProgram(out status);

            // Settings for a upload to a physical controller
            if (_controller.IsVirtual == false)
            {
                _controller.AuthenticationSystem.DemandGrant(ControllersNS.Grant.WriteFtp);
                _controller.FileSystem.PutDirectory(tempDirectory, "RAPID", true);
                directory = controllerDirectory;
            }
            // Settings for a upload to a virtual controller
            else
            {
                directory = tempDirectory;
            }

            // The real upload
            try
            {
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

                    // Give back the mastership
                    master.Release();
                }
            }
            catch (Exception e)
            {
                status = $"Could not upload the module. {e.Message}.";
                Log(status);
                return false;
            }
            finally
            {
                // Delete the temporary files
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
            }

            status = "Upload succeeded.";
            Log(status);

            return true;
        }

        /// <summary>
        /// Sets the Programing pointer at specific routine in specified task. 
        /// </summary>
        /// <param name="task"> The name of the task, </param>
        /// <param name="routine"> The name of the routine. </param>
        /// <returns> True on success, false on failure. </returns>
        private bool SetProgramPointer(string task, string routine)
        {
            //TODO: Therefore it is a private. 

            return false;
        }

        /// <summary>
        /// Resets all the program pointers to the main routine. 
        /// </summary>
        /// <param name="status"> The status message, </param>
        /// <returns> True on success, false on failure. </returns>
        public bool ResetProgramPointers(out string status)
        {
            bool succeeded = false;
            status = "";

            if (_isEmtpy == true)
            {
                status = "Could not reset the program pointers. The controller is empty.";
                Log(status);
                return succeeded;
            }

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

                        status = "Reset of the program pointers succeeded.";
                        Log(status);

                        succeeded = true;

                    }
                    catch (Exception e)
                    {
                        status = "Could not reset the program pointers.";
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

        /// <summary>
        /// Resets the program pointer of a given task. 
        /// </summary>
        /// <param name="taskName"> The task name. </param>
        /// <param name="status">The status message. </param>
        /// <returns> True on success, false on failure. </returns>
        public bool ResetProgramPointer(string taskName, out string status)
        {
            bool succeeded = false;
            status = "";

            if (_isEmtpy == true)
            {
                status = "Could not reset the program pointer. The controller is empty.";
                Log(status);
                return succeeded;
            }

            using (ControllersNS.Mastership master = ControllersNS.Mastership.Request(_controller))
            {
                if (_controller.OperatingMode == ControllersNS.ControllerOperatingMode.Auto)
                {
                    _controller.AuthenticationSystem.DemandGrant(ControllersNS.Grant.ExecuteRapid);

                    try
                    {
                        RapidDomainNS.Task task = _controller.Rapid.GetTask(taskName);
                        task.ResetProgramPointer();

                        status = "Reset of the program pointer succeeded.";
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

        /// <summary>
        /// Makes a call to run the program. 
        /// </summary>
        /// <param name="status"> The status message. </param>
        /// <returns> True on success, false on failure. </returns>
        public bool RunProgram(out string status)
        {
            if (_isEmtpy == true)
            {
                status = "Could not start the program. The controller is empty.";
                Log(status);
                return false;
            }

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

        /// <summary>
        /// Makes a call to stop the program. 
        /// </summary>
        /// <param name="status"> The status message. </param>
        /// <returns> True on success, false on failure. </returns>
        public bool StopProgram(out string status)
        {
            if (_isEmtpy == true)
            {
                status = "Could not stop the program. The controller is empty.";
                Log(status);
                return false;
            }

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

        /// <summary>
        /// Returns the value from the configuration database for a given path. 
        /// </summary>
        /// <param name="domain"> The domain name. </param>
        /// <param name="type"> The type name. </param>
        /// <param name="instance"> The instance name. </param>
        /// <param name="attribute">TThe attribute name. </param>
        /// <returns> A value from the configuration database. </returns>
        public string ReadConfigurationDomain(string domain, string type, string instance, string attribute)
        {
            if (_isEmtpy == true)
            {
                return "";
            }

            string[] path = new string[4] { domain, type, instance, attribute };
            return _controller.Configuration.Read(path);
        }

        /// <summary>
        /// Returns the rapid value of a specified path.
        /// </summary>
        /// <param name="task"> The task name. </param>
        /// <param name="module"> The module name. </param>
        /// <param name="variable"> The variable name. </param>
        /// <returns> The rapid value. </returns>
        private string ReadRapidDomain(string task, string module, string variable)
        {
            if (_isEmtpy == true)
            {
                return "";
            }

            return _controller.Rapid.GetRapidData(task, module, variable).StringValue;   
        }
        #endregion

        #region static properties
        /// <summary>
        /// Gets the controllers found in the network. 
        /// Call the static method GetControllers to scan the network. 
        /// </summary>
        public static List<Controller> Controllers
        {
            get { return _controllers; }
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
                if (_controller == null) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets the controller instance. 
        /// </summary>
        public ControllersNS.Controller ControllerABB
        {
            get { return _controller; }
        }

        /// <summary>
        /// Gets the list with controller tasks. 
        /// </summary>
        public List<RapidDomainNS.Task> TasksABB
        {
            get { return _tasks; }
        }

        /// <summary>
        /// Gets the task names. 
        /// </summary>
        public List<string> TaskNames
        {
            get { return _tasks.ConvertAll(item => item.Name); }
        }

        /// <summary>
        /// Gets the name of the controller. 
        /// </summary>
        public string Name
        {
            get { return _controller.Name; }
        }

        /// <summary>
        /// Gets the set username. 
        /// </summary>
        public string UserName
        {
            get { return _userName; }
        }

        /// <summary>
        /// Gets the logger. 
        /// </summary>
        public List<string> Logger
        {
            get { return _logger; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the controller instance is empty.
        /// If empty, there is no ABB controller instance defined inside this instance. 
        /// </summary>
        public bool IsEmpty
        {
            get { return _isEmtpy; }
        }

        /// <summary>
        /// Gets the analog inputs. 
        /// </summary>
        public List<Signal> AnalogInputs
        {
            get { return _analogInputs; }
        }

        /// <summary>
        /// Gets the analog outputs. 
        /// </summary>
        public List<Signal> AnalogOutputs
        {
            get { return _analogOutputs; }
        }

        /// <summary>
        /// Gets the digital inputs. 
        /// </summary>
        public List<Signal> DigitalInputs
        {
            get { return _digitalInputs; }
        }

        /// <summary>
        /// Gets the analog outputs. 
        /// </summary>
        public List<Signal> DigitalOutputs
        {
            get { return _digitalOutputs; }
        }
        #endregion

        #region events
        private bool SubscribeToEvents()
        {
            if (_isEmtpy == true)
            {
                Log("Could not subscribe the events. The controller is empty.");
                return false;
            }

            try
            {
                // Controller domain
                _controller.ConnectionChanged += OnConnectionChangedEvent;
                _controller.MastershipChanged += OnMastershipChangedEvent;
                _controller.OperatingModeChanged += OnOperatingModeChangeEvent;
                _controller.StateChanged += OnStateChangedEventArgs;

                // Event log domain
                // Not implemented

                // Signal domain
                // Not implemented

                // Motion domain
                // Not implemented

                // Rapid domain
                // Not implemented

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void OnBackupEventArgs(object sender, ControllersNS.BackupEventArgs e)
        {
            // Not implemented
        }

        private void OnConnectionChangedEvent(object sender, ControllersNS.ConnectionChangedEventArgs e)
        {
            // Not implemented
        }

        private void OnControllerEvent(object sender, ControllersNS.ControllerEventArgs e)
        {
            // Not implemented
        }

        private void OnMastershipChangedEvent(object sender, ControllersNS.MastershipChangedEventArgs e)
        {
            // Not implemented
        }

        private void OnOperatingModeChangeEvent(object sender, ControllersNS.OperatingModeChangeEventArgs e)
        {
            // Not implemented
        }

        private void OnStateChangedEventArgs(object sender, ControllersNS.StateChangedEventArgs e)
        {
            // Not implemented
        }
        #endregion
    }
}