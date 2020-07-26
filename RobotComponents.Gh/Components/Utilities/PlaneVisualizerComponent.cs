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
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Utilities
{
    /// <summary>
    /// RobotComponents Plane visualization component. An inherent from the GH_Component Class.
    /// </summary>
    public class PlaneVisualizerComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public PlaneVisualizerComponent()
          : base("Plane Visualizer", "PV",
              "Visualizes the orientation vectors of a plane."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // This component has no ouput parameters. It only visualizes the plane orientation.
        }

        // Fields
        private List<Plane> _planes = new List<Plane>();

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Clear the list with plans before catching the input data
            _planes.Clear();

            // Create an empty datatree structure for catching the input data
            GH_Structure<GH_Plane> inputPlanes;

            // Catch input data
            if (!DA.GetDataTree(0, out inputPlanes)) { return; }

            // Flatten the datatree to a list
            for (int i = 0; i < inputPlanes.Branches.Count; i++)
            {
                var branches = inputPlanes.Branches[i];

                for (int j = 0; j < branches.Count; j++)
                {
                    _planes.Add(branches[j].Value);
                }
            }
        }

        /// <summary>
        /// This methods displays the three vectors of all the planes in the list.
        /// </summary>
        /// <param name="args"> Preview display arguments for IGH_PreviewObjects. </param>
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            for (int i = 0; i < _planes.Count; i++)
            {
                Plane plane = _planes[i];

                if (plane != null)
                {
                    if (plane.IsValid == true)
                    {
                        args.Display.DrawDirectionArrow(plane.Origin, plane.ZAxis, System.Drawing.Color.Blue);
                        args.Display.DrawDirectionArrow(plane.Origin, plane.XAxis, System.Drawing.Color.Red);
                        args.Display.DrawDirectionArrow(plane.Origin, plane.YAxis, System.Drawing.Color.Green);
                    }
                }
            }
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
            get { return Properties.Resources.Plane_Visualizer_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F861C697-DE9D-483E-9651-C53649775412"); }
        }

    }
}
