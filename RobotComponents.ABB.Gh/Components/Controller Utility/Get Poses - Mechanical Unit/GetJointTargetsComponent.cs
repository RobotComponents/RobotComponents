﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// Robot Components Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Controllers;
using RobotComponents.ABB.Gh.Parameters.Controllers;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.ControllerUtility
{
    /// <summary>
    /// Represents the component that gets the joint targets from a defined controller. An inherent from the GH_Component Class.
    /// </summary>
    public class GetJointTargetsComponent : GH_Component
    {
        #region fields
        private Controller _controller;
        private Dictionary<string, JointTarget> _jointTargets;
        #endregion

        /// <summary>
        /// Initializes a new instance of the GetExternalJointPositionComponent class.
        /// </summary>
        public GetJointTargetsComponent()
          : base("Get Joint Targets", "GetJT",
              "Gets the current joint targets from an ABB controller."
               + System.Environment.NewLine + System.Environment.NewLine +
                "This component uses the ABB PC SDK." +
                System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Controller Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Controller(), "Controller", "C", "Controller as Controller", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the task", GH_ParamAccess.list);
            pManager.AddParameter(new Param_JointTarget(), "Joint Target", "JT", "Extracted Joint Target", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Check the operating system
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is only supported on Windows operating systems.");
                return;
            }

            // Catch input data
            if (!DA.GetData(0, ref _controller)) { return; }

            try
            {
                _jointTargets = _controller.GetJointTargets();
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            }

            // Output
            DA.SetDataList(0, _jointTargets.Keys);
            DA.SetDataList(1, _jointTargets.Values);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.GetJointTarget_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2D0E3A95-6927-4EE8-8E01-B7574903290A"); }
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
        private void MenuItemClickComponentDoc(object sender, EventArgs e)
        {
            string url = Documentation.ComponentWeblinks[this.GetType()];
            Documentation.OpenBrowser(url);
        }
        #endregion
    }
}