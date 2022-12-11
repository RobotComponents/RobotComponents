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
using RobotComponents.ABB.Gh.Parameters.Controllers;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.ControllerUtility
{
    /// <summary>
    /// RobotComponents Controller Utility : Get and set the Digital Inputs on a defined controller. An inherent from the GH_Component Class.
    /// </summary>
    public class SetDigitalInputComponent : GH_Component
    {
        #region fields
        private Controller _controller;
        private Signal _signal = new Signal();
        #endregion

        /// <summary>
        /// Initializes a new instance of the SetDigitalInput class.
        /// </summary>
        public SetDigitalInputComponent()
          : base("Set Digital Input", "SetDI",
              "Changes the state of a defined digital input from an IRC5 robot controller in realtime."
               + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Controller Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Controller", "C", "Controller to connected to as Controller", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Name of the Digital Input Signal as text", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Value", "V", "State of the Digital Input as bool", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Update", "U", "Updates the Digital Input as bool", GH_ParamAccess.item, false);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_Signal(), "Signal", "S", "Digital Input Signal", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables      
            string name = "";
            bool value = false;
            bool update = false;

            // Catch input data
            if (!DA.GetData(0, ref _controller)) { return; }
            if (!DA.GetData(1, ref name)) { return; }
            if (!DA.GetData(2, ref value)) { return; }
            if (!DA.GetData(3, ref update)) { return; }

            // Get the signal
            _signal = _controller.GetSignal(name);

            // Update the signal
            if (update == true)
            {
                _controller.SetSignal(name, Convert.ToSingle(value));
            }

            // Output
            DA.SetData(0, _signal);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
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
            get { return Properties.Resources.SetDigitalOutput_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9DCABFA9-49E3-4D00-8298-3EE9B7C7D32F"); }
        }
        #endregion

        #region menu-items
        /// <summary>
        /// Adds the additional item "Pick Signal" to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Create Value List", MenuItemClick);
            //Menu_AppendSeparator(menu);
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

        /// <summary>
        /// Registers the event when the custom menu item is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClick(object sender, EventArgs e)
        {
            this.Params.Input[1].RemoveAllSources();
            HelperMethods.CreateValueList(this, _controller.GetDigitalInputNames(), 1);
            ExpireSolution(true);
        }
        #endregion
    }
}