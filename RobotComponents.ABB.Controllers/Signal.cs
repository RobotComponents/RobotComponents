// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Rhino Libs
using Rhino.Geometry;
// ABB Libs
using ControllersNS = ABB.Robotics.Controllers;
using IOSystemDomainNS = ABB.Robotics.Controllers.IOSystemDomain;

namespace RobotComponents.ABB.Controllers
{
    /// <summary>
    /// Represents the Signal class. 
    /// </summary>
    /// <remarks>
    /// This class is a wrapper around the ABB Signal class. 
    /// </remarks>
    public class Signal
    {
        #region fields
        private IOSystemDomainNS.Signal _signal;
        private Interval _limits = new Interval();
        private string _name = "";
        private bool _isEmpty = true;
        private string _accesLevel = "";
        #endregion

        #region constructors
        /// <summary>
        /// Empty constructor. 
        /// </summary>
        public Signal()
        {
            _isEmpty = true;
        }

        /// <summary>
        /// Construct a Signal instance from an ABB Signal instance. 
        /// </summary>
        /// <param name="signal"> The ABB Signal instance. </param>
        /// <param name="controller"> The ABB Controller instance. </param>
        public Signal(IOSystemDomainNS.Signal signal, ControllersNS.Controller controller)
        {
            _signal = signal;
            _limits = new Interval(_signal.MinValue, _signal.MaxValue);
            _name = _signal.Name;
            _isEmpty = false;

            this.SetAccesLevels(controller);
        }

        /// <summary>
        /// Construct a Signal instance from an ABB Signal instance. 
        /// </summary>
        /// <param name="signal"> The ABB Signal instance. </param>
        /// <param name="controller"> The Robot Components Controller instance. </param>
        public Signal(IOSystemDomainNS.Signal signal, Controller controller)
        {
            _signal = signal;
            _limits = new Interval(_signal.MinValue, _signal.MaxValue);
            _name = _signal.Name;
            _isEmpty = false;

            this.SetAccesLevels(controller.ControllerABB);
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> 
        /// A string that represents the current object. 
        /// </returns>
        public override string ToString()
        {
            if (_isEmpty == true)
            {
                return "Empty signal";
            }
            if (_signal.Type == IOSystemDomainNS.SignalType.AnalogInput)
            {
                return $"Analog Input ({_signal.Name}/{_signal.Value})";
            }
            else if (_signal.Type == IOSystemDomainNS.SignalType.AnalogOutput)
            {
                return $"Analog Output ({_signal.Name}/{_signal.Value})";
            }
            else if (_signal.Type == IOSystemDomainNS.SignalType.DigitalInput)
            {
                return $"Digital Input ({_signal.Name}/{_signal.Value})";
            }
            else if (_signal.Type == IOSystemDomainNS.SignalType.DigitalOutput)
            {
                return $"Digital Output ({_signal.Name}/{_signal.Value})";
            }
            else if (_signal.Type == IOSystemDomainNS.SignalType.GroupInput)
            {
                return $"Group Input ({_signal.Name}/{_signal.Value})";
            }
            else if (_signal.Type == IOSystemDomainNS.SignalType.GroupOutput)
            {
                return $"Group Output ({_signal.Name}/{_signal.Value})";
            }
            else
            {
                return "Unknown signal";
            }
        }

        /// <summary>
        /// Sets the acces level of the signal.
        /// </summary>
        /// <param name="controller"> The ABB controller instance. </param>
        private void SetAccesLevels(ControllersNS.Controller controller)
        {
            // Try: some acces levels are restricted. 
            try
            {
                _accesLevel = controller.Configuration.Read("EIO", "EIO_SIGNAL", this.Name, "Access");
            }
            catch
            {
                _accesLevel = "-";
            }
        }

        /// <summary>
        /// Overwrites the current value of the signal. 
        /// </summary>
        /// <param name="value"> The desired signal state. </param>
        /// <param name="msg"> The status message. </param>
        /// <returns> 
        /// True on success, false on failure. 
        /// </returns>
        public bool SetValue(float value, out string msg)
        {
            msg = "";

            if (_isEmpty == true)
            {
                msg = $"Could not set the value of the signal. No signal defined.";
            }

            if (_limits.IncludesParameter(value, false) == false)
            {
                msg = $"Desired value of signal {_signal.Name} is not within limits.";
                return false;
            }

            try
            {
                _signal.Value = value;
                return true;
            }
            catch
            {
                msg = $"Could not set the value of signal {_signal.Name}. Does the signal has the correct acces level?.";
                return false;
            }
        }


        /// <summary>
        /// Disposes the current signal object inside this instance.
        /// </summary>
        /// <returns> 
        /// True on success, false on failure. 
        /// </returns>
        public bool Dispose()
        {
            if (_isEmpty == true)
            {
                return false;
            }

            try
            {
                _signal.Dispose();
                _limits = new Interval();
                _name = "";
                _accesLevel = "";
                _isEmpty = true;

                return true;
            }

            catch
            {
                return false;
            }
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
                if (_signal == null) { return false; }
                if (_signal.Type == IOSystemDomainNS.SignalType.Unknown) { return false; }
                return true; 
            }
        }

        /// <summary>
        /// Gets the signal instance.
        /// </summary>
        public IOSystemDomainNS.Signal SignalABB
        {
            get { return _signal; }
        }

        /// <summary>
        /// Gets the signal name. 
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the minimum and maximum values of the signal as an interval.
        /// </summary>
        public Interval Limits
        {
            get { return _limits; }
        }

        /// <summary>
        /// Gets the current value of the signal. 
        /// </summary>
        public float Value
        {
            get 
            { 
                if (!_isEmpty) { return _signal.Value; }
                else { return 0; }
            }
        }

        /// <summary>
        /// Gets the minimum value of the signal.
        /// </summary>
        public double MinValue
        {
            get { return _limits.Min; }
        }

        /// <summary>
        /// Gets the maximum value of the signal. 
        /// </summary>
        public double MaxValue
        {
            get { return _limits.Max; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the signal instance is empty.
        /// <remarks>
        /// If empty, there is no ABB signal instance defined inside this instance. 
        /// </remarks>
        /// </summary>
        public bool IsEmpty
        {
            get { return _isEmpty; }
        }

        /// <summary>
        /// Gets the acces level.
        /// </summary>
        public string AccesLevel
        {
            get { return _accesLevel; }
        }
        #endregion
    }
}
