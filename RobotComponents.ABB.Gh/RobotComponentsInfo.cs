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
    /// <summary>
    /// Contains metadata for the Robot Components Grasshopper plugin, 
    /// including name, description, icon, version, and author information.
    /// Used by Grasshopper to display plugin details.
    /// </summary>
    public class RobotComponentsInfo : GH_AssemblyInfo
    {
        /// <summary>
        /// The display name of the plugin.
        /// </summary>
        public override string Name
        {
            get { return "Robot Components"; }
        }

        /// <summary>
        /// The icon shown in the Grasshopper component ribbon.
        /// </summary>
        public override Bitmap Icon
        {
            get { return Properties.Resources.RobotComponents_Icon24; }
        }

        /// <summary>
        /// A short description of the plugin's purpose.
        /// </summary>
        public override string Description
        {
            get { return "A Grasshopper plugin for intuitive robot programming."; }
        }

        /// <summary>
        /// The unique identifier of the plugin assembly.
        /// </summary>
        public override Guid Id
        {
            get { return new Guid("1520BAB6-6B67-47C6-A006-F3E22F0C6C2E"); }
        }

        /// <summary>
        /// The name of the plugin author or organization.
        /// </summary>
        public override string AuthorName
        {
            get { return "Robot Components"; }
        }

        /// <summary>
        /// Contact information for support or inquiries.
        /// </summary>
        public override string AuthorContact
        {
            get { return "info@arjendeetman.nl"; }
        }

        /// <summary>
        /// Indicates the license type under which the plugin is distributed.
        /// </summary>
        public override GH_LibraryLicense License
        {
            get { return GH_LibraryLicense.opensource; }
        }

        /// <summary>
        /// The current version of the plugin.
        /// </summary>
        public override string Version
        {
            get { return VersionNumbering.CurrentVersion; }
        }
    }
}