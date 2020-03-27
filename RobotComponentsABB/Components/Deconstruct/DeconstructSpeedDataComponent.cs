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
using RobotComponentsGoos.Actions;
using RobotComponentsABB.Parameters.Actions;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components.Deconstruct
{
    /// <summary>
    /// RobotComponents Deconstruct Speed Data component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructSpeedDataComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructSpeedData class.
        /// </summary>
        public DeconstructSpeedDataComponent()
          : base("Deconstruct Speed Data", "DeConSpeed",
              "Deconstructs a Speed Data Component into its parameters."
                + System.Environment.NewLine +
                "RobotComponents : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new SpeedDataParameter(), "Speed Data", "SD", "Speed Data as Custom Speed Data or as a number", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string",GH_ParamAccess.item);
            pManager.Register_DoubleParam("TCP Velocity", "vTCP", "tcp velocity in mm/s as a number");
            pManager.Register_DoubleParam("ORI Velocity", "vORI", "reorientation of the tool in degree/s as a number");
            pManager.Register_DoubleParam("LEAX Velocity", "vLEAX", "linear external axes velocity in mm/s as a number");
            pManager.Register_DoubleParam("REAX Velocity", "vREAX", "reorientation of the external rotational axes in degrees/s as a number");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            GH_SpeedData speedDataGoo = null;

            // Catch the input data
            if (!DA.GetData(0, ref speedDataGoo)) { return; }

            // Output
            DA.SetData(0, speedDataGoo.Value.Name);
            DA.SetData(1, speedDataGoo.Value.V_TCP);
            DA.SetData(2, speedDataGoo.Value.V_ORI);
            DA.SetData(3, speedDataGoo.Value.V_LEAX);
            DA.SetData(4, speedDataGoo.Value.V_REAX);
        }

        #region menu item
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            // Add menu separator
            Menu_AppendSeparator(menu);

            // Add custom menu items
            Menu_AppendItem(menu, "Documentation", MenuItemClickComponentDoc, Properties.Resources.WikiPage_MenuItem_Icon);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Documentation" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        public void MenuItemClickComponentDoc(object sender, EventArgs e)
        {
            string url = Documentation.ComponentWeblinks[this.GetType()];
            System.Diagnostics.Process.Start(url);
        }
        #endregion

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructSpeedData_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2A20F8ED-850F-4E01-9318-5548356F8A3A"); }
        }
    }
}