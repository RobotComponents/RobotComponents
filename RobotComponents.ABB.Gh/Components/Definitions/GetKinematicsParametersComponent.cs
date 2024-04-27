﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.Definitions
{
    /// <summary>
    /// RobotComponents Get Robot Kinematics Parameters component. An inherent from the GH_Component Class.
    /// </summary>
    public class GetKinematicsParametersComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetRobotKinematicsParametersComponent class.
        /// </summary>
        public GetKinematicsParametersComponent()
          : base("Get Kinematics Parameters", "GetKiParams",
              "Gets the kinematics parameters from a robot."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Definitions")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Robot(), "Robot", "R", "Robot as Robot", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("A1", "A1", "The shoulder offset.", GH_ParamAccess.item);
            pManager.AddNumberParameter("A2", "A2", "The elbow offset.", GH_ParamAccess.item);
            pManager.AddNumberParameter("A3", "A3", "The wrist offset.", GH_ParamAccess.item);
            pManager.AddNumberParameter("B", "B", "The lateral offset.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C1", "C1", "The first link length.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C2", "C2", "The second link length.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C3", "C3", "The third link length.", GH_ParamAccess.item);
            pManager.AddNumberParameter("C4", "C4", "The fourth link length", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            Robot robot = null;

            // Catch the input data
            if (!DA.GetData(0, ref robot)) { return; }

            if (robot != null)
            {
                // Check if the input is valid
                if (!robot.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Robot is not valid");
                }

                // Output
                DA.SetData(0, robot.A1);
                DA.SetData(1, robot.A2);
                DA.SetData(2, robot.A3);
                DA.SetData(3, robot.B);
                DA.SetData(4, robot.C1);
                DA.SetData(5, robot.C2);
                DA.SetData(6, robot.C3);
                DA.SetData(7, robot.C4);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary; }
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
            get { return Properties.Resources.GetKinematicsParameters_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("76B45727-CA77-45CB-800B-417C60B67A3F"); }
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