// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
// Robot Components Libs
using RobotComponents.ABB.Controllers;
using RobotComponents.ABB.Controllers.Gh.Parameters.Controllers;

namespace RobotComponents.ABB.Controllers.Gh.Components.ControllerUtility
{
    /// <summary>
    /// RobotComponents Controller Utility : Get the Axis Values from a defined controller. An inherent from the GH_Component Class.
    /// </summary>
    public class GetExternalJointPositionComponent : GH_Component
    {
        #region fields
        private Controller _controller;
        private Dictionary<string, double[]> _externalJointPositions;
        #endregion

        /// <summary>
        /// Initializes a new instance of the GetAxisValues class.
        /// </summary>
        public GetExternalJointPositionComponent()
          : base("Get External Joint Position", "GEJP",
              "Gets the current external joint position from an ABB IRC5 robot controller."
               + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.ABB.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Controller Utility 2.0-beta")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Controller(), "Controller", "C", "Controller as Controller", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            //TODO: Change generic parameter to Param_ExternalJointPosition

            pManager.AddTextParameter("Name", "N", "Name of the external axis as Text", GH_ParamAccess.list);
            pManager.AddNumberParameter("External Joint Position", "EJ", "Extracted External Joint Positions", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Catch input data
            if (!DA.GetData(0, ref _controller)) { return; }

            _externalJointPositions = _controller.GetExternalJointPositions();

            // Output
            DA.SetDataList(0, _externalJointPositions.Keys);
            DA.SetDataTree(1, this.ToDataTree(_externalJointPositions));
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
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
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.GetAxisValues_Icon; ; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8D819F0C-A554-4D73-BE40-771043A33FFA"); }
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
            //Menu_AppendItem(menu, "Documentation", MenuItemClickComponentDoc, Properties.Resources.WikiPage_MenuItem_Icon);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Documentation" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickComponentDoc(object sender, EventArgs e)
        {
            //string url = Documentation.ComponentWeblinks[this.GetType()];
            //Documentation.OpenBrowser(url);
        }
        #endregion

        #region methods
        private GH_Structure<GH_Number> ToDataTree(Dictionary<string, double[]> data)
        {
            GH_Structure<GH_Number> result = new GH_Structure<GH_Number>();
            
            int counter = 0;

            foreach (KeyValuePair<string, double[]> entry in data)
            {
                GH_Path path = new GH_Path(counter);

                for (int i = 0; i < entry.Value.Length; i++)
                {
                    result.Append(new GH_Number(entry.Value[i]), path);
                }
                
                counter++;
            }

            return result;
        }
        #endregion
    }
}