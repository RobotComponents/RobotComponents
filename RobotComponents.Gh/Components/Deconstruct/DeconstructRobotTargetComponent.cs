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
using RobotComponents.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Deconstruct
{
    /// <summary>
    /// RobotComponents Deconstruct Target component. An inherent from the GH_Component Class.
    /// </summary>
    public class DeconstructRobotTargetComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructTarget class.
        /// </summary>
        public DeconstructRobotTargetComponent()
          : base("Deconstruct Robot Target", "DeConRobTar", 
              "Deconstructs a Robot Target into its parameters."
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
            pManager.AddParameter(new RobotTargetParameter(), "Robot Target", "RT", "Robot Target as Robot Target", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_StringParam("Name", "N", "Name as string");
            pManager.Register_PlaneParam("Plane", "P", "Plane as Plane");
            pManager.Register_IntegerParam("Axis Configuration", "AC", "Axis Configuration as int.");
            pManager.RegisterParam(new ExternalJointPositionParameter(), "External Joint Position", "EJ", "The external axis value as an External Joint Position");
        }
   
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            RobotTarget target = null;

            // Catch the input data
            if (!DA.GetData(0, ref target)) { return; }

            // Check if the object is valid
            if (!target.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The Robot Target is not valid");
            }

            // Output
            DA.SetData(0, target.Name);
            DA.SetData(1, target.Plane);
            DA.SetData(2, target.AxisConfig);
            DA.SetData(3, target.ExternalJointPosition);
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
        /// Provides an Icon for the component
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DeconstructRobTarget_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2EE07DDD-B0F2-4111-BFD3-B88364B55415"); }
        }
    }
}