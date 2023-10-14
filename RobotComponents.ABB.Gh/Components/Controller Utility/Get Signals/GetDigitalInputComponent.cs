// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
// Robot Components Libs
using RobotComponents.ABB.Controllers;
using RobotComponents.ABB.Gh.Parameters.Controllers;
using RobotComponents.ABB.Gh.Utils;
using RobotComponents.ABB.Controllers.Forms;

namespace RobotComponents.ABB.Gh.Components.ControllerUtility
{
    /// <summary>
    /// Represents the component that gets digital inputs from a defined controller. An inherent from the GH_Component Class.
    /// </summary>
    public class GetDigitalInputComponent : GH_Component
    {
        #region fields
        private Controller _controller;
        #endregion

        /// <summary>
        /// Initializes a new instance of the GetDigitalInputComponent class.
        /// </summary>
        public GetDigitalInputComponent()
          : base("Get Digital Input", "GetDI",
              "Gets the signal of a defined digital input from an ABB controller."
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
            pManager.AddGenericParameter("Controller", "C", "Controller to be connected to as Controller", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Digital Input Name as text", GH_ParamAccess.item);
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

            // Catch input data
            if (!DA.GetData(0, ref _controller)) { return; }
            if (!DA.GetData(1, ref name)) { return; }

            try
            {
                Signal signal  = _controller.GetDigitalInput(name, out int index);

                if (index == -1)
                {
                    if (_controller.IsEmpty == true)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Could not get the signal {name}. The controller is empty.");
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Could not get the signal {name}. Signal not found.");
                    }
                }

                // Output
                DA.SetData(0, signal);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            }
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
            get { return Properties.Resources.GetDigitalInput_Icon; ; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B45A32B4-D8DF-42FD-98D4-479BAAB9341E"); }
        }
        #endregion

        #region menu item
        /// <summary>
        /// Adds the additional item "Pick Signal" to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Pick Signal", MenuItemClick);
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
            if (this.GetSignal(out string name) == true)
            {
                if (this.Params.Input[1].Sources.Count == 1 && this.Params.Input[1].Sources[0] is GH_Panel panel)
                {
                    panel.SetUserText(name);
                }
                else
                {
                    HelperMethods.CreatePanel(this, name, 1);
                }

                this.ExpireSolution(true);
            }
        }
        #endregion

        #region addtional methods
        /// <summary>
        /// Get the signal
        /// </summary>
        /// <returns> Indicates whether or not the signal was picked successfully. </returns>
        private bool GetSignal(out string name)
        {
            List<Signal> signals = _controller.DigitalInputs;
            name = "";

            if (signals.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No digital input signals found!");
                return false;
            }

            else if (signals.Count == 1)
            {
                name = signals[0].Name;
                return true;
            }

            else if (signals.Count > 1)
            {
                PickSignalForm frm = new PickSignalForm(signals);
                Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnScreen(frm, false);
                frm.ShowDialog();
                int index = frm.Index;

                if (index < 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No signal picked from the menu!");
                    return false;
                }
                else
                {
                    name = signals[index].Name;
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}