// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Deconstruct
{
    /// <summary>
    /// RobotComponents Deconstruct External Rotational Axis Component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructExternalRotationalAxisComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructRobotTool class.
        /// </summary>
        public DeconstructExternalRotationalAxisComponent()
          : base("Deconstruct External Rotational Axis", "DeConERA", 
              "Deconstructs an External Rotational Axis component into its parameters."
         + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Deconstruct")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new ExternalRotationalAxisParameter(), "External Rotational Axis", "ERA", "External Rotational Axis as External Rotational Axis", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Axis Name as a Text", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Axis Plane", "AP", "Axis Plane as Plane", GH_ParamAccess.item);
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
            ExternalRotationalAxis externalRotationalAxis = null;

            // Catch the input data
            if (!DA.GetData(0, ref externalRotationalAxis)) { return; }

            // Check if the object is valid
            if (!externalRotationalAxis.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The External Rotational Axis is not valid");
            }

            // Output
            DA.SetData(0, externalRotationalAxis.Name);
            DA.SetData(1, externalRotationalAxis.AxisPlane);
            DA.SetData(2, externalRotationalAxis.AxisLimits);
            DA.SetData(3, externalRotationalAxis.BaseMesh);
            DA.SetData(4, externalRotationalAxis.LinkMesh);
            DA.SetData(5, externalRotationalAxis.AxisLogic);
            DA.SetData(6, externalRotationalAxis.MovesRobot);
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
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructExternalRotationalAxis_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("947F18D8-5789-485D-BD81-B93778124934"); }
        }
    }

}