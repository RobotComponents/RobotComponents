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
using RobotComponents.Actions;
using RobotComponentsABB.Parameters.Actions;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Robot Joint Position component. An inherent from the GH_Component Class.
    /// </summary>
    public class RobotJointPositionComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RobotJointPositionComponent()
          : base("Robot Joint Position", "RJ",
              "Defines a Robot Joint Position for a Declaration : Joint Target."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Robot axis value 1", "RA1", "Defines the position of robot axis 1 in degrees.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Robot axis value 2", "RA2", "Defines the position of robot axis 2 in degrees.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Robot axis value 3", "RA3", "Defines the position of robot axis 3 in degrees.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Robot axis value 4", "RA4", "Defines the position of robot axis 4 in degrees.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Robot axis value 5", "RA5", "Defines the position of robot axis 5 in degrees.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Robot axis value 6", "RA6", "Defines the position of robot axis 6 in degrees.", GH_ParamAccess.item, 0.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new RobotJointPositionParameter(), "Robot Joint Position", "RJ", "The resulting robot joint position");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Variables
            double internalAxisValue1 = 0.0;
            double internalAxisValue2 = 0.0;
            double internalAxisValue3 = 0.0;
            double internalAxisValue4 = 0.0;
            double internalAxisValue5 = 0.0;
            double internalAxisValue6 = 0.0;

            // Catch input data
            if (!DA.GetData(0, ref internalAxisValue1)) { return; }
            if (!DA.GetData(1, ref internalAxisValue2)) { return; }
            if (!DA.GetData(2, ref internalAxisValue3)) { return; }
            if (!DA.GetData(3, ref internalAxisValue4)) { return; }
            if (!DA.GetData(4, ref internalAxisValue5)) { return; }
            if (!DA.GetData(5, ref internalAxisValue6)) { return; }

            // Create external joint position
            RobotJointPosition robJointPosition = new RobotJointPosition(internalAxisValue1, internalAxisValue2, internalAxisValue3, internalAxisValue4, internalAxisValue5, internalAxisValue6);

            // Sets Output
            DA.SetData(0, robJointPosition);
        }

        // Methods for creating custom menu items and event handlers when the custom menu items are clicked
        #region menu items
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
            get { return Properties.Resources.RobotJointPosition_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B06D42A7-0568-4936-A86E-219242E36CFC"); }
        }

    }
}
