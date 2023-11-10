// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Drawing;
// Grasshopper Libs
using Grasshopper.Kernel;

namespace RobotComponents.ABB.Gh
{
    public class RobotComponentsInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get { return "Robot Components"; }
        }

        public override Bitmap Icon
        {
            get { return Properties.Resources.RobotComponents_Icon24; }
        }

        public override string Description
        {
            get { return "A Grasshopper plugin for intuitive robot programming."; }
        }

        public override Guid Id
        {
            get { return new Guid("1520BAB6-6B67-47C6-A006-F3E22F0C6C2E"); }
        }

        public override string AuthorName
        {
            get { return "Robot Components"; }
        }

        public override string AuthorContact
        {
            get { return "robot-components@outlook.com"; }
        }

        public override GH_LibraryLicense License
        {
            get { return GH_LibraryLicense.opensource; }
        }

        public override string Version
        {
            get { return VersionNumbering.CurrentVersion; }
        }
    }
}