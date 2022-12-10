// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Deconstruct.CodeGeneration
{
    /// <summary>
    /// RobotComponents Deconstruct Zone Data component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructZoneDataComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructZoneData class.
        /// </summary>
        public DeconstructZoneDataComponent()
          : base("Deconstruct Zone Data", "DeConZone", 
              "Deconstructs a Zone Data component into its parameters."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.ABB.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ZoneData(), "Zone Data", "ZD", "Zone Data as Zone Data or as a number", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string");
            pManager.Register_BooleanParam("Fine Point", "FP", "Defines whether the movement is to terminate as a stop point (fine point) or as a fly-by point as a bool.");
            pManager.Register_DoubleParam("Path Zone TCP", "pzTCP", "The size (the radius) of the TCP zone in mm as a number.");
            pManager.Register_DoubleParam("Path Zone Reorientation", "pzORI", "The zone size (the radius) for the tool reorientation in mm as a number.");
            pManager.Register_DoubleParam("Path Zone External Axes", "pzEA", "The zone size (the radius) for external axes in mm as a number.");
            pManager.Register_DoubleParam("Zone Reorientation", "zORI", "The zone size for the tool reorientation in degrees as a number.");
            pManager.Register_DoubleParam("Zone External Linear Axes", "zELA", "The zone size for linear external axes in mm as a number.");
            pManager.Register_DoubleParam("Zone External Rotational Axes", "zERA", "The zone size for rotating external axes in degrees as a number.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            ZoneData zoneData = null;

            // Catch the input data
            if (!DA.GetData(0, ref zoneData)) { return; }

            if (zoneData != null)
            {
                // Check if the object is valid
                if (!zoneData.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Zone Data is not valid");
                }

                // Output
                DA.SetData(0, zoneData.Name);
                DA.SetData(1, zoneData.FinePoint);
                DA.SetData(2, zoneData.PathZoneTCP);
                DA.SetData(3, zoneData.PathZoneOrientation);
                DA.SetData(4, zoneData.PathZoneExternalAxes);
                DA.SetData(5, zoneData.ZoneOrientation);
                DA.SetData(6, zoneData.ZoneExternalLinearAxes);
                DA.SetData(7, zoneData.ZoneExternalRotationalAxes);
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
            get { return Properties.Resources.DeconstructZoneData_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0AF80713-802A-49C8-84C2-E00671B369AB"); }
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