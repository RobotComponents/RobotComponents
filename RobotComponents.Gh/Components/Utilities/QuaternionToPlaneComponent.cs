// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Utilities
{
    /// <summary>
    /// RobotComponents convert quarternion to plane component. An inherent from the GH_Component Class.
    /// </summary>
    public class QuaternionToPlaneComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the QuarernionToPlane class
        /// </summary>
        public QuaternionToPlaneComponent()
          : base("Quaternion to Plane", "QtoP",
              "Converts quaternion values to a plane."
                + "The first value a is the real part, while the rest multiplies i, j and k, that are imaginary. "
                + System.Environment.NewLine + System.Environment.NewLine + "quarternion = a + bi + ci + dk"
                + System.Environment.NewLine + System.Environment.NewLine 
                + "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
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
            pManager.AddNumberParameter("Coord X", "X", "The x-coordinate of the plane origin.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Coord Y", "Y", "The y-coordinate of the plane origin.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Coord Z", "Z", "The z-coordinate of the plane origin.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Quaternion A", "A", "The real part of the quaternion.", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Quaternion B", "B", "The first imaginary coefficient of the quaternion.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Quaternion C", "C", "The second imaginary coefficient of the quaternion.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Quaternion D", "D", "The third imaginary coefficient of the quaternion.", GH_ParamAccess.item, 0.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_PlaneParam("Plane", "P", "Plane as a Plane.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            double x = 0.0;
            double y = 0.0;
            double z = 0.0;
            double A = 1.0;
            double B = 0.0;
            double C = 0.0;
            double D = 0.0;

            // Catch the input data
            if (!DA.GetData(0, ref x)) { return; }
            if (!DA.GetData(1, ref y)) { return; }
            if (!DA.GetData(2, ref z)) { return; }
            if (!DA.GetData(3, ref A)) { return; }
            if (!DA.GetData(4, ref B)) { return; }
            if (!DA.GetData(5, ref C)) { return; }
            if (!DA.GetData(6, ref D)) { return; }

            // Get plane
            Plane plane = RobotComponents.Utils.HelperMethods.QuaternionToPlane(x, y, z, A, B, C, D);

            // Output
            DA.SetData(0, plane);
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
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.QuatToPlane_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3AFD4001-E0F2-46E7-A885-19ADB3118D50"); }
        }
    }
}