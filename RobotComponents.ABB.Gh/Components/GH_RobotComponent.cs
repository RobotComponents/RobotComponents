// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components
{
    /// <summary>
    /// Component abstract class.
    /// </summary>
    public abstract class GH_RobotComponent : GH_Component
    {
        #region constructors
        /// <summary>
        /// Empty constructor. 
        /// </summary>
        protected GH_RobotComponent() : base()
        {
        }

        /// <summary>
        /// Constructs a generic Robot Components component. 
        /// </summary>
        /// <param name="name"> Name of the component. </param>
        /// <param name="nickname"> Nickname of the component. </param>
        /// <param name="category"> Category in which this component belongs. </param>
        /// <param name="description"> Description of the component. </param>
        protected GH_RobotComponent(string name, string nickname, string category, string description) : 
            base(name, nickname, description + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion, "Robot Components ABB", category)
        {
        }
        #endregion

        #region menu item
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Documentation", MenuItemClickComponentDoc, Properties.Resources.WikiPage_MenuItem_Icon);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Documentation" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        internal void MenuItemClickComponentDoc(object sender, EventArgs e)
        {
            string url = Documentation.ComponentWeblinks[this.GetType()];
            Documentation.OpenBrowser(url);
        }
        #endregion
    }
}

