// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

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
        public static readonly Guid PluginId = new Guid("{60765982-3CAC-4DBC-B55E-E9E68E70381F}");
        public PluginInfo()
            : base(PluginId,
                  new Nomen("Robot Components ABB", "Intuitive robot programming."),
                  typeof(PluginInfo).Assembly.GetName().Version)
        {
            _icon = null;
        }

        private readonly IIcon _icon;

        public override string Author => "Robot Components";

        public override IIcon Icon => _icon;

        public override string LicenseDescription => "GPL v3";

        public override string LicenseAgreement => "GPL v3";
    }
}