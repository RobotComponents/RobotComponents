// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.Definitions
{
    /// <summary>
    /// RobotComponents Deconstruct Load Data component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructLoadDataComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructLoadData class.
        /// </summary>
        public DeconstructLoadDataComponent()
          : base("Deconstruct Load Data", "DeLoDa",
              "Deconstructs a Load Data component into its parameters."
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
            pManager.AddParameter(new Param_LoadData(), "Load Data", "LD", "Load Data as Load Data", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Load Data Name as Text", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mass", "M", "The weight of the load in kg as a Number.", GH_ParamAccess.item);
            pManager.AddPointParameter("Center of Gravity", "CG", "The center of gravity of the load as a Point.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Axes of Moment", "AM", "The orientation of the load coordinate system defined by the principal inertial axes of the tool as a Plane.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Inertial Moments", "IM", "the moment of inertia of the load in kgm2 as a Vector.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            LoadData loadData = null;

            // Catch the input data
            if (!DA.GetData(0, ref loadData)) { return; }

            if (loadData != null)
            {
                // Check if the object is valid
                if (!loadData.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Load Data is not valid");
                }

                // Output
                DA.SetData(0, loadData.Name);
                DA.SetData(1, loadData.Mass);
                DA.SetData(2, loadData.CenterOfGravity);
                DA.SetData(3, ABB.Utils.HelperMethods.QuaternionToPlane(new Point3d(0, 0, 0), loadData.AxesOfMoment));
                DA.SetData(4, loadData.InertialMoments);
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
            get { return null; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("670E1BAC-6F56-4B9E-BE44-4677A4EF8E75"); }
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