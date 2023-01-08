// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// Robot Components Libs
using RobotComponents.ABB.Controllers;
using RobotComponents.ABB.Controllers.Forms;
using RobotComponents.ABB.Gh.Parameters.Controllers;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.ControllerUtility
{
    /// <summary>
    /// Represents the component that reads values from the configuration database. An inherent from the GH_Component Class.
    /// </summary>
    public class ReadConfigurationDomainComponent : GH_Component
    {
        #region fields
        private Controller _controller;
        #endregion

        /// <summary>
        /// Initializes a new instance of the ReadConfigurationDomain class.
        /// </summary>
        public ReadConfigurationDomainComponent()
          : base("Read Configuration Domain", "ReadConf",
              "Connects to a real or virtual ABB controller and extracts data from the configuration domain."
                + System.Environment.NewLine + System.Environment.NewLine +
                "This component uses the ABB PC SDK." +
                System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Controller Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Controller(), "Controller", "C", "Controller as Controller", GH_ParamAccess.item);
            pManager.AddTextParameter("Domain", "D", "The domain as Text", GH_ParamAccess.item, "");
            pManager.AddTextParameter("Type", "T", "The type as Text", GH_ParamAccess.item, "");
            pManager.AddTextParameter("Instance", "I", "The instance as Text", GH_ParamAccess.item, "");
            pManager.AddTextParameter("Attribute", "A", "The attribute as Text", GH_ParamAccess.item, "");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Data", "D", "Resulting data as Text", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare input variables
            string domain = "";
            string type = "";
            string instance = "";
            string attribute = "";

            // Catch the input data
            if (!DA.GetData(0, ref _controller)) { return; }
            if (!DA.GetData(1, ref domain)) { return; }
            if (!DA.GetData(2, ref type)) { return; }
            if (!DA.GetData(3, ref instance)) { return; }
            if (!DA.GetData(4, ref attribute)) { return; }

            string value = "";

            try
            {
                value = _controller.ReadConfigurationDomain(domain, type, instance, attribute);
            }
            catch
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Path not found!");
            }

            // Output
            DA.SetData(0, value);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.septenary; }
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
            get { return Properties.Resources.ReadDatabase_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0901438D-9049-4E33-84F6-1E7B1D709C40"); }
        }
        #endregion

        #region menu items
        /// <summary>
        /// Adds the additional item "Pick path" to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Pick Path", MenuItemClick);
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

        /// <summary>
        /// Registers the event when the custom menu item is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClick(object sender, EventArgs e)
        {
            PickConfigurationDomainPathForm frm = new PickConfigurationDomainPathForm(_controller);
            Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnEditor(frm, false);
            frm.ShowDialog();

            HelperMethods.CreatePanel(this, frm.Domain, 1);
            HelperMethods.CreatePanel(this, frm.Type, 2);
            HelperMethods.CreatePanel(this, frm.Instance, 3);
            HelperMethods.CreatePanel(this, frm.Attribute, 4);

            this.ExpireSolution(true);
        }
        #endregion
    }
}