// This file is part of RobotComponents. RobotComponents is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

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
    /// RobotComponents Deconstruct Speed Data component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructSpeedDataComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructSpeedData class.
        /// </summary>
        public DeconstructSpeedDataComponent()
          : base("Deconstruct Speed Data", "DeConSpeed", 
              "Deconstructs a Speed Data component into its parameters."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_SpeedData(), "Speed Data", "SD", "Speed Data as Speed Data or as a number", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string",GH_ParamAccess.item);
            pManager.Register_DoubleParam("TCP Velocity", "vTCP", "tcp velocity in mm/s as a number");
            pManager.Register_DoubleParam("ORI Velocity", "vORI", "reorientation velocity of the tool in degree/s as a number");
            pManager.Register_DoubleParam("LEAX Velocity", "vLEAX", "linear external axes velocity in mm/s as a number");
            pManager.Register_DoubleParam("REAX Velocity", "vREAX", "reorientation velocity of the external rotational axes in degrees/s as a number");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            SpeedData speedData = null;

            // Catch the input data
            if (!DA.GetData(0, ref speedData)) { return; }

            if (speedData != null)
            {
                // Check if the object is valid
                if (!speedData.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Speed Data is not valid");
                }

                // Output
                DA.SetData(0, speedData.Name);
                DA.SetData(1, speedData.V_TCP);
                DA.SetData(2, speedData.V_ORI);
                DA.SetData(3, speedData.V_LEAX);
                DA.SetData(4, speedData.V_REAX);
            }
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
            get { return Properties.Resources.DeconstructSpeedData_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2A20F8ED-850F-4E01-9318-5548356F8A3A"); }
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