// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Framework;
using Grasshopper.UI;
using Grasshopper.UI.Icon;

namespace RobotComponents.ABB.Gh2.GhInterop
{
    public class PluginInfo : Plugin
    {
        public static readonly Guid PluginId = new Guid ("{60765982-3CAC-4DBC-B55E-E9E68E70381F}");
        public PluginInfo()
            : base(PluginId, 
                  new Nomen("Robot Components", "Intuitive robot programming."), 
                  typeof(PluginInfo).Assembly.GetName().Version)
        {
            _icon = null;
        }

        private IIcon _icon;

        public override string Author => "Robot Components";

        public override IIcon Icon => _icon;

        public override string LicenseDescription => "GPL v3";

        public override string LicenseAgreement => "GPL v3";
    }
}