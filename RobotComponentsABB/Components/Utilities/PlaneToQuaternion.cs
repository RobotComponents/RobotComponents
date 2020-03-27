// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components.Utilities
{
    /// <summary>
    /// RobotComponents convert plane orientation to quarternion component. An inherent from the GH_Component Class.
    /// </summary>
    public class PlaneToQuaternion : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Plane to Quarternion
        /// </summary>
        public PlaneToQuaternion()
          : base("Plane to Quaternion", "PtoQ",
              "Calculates the four coefficient values in a quarternion. "
                + "The first value a is the real part, while the rest multiplies i, j and k, that are imaginary. "
                + System.Environment.NewLine + System.Environment.NewLine + "quarternion = a + bi + ci + dk"
                + System.Environment.NewLine + System.Environment.NewLine 
                + "RobotComponents : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("Coord X", "X", "The x-coordinate of the plane origin.");
            pManager.Register_DoubleParam("Coord Y", "Y", "The y-coordinate of the plane origin.");
            pManager.Register_DoubleParam("Coord Z", "Z", "The z-coordinate of the plane origin.");
            pManager.Register_DoubleParam("Quaternion A", "A", "The real part of the quaternion.");
            pManager.Register_DoubleParam("Quaternion B", "B", "The first imaginary coefficient of the quaternion.");
            pManager.Register_DoubleParam("Quaternion C", "C", "The second imaginary coefficient of the quaternion.");
            pManager.Register_DoubleParam("Quaternion D", "D", "The third imaginary coefficient of the quaternion.");
            pManager.Register_StringParam("String", "STR", "The quaternion in string format that can be used in RAPID code.");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            List<Plane> planes = new List<Plane>();

            // Catch the input data
            if (!DA.GetDataList(0, planes)) { return; }

            // Output variables
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            List<double> z = new List<double>();
            List<double> quatA = new List<double>();
            List<double> quatB = new List<double>();
            List<double> quatC = new List<double>();
            List<double> quatD = new List<double>();
            List<string> text = new List<string>();

            // Create variables for necessary for getting the quarternion
            Quaternion quat;
            Plane refPlane = Plane.WorldXY;

            // Loop over all the input planes and get the quarternion of the plane
            for (int i = 0; i < planes.Count; i++)
            {
                // Get the individual plane
                Plane plane = planes[i];

                // Get the quarternion
                quat = Quaternion.Rotation(refPlane, plane);

                // Save the four quarternion values
                quatA.Add(quat.A);
                quatB.Add(quat.B);
                quatC.Add(quat.C);
                quatD.Add(quat.D);

                // Coords
                x.Add(plane.Origin.X);
                y.Add(plane.Origin.Y);
                z.Add(plane.Origin.Z);

                // Write the quarternion value in the string format that is used in the RAPID and BASE code
                text.Add("[" 
                    + quat.A.ToString("0.######") + ", " 
                    + quat.B.ToString("0.######") + ", " 
                    + quat.C.ToString("0.######") + ", " 
                    + quat.D.ToString("0.######") + "]");
            }

            // Output
            DA.SetDataList(0, x);
            DA.SetDataList(1, y);
            DA.SetDataList(2, z);
            DA.SetDataList(3, quatA);
            DA.SetDataList(4, quatB);
            DA.SetDataList(5, quatC);
            DA.SetDataList(6, quatD);
            DA.SetDataList(7, text);
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
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.PlaneToQuat_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D606FF44-74B7-4312-9198-FE68B47F825F"); }
        }
    }
}