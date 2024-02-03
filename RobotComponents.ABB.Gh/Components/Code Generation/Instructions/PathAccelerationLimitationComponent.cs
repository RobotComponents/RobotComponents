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
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Gh.Parameters.Actions.Instructions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Path Acceleration Limitation component. An inherent from the GH_Component Class.
    /// </summary>
    public class PathAccelerationLimitationComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public PathAccelerationLimitationComponent()
          : base("Path Acceleration Limitation", "PAL",
              "Defines an instruction used to set or reset limitations on TCP acceleration and/or TCP deceleration along the movement path."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Code Generation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Acceleration Limitation", "AL", "Specifies whether or not the acceleration is limited as a Boolean.", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Acceleration Max", "AM", "Defines the absolute value of the acceleration limitation in m/s^2 as a Number.", GH_ParamAccess.item, 0.0);
            pManager.AddBooleanParameter("Deceleration Limitation", "DL", "Specifies whether or not the deceleration is limited as a Boolean.", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Deceleration Max", "DM", "Defines the absolute value of the deceleration limitation in m/s^2 as a Number", GH_ParamAccess.item, 0.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_PathAccelerationLimitation(), "Path Acceleration Limitation", "PAL", "Resulting Path Acceleration Limitation instruction");   
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            bool accelerationLimitation = false;
            double accelerationMax = 0;
            bool decelerationLimitation = false;
            double decelerationMax = 0;

            // Catch the input data
            if (!DA.GetData(0, ref accelerationLimitation)) { return; }
            if (!DA.GetData(1, ref accelerationMax)) { return; }
            if (!DA.GetData(2, ref decelerationLimitation)) { return; }
            if (!DA.GetData(3, ref decelerationMax)) { return; }

            // Create the action
            PathAccelerationLimitation pathAccelerationLimitation = new PathAccelerationLimitation(accelerationLimitation, accelerationMax, decelerationLimitation, decelerationMax);

            // Output
            DA.SetData(0, pathAccelerationLimitation);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary | GH_Exposure.obscure; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return null; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B36062B1-3985-4AF1-972A-4BFF1AA2561F"); }
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
