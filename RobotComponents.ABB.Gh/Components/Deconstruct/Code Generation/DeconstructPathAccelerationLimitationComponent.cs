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

namespace RobotComponents.ABB.Gh.Components.Deconstruct.CodeGeneration
{
    /// <summary>
    /// RobotComponents Deconstruct Path Acceleration Limitation component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructPathAccelerationLimitationComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructPathAccelerationLimitationComponent class.
        /// </summary>
        public DeconstructPathAccelerationLimitationComponent()
          : base("Deconstruct Path Acceleration Limitation", "DePaAcLi",
              "Deconstructs a Path Acceleration Limitation into its parameters."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_PathAccelerationLimitation(), "Path Acceleration Limitation", "PAL", "Path Acceleration Limitation as Path Acceleration Limitation", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_BooleanParam("Acceleration Limitation", "AL", "Specifies whether or not the acceleration is limited as a Boolean.");
            pManager.Register_DoubleParam("Acceleration Max", "AM", "The absolute value of the acceleration limitation in m/s^2 as a Number.");
            pManager.Register_BooleanParam("Deceleration Limitation", "DL", "Specifies whether or not the deceleration is limited as a Boolean.");
            pManager.Register_DoubleParam("Deceleration Max", "DM", "The absolute value of the deceleration limitation in m/s^2 as a Number.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            PathAccelerationLimitation pathAccelerationLimitation = null;

            // Catch the input data
            if (!DA.GetData(0, ref pathAccelerationLimitation)) { return; }

            if (pathAccelerationLimitation != null)
            {
                // Check if the object is valid
                if (!pathAccelerationLimitation.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Path Acceleration Limitation is not valid");
                }

                // Output
                DA.SetData(0, pathAccelerationLimitation.AccelerationLimitation);
                DA.SetData(1, pathAccelerationLimitation.AccelerationMax);
                DA.SetData(2, pathAccelerationLimitation.DecelerationLimitation);
                DA.SetData(3, pathAccelerationLimitation.DecelerationMax);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
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
            get { return Properties.Resources.DeconstructPathAccelerationLimitation_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6D125379-6D2D-4CF0-98D7-2C30AB4C639D"); }
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