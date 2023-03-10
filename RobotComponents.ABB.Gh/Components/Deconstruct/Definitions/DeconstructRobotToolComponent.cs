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
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.Deconstruct.Definitions
{
    /// <summary>
    /// RobotComponents Deconstruct Robot Tool component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructRobotToolComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructRobotTool class.
        /// </summary>
        public DeconstructRobotToolComponent()
          : base("Deconstruct Robot Tool", "DeRobTool",
              "Deconstructs a Robot Tool component into its parameters."
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
            pManager.AddParameter(new Param_RobotTool(), "Robot Tool", "RT", "Robot Tool as Robot Tool", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Robot Tool Name as Text", GH_ParamAccess.item);
            pManager.AddMeshParameter("Mesh", "M", "Robot Tool Mesh as Mesh", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Attachment Plane", "AP", "Robot Tool Attachment Plane as Plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Tool Plane", "TP", "Robot Tool Plane as Plane", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mass", "M", "The weight of the load in kg as a Number", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Center of Gravity", "CG", "The center of gravity of the toal load as a Plane.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Moment of Inertia", "MI", "Moment of intertia of the load in kgm2 as a Vector.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            RobotTool robotTool = null;

            // Catch the input data
            if (!DA.GetData(0, ref robotTool)) { return; }

            if (robotTool != null)
            {
                // Check if the object is valid
                if (!robotTool.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Robot Tool is not valid");
                }

                // Output
                DA.SetData(0, robotTool.Name);
                DA.SetData(1, robotTool.Mesh);
                DA.SetData(2, robotTool.AttachmentPlane);
                DA.SetData(3, robotTool.ToolPlane);
                DA.SetData(4, robotTool.Mass);
                DA.SetData(5, robotTool.CenterOfGravity);
                DA.SetData(6, robotTool.Inertia);
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
            get { return Properties.Resources.DeconstructRobotTool_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        { 
            get { return new Guid("830CC56C-ACEB-448D-A513-89AAA5414145"); }
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