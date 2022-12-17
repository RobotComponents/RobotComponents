// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// Rhino Libs
using Rhino.Geometry;
// ABB Libs
using IOSystemDomainNS = ABB.Robotics.Controllers.IOSystemDomain;

namespace RobotComponents.ABB.Controllers
{
    /// <summary>
    /// Represents the Signal class. 
    /// This class is a wrapper around the ABB Signal class. 
    /// </summary>
    public class Signal
    {
        #region fields
        private readonly IOSystemDomainNS.Signal _signal;
        private readonly Interval _limits = new Interval();
        private readonly string _name = "";
        #endregion

        #region constructors
        /// <summary>
        /// Empty constructor. 
        /// </summary>
        public Signal()
        { 
        }

        /// <summary>
        /// Construct a Signal instance from an ABB Signal instance. 
        /// </summary>
        /// <param name="signal"> The ABB Signal instance. </param>
        public Signal(IOSystemDomainNS.Signal signal)
        {
            _signal = signal;
            _limits = new Interval(_signal.MinValue, _signal.MinValue);
            _name = _signal.Name;
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
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
        /// Overwrites the current value of the signal. 
        /// </summary>
        /// <param name="value"> The desired signal state. </param>
        /// <param name="msg"> The status message. </param>
        /// <returns> True on success, false on failure. </returns>
        public bool SetValue(float value, out string msg)
        {
            // TODO: Check Acces level

            msg = "";

            if (_limits.IncludesParameter(value) == false)
            {
                msg = $"Desired value of signal {_signal.Name} is not within  limits.";
                return false;
            }

            try
            {
                _signal.Value = value;
                return true;
            }
            catch
            {
                msg = $"Could not set the value of signal {_signal.Name}.";
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
            get { return _signal.Value; }
        }

        /// <summary>
        /// Gets the minimum value of the signal.
        /// </summary>
        public float MinValue
        {
            get { return _signal.MinValue; }
        }

        /// <summary>
        /// Gets the maximum value of the signal. 
        /// </summary>
        public float MaxValue
        {
            get { return _signal.MaxValue; }
        }
        #endregion
    }
}
