// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Utilities
{
    /// <summary>
    /// RobotComponents Info component. An inherent from the GH_Component Class.
    /// </summary>
    public class InfoComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public InfoComponent()
          : base("Info", "I",
              "Robot Components is a Plugin for intuitive Robot Programming for ABB robots inside Rhinoceros Grasshopper. " 
                + "This open source project is initiated by the chair of Experimental and Digital Design and Construction "
                + "of the University of Kassel led by Prof. Eversmann. The technical development is initiated and executed "
                + "by the research associates and student assistants."
                + Environment.NewLine 
                + Environment.NewLine 
                + "----------"
                + Environment.NewLine
                + Environment.NewLine
                + "Copyright (c) 2018-2021 The Robot Components authors and / or their affiliations"
                + Environment.NewLine
                + Environment.NewLine
                + "Robot Components is free software; you can redistribute it and/or modify it under "
                + "the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation."
                + Environment.NewLine
                + Environment.NewLine
                + "Robot Components is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; " 
                + "without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE."
                + "See the GNU General Public License for more details."
                + Environment.NewLine
                + Environment.NewLine
                + "You should have received a copy of the GNU General Public License along with Robot Components; "
                + "If not, see http://www.gnu.org/licenses/."
                + Environment.NewLine
                + Environment.NewLine
                + "----------"
                + Environment.NewLine
                + Environment.NewLine
                + "More information can be found here: " 
                + Environment.NewLine 
                + "https://github.com/RobotComponents/RobotComponents/"
                + System.Environment.NewLine 
                + System.Environment.NewLine
                + "----------"
                + Environment.NewLine
                + Environment.NewLine
                + "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.septenary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // This component has no input parameters
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // This component has no output parameters
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
        
        }

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
        private void MenuItemClickComponentDoc(object sender, EventArgs e)
        {
            string url = Documentation.ComponentWeblinks[this.GetType()];
            Documentation.OpenBrowser(url);
        }
        #endregion

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.Info_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4FEC796B-E6F3-4996-84FD-FB6E85FDA16B"); }
        }
    }

}
