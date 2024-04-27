﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.Definitions
{
    /// <summary>
    /// RobotComponents Deconstruct External Linear Axis Component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructExternalLinearAxisComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructExternalLinearAxisComponent class.
        /// </summary>
        public DeconstructExternalLinearAxisComponent()
          : base("Deconstruct External Linear Axis", "DeConELA",
              "Deconstructs an External Linear Axis component into its parameters."
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
            pManager.AddParameter(new Param_ExternalLinearAxis(), "External Linear Axis", "ELA", "External Linear Axis as External Linear Axis", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Axis Name as a Text", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Attachment Plane", "AP", "Attachment Plane as Plane", GH_ParamAccess.item);
            pManager.AddVectorParameter("Axis", "A", "Axis as Vector", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Domain", GH_ParamAccess.item);
            pManager.AddMeshParameter("Base Mesh", "BM", "Base Mesh as Mesh", GH_ParamAccess.item);
            pManager.AddMeshParameter("Link Mesh", "LM", "Link Mesh as Mesh", GH_ParamAccess.item);
            pManager.AddTextParameter("Axis Logic Number", "AL", "Axis Logic Number as Text", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Moves Robot", "MR", "Moves Robot as Boolean", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            ExternalLinearAxis externalLinearAxis = null;

            // Catch the input data
            if (!DA.GetData(0, ref externalLinearAxis)) { return; }

            if (externalLinearAxis != null)
            {
                // Check if the object is valid
                if (!externalLinearAxis.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The External Linear Axis is not valid");
                }

                // Output
                DA.SetData(0, externalLinearAxis.Name);
                DA.SetData(1, externalLinearAxis.AttachmentPlane);
                DA.SetData(2, externalLinearAxis.AxisPlane.ZAxis);
                DA.SetData(3, externalLinearAxis.AxisLimits);
                DA.SetData(4, externalLinearAxis.BaseMesh);
                DA.SetData(5, externalLinearAxis.LinkMesh);
                DA.SetData(6, externalLinearAxis.AxisLogic);
                DA.SetData(7, externalLinearAxis.MovesRobot);
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
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
            get { return Properties.Resources.DeconstructExternalLinearAxis_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6A7ABCC2-2EC1-4D9A-8DD4-57F0DD1BE681"); }
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