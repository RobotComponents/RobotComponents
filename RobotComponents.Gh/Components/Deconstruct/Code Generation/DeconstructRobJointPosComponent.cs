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
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Deconstruct.CodeGeneration
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
            pManager.AddParameter(new Param_RobotJointPosition(), "Robot Joint Position", "RJ", "The Robot Joint Position.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Robot joint position variable name as text");
            pManager.Register_DoubleParam("Robot joint position 1", "RJ1", "Defines the position of robot joint 1 in degrees.");
            pManager.Register_DoubleParam("Robot joint position 2", "RJ2", "Defines the position of robot joint 2 in degrees.");
            pManager.Register_DoubleParam("Robot joint position 3", "RJ3", "Defines the position of robot joint 3 in degrees.");
            pManager.Register_DoubleParam("Robot joint position 4", "RJ4", "Defines the position of robot joint 4 in degrees.");
            pManager.Register_DoubleParam("Robot joint position 5", "RJ5", "Defines the position of robot joint 5 in degrees.");
            pManager.Register_DoubleParam("Robot joint position 6", "RJ6", "Defines the position of robot joint 6 in degrees.");
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
            DA.SetData(0, robJointPosition.Name);
            DA.SetData(1, robJointPosition[0]);
            DA.SetData(2, robJointPosition[1]);
            DA.SetData(3, robJointPosition[2]);
            DA.SetData(4, robJointPosition[3]);
            DA.SetData(5, robJointPosition[4]);
            DA.SetData(6, robJointPosition[5]);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructRobotJointPosition_Icon; } 
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F1308FD1-F3E3-43EC-8ACA-54990B1664FF"); }
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