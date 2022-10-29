// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.IO;
using System.Collections.Generic;
// ABB Libs
using ControllersNS = ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using ABB.Robotics.Controllers.IOSystemDomain;

namespace RobotComponents.ABB.Controllers
{
    public class Controller
    {
        #region fields
        private ControllersNS.Controller _controller;
        private ControllersNS.UserInfo _userInfo = ControllersNS.UserInfo.DefaultUser;
        private string _userName = ControllersNS.UserInfo.DefaultUser.Name;
        private string _password = ControllersNS.UserInfo.DefaultUser.Password;
        private readonly List<string> _logger = new List<string>();
        #endregion

        #region constructors
        public Controller()
        {
            _controller = null;
        }

        public Controller(ControllersNS.ControllerInfo controllerInfo)
        {
            _controller = ControllersNS.Controller.Connect(controllerInfo, ControllersNS.ConnectionType.Standalone);
        }
        #endregion

        #region static methods
        public static ControllersNS.ControllerInfo[] GetControllers()
        {
            NetworkScanner scanner = new NetworkScanner();
            scanner.Scan();

            ControllersNS.ControllerInfo[] controllers = scanner.GetControllers();

            return controllers;
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

        public ControllersNS.Controller GetController()
        {
            return _controller;
        }

        public bool LogOn()
        {
            try
            {
                _controller.Logon(_userInfo);
                _logger.Add(string.Format("{0}: Log on with username {1} succeeded.", CurrentTime(), _userName));
                return true;
            }

            catch
            {
                _logger.Add(string.Format("{0}: Log on with username {1} failed.", CurrentTime(), _userName));
                return false;
            }
        }

        public bool LogOff()
        {
            try
            {
                _controller.Logoff();
                _logger.Add(string.Format("{0}: Log off succeeded.", CurrentTime()));
                return true;
            }

            catch
            {
                _logger.Add(string.Format("{0}: Log off failed.", CurrentTime()));
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
                _logger.Add(string.Format("{0}: Failed to dispose the controller object.", CurrentTime()));
                return false;
            }
        }

        public void SetUserInfo(string name, string password = "")
        {
            _userName = name;
            _password = password;

            _userInfo = new ControllersNS.UserInfo(_userName, _password);

            _logger.Add(string.Format("{0}: Username set to {1}", CurrentTime(), _userName));
        }

        public void SetDefaultUser()
        {
            _userInfo = ControllersNS.UserInfo.DefaultUser;

            _userName = _userInfo.Name;
            _password = _userInfo.Password;

            _logger.Add(string.Format("{0}: User Info set to DefaultUser.", CurrentTime()));
        }

        public SignalCollection GetAnalogOutputs()
        {
            return _controller.IOSystem.GetSignals(filter: IOFilterTypes.Output | IOFilterTypes.Analog);
        }

        public SignalCollection GetDigitalOutputs()
        {
            return _controller.IOSystem.GetSignals(filter: IOFilterTypes.Output | IOFilterTypes.Digital);
        }

        public SignalCollection GetAnalogInputs()
        {
            return _controller.IOSystem.GetSignals(filter: IOFilterTypes.Input | IOFilterTypes.Analog);
        }

        public SignalCollection GetDigitalInputs()
        {
            return _controller.IOSystem.GetSignals(filter: IOFilterTypes.Input | IOFilterTypes.Digital);
        }

        public Signal PickSignal(string name)
        {
            return _controller.IOSystem.GetSignal(name);
        }

        public bool UploadModules(List<string> modules)
        {
            return false; // Returns true on success
        }

        public bool UploadModule(string module)
        {
            StopProgram();

            string userDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "RobotComponents", "temp");

            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            // TODO

            return false; // Returns true on success
        }
        public bool ResetProgramPointer()
        {
            // TODO

            return false; // Returns true on success
        }

        public bool RunProgram()
        {
            if (_controller.OperatingMode != ControllersNS.ControllerOperatingMode.Auto)
            {
                _logger.Add(string.Format("{0}: Could not start the program. The controller is not set in automatic mode.", CurrentTime()));
                return false;
            }

            else if (_controller.State != ControllersNS.ControllerState.MotorsOn)
            {
                _logger.Add(string.Format("{0}: Could not start the program. The motors are not on.", CurrentTime()));
                return false;
            }

            else
            {
                using (ControllersNS.Mastership master = ControllersNS.Mastership.Request(_controller))
                {
                    _controller.Rapid.Start(RegainMode.Continue, ExecutionMode.Continuous, ExecutionCycle.Once, StartCheck.CallChain);
                    master.Release();
                }

                _logger.Add(string.Format("{0}: Program started.", CurrentTime()));
                return true;
            }
        }

        public bool StopProgram()
        {
            if (_controller.OperatingMode != ControllersNS.ControllerOperatingMode.Auto)
            {
                _logger.Add(string.Format("{0}: Could not stop the program. The controller is not set in automatic mode.", CurrentTime()));
                return false;
            }

            else
            {
                using (ControllersNS.Mastership master = ControllersNS.Mastership.Request(_controller))
                {
                    _controller.Rapid.Stop(StopMode.Instruction);
                    master.Release();
                }

                _logger.Add(string.Format("{0}: Program stopped.", CurrentTime()));
                return false;
            }
        }

        public void ResetLogger()
        {
            _logger.Clear();
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

        public string Name
        {
            get { return _controller.Name; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public ControllersNS.UserInfo UserInfo
        {
            get { return _userInfo; }
        }

        public List<string> Logger
        {
            get { return _logger; }
        }
        #endregion
    }
}