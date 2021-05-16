// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Definitions;
using RobotComponents.Utils;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Definitions
{
    /// <summary>
    /// RobotComponents Robot Tool Calibration component. An inherent from the GH_Component Class.
    /// </summary>
    public class RobotToolCalibrationComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear,  Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RobotToolCalibrationComponent()
          : base("Robot Tool Calibration", "ToolCal",
              "EXPERIMENTAL: Calculates the robot tool TCP from given joint positions."
            + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new RobotParameter(), "Robot", "R", "Robot", GH_ParamAccess.item);
            pManager.AddParameter(new RobotJointPositionParameter(), "Robot Joint Positions", "RJ", "Robot Joint Positions", GH_ParamAccess.list);
            pManager.AddParameter(new ExternalJointPositionParameter(), "External Joint Positions", "EJ", "External Joint Positions", GH_ParamAccess.list);

            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("TCP", "P", "Tool Center Point as a point", GH_ParamAccess.item);
            pManager.AddVectorParameter("Errors", "E", "Maximum calibration errors in x, y and z direction as a vector.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            Robot robot = new Robot();
            List<RobotJointPosition> robotJointPositions = new List<RobotJointPosition>();
            List<ExternalJointPosition> externalJointPositions = new List<ExternalJointPosition>();

            // Catch the input data
            if (!DA.GetData(0, ref robot)) { return; }
            if (!DA.GetDataList(1, robotJointPositions)) { return; }
            if (!DA.GetDataList(2, externalJointPositions)) { externalJointPositions = new List<ExternalJointPosition>() { new ExternalJointPosition() }; }

            // Construct object
            RobotToolCalibration calibration = new RobotToolCalibration(robot, robotJointPositions, externalJointPositions);
            List<string> errors = calibration.CheckJointPositionsAxisLimits();

            // Check given joint positions
            if (robotJointPositions.Count < 4 && externalJointPositions.Count < 4)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Provide at least 4 different joint positions.");
            }
            if (errors.Count != 0)
            {
                for (int i = 0; i < errors.Count; i++ )
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, errors[i]);
                }
            }

            // Calculate
            calibration.Calculate();

            // Outputs
            DA.SetData(0, calibration.TcpPoint);
            DA.SetData(1, calibration.MaximumError);
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
            get { return Properties.Resources.ToolCalibration_Icon; }
        }
 
        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("72FB2BFD-9A28-4CBE-9586-242CD29DE1E1"); }
        }
    }

}
