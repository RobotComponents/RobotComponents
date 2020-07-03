// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponentsABB.Parameters.Actions;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components.Deconstruct
{
    /// <summary>
    /// RobotComponents Deconstruct Rob Joint Position component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructRobJointPosComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructExtJointPos class.
        /// </summary>
        public DeconstructRobJointPosComponent()
          : base("Deconstruct Robot Joint Position", "DeConRobJoint",
              "Deconstructs a Robot Joint Position Component into its parameters."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new RobotJointPositionParameter(), "Robot Joint Position", "RJ", "The Robot Joint Position.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("Robot axis value 1", "RA1", "Defines the position of robot axis 1 in degrees.");
            pManager.Register_DoubleParam("Robot axis value 2", "RA2", "Defines the position of robot axis 2 in degrees.");
            pManager.Register_DoubleParam("Robot axis value 3", "RA3", "Defines the position of robot axis 3 in degrees.");
            pManager.Register_DoubleParam("Robot axis value 4", "RA4", "Defines the position of robot axis 4 in degrees.");
            pManager.Register_DoubleParam("Robot axis value 5", "RA5", "Defines the position of robot axis 5 in degrees.");
            pManager.Register_DoubleParam("Robot axis value 6", "RA6", "Defines the position of robot axis 6 in degrees.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            RobotJointPosition robJointPosition = null;

            // Catch the input data
            if (!DA.GetData(0, ref robJointPosition)) { return; }

            // Check if the object is valid
            if (!robJointPosition.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Robot Joint Position is not valid");
            }

            // Output
            DA.SetData(0, robJointPosition[0]);
            DA.SetData(1, robJointPosition[1]);
            DA.SetData(2, robJointPosition[2]);
            DA.SetData(3, robJointPosition[3]);
            DA.SetData(4, robJointPosition[4]);
            DA.SetData(5, robJointPosition[5]);
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
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return null; } // TODO
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("909666AF-E627-4FB1-A54E-F9557E400211"); }
        }
    }
}