// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Rhino Libs
using Rhino.Geometry;
// ABB Libs
using IOSystemDomainNS = ABB.Robotics.Controllers.IOSystemDomain;

namespace RobotComponents.ABB.Controllers
{
    public class Signal
    {
        #region fields
        private readonly IOSystemDomainNS.Signal _signal;
        private readonly Interval _limits;
        #endregion

        #region constructors
        public Signal()
        { 
        }

        public Signal(IOSystemDomainNS.Signal signal)
        {
            _signal = signal;
            _limits = new Interval(_signal.MinValue, _signal.MinValue);
        }
        #endregion

        #region methods
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
        #endregion

        #region properties
        public bool IsValid
        {
            get 
            { 
                if (_signal == null) { return false; }
                if (_signal.Type == IOSystemDomainNS.SignalType.Unknown) { return false; }
                return true; 
            }
        }

        public IOSystemDomainNS.Signal SignalInstanceABB
        {
            get { return _signal; }
        }

        public string Name
        {
            get { return _signal.Name; }
        }

        public float Value
        {
            get { return _signal.Value; }
        }

        public float MinValue
        {
            get { return _signal.MinValue; }
        }

        public float MaxValue
        {
            get { return _signal.MaxValue; }
        }

        public Interval Limits
        {
            get { return _limits; }
        }
        #endregion
    }
}
