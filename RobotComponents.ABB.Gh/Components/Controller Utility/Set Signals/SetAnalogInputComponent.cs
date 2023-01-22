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
// Robot Components Libs
using RobotComponents.ABB.Controllers;
using RobotComponents.ABB.Gh.Parameters.Controllers;
using RobotComponents.ABB.Gh.Utils;
using RobotComponents.ABB.Controllers.Forms;

namespace RobotComponents.ABB.Gh.Components.ControllerUtility
{
    /// <summary>
    /// Represents the component that gets and sets analog inputs on a defined controller. An inherent from the GH_Component Class.
    /// </summary>
    public class SetAnalogInputComponent : GH_Component
    {
        #region fields
        private Controller _controller;
        #endregion

        /// <summary>
        /// Initializes a new instance of the SetAnalogIntputComponent class.
        /// </summary>
        public SetAnalogInputComponent()
          : base("Set Analog Input", "SetAI",
              "Changes the state of a defined analog input from an ABB controller in realtime."
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
            pManager.AddGenericParameter("Controller", "C", "Controller to connected to as Controller", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Name of the Analog Intput Signal as text", GH_ParamAccess.item);
            pManager.AddNumberParameter("Value", "V", "State of the Analog Input as a number", GH_ParamAccess.item, 0.0);
            pManager.AddBooleanParameter("Update", "U", "Updates the Analog Input as bool", GH_ParamAccess.item, false);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_Signal(), "Signal", "S", "Analog Input Signal", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables      
            string name = "";
            double value = 0.0;
            bool update = false;

            // Catch input data
            if (!DA.GetData(0, ref _controller)) { return; }
            if (!DA.GetData(1, ref name)) { return; }
            if (!DA.GetData(2, ref value)) { return; }
            if (!DA.GetData(3, ref update)) { return; }

            // Define an empty signal
            Signal signal = new Signal();

            // Get the signal
            try
            {
                signal = _controller.GetAnalogInput(name, out int index);

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
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            }

            // Update the signal
            if (update == true)
            {
                bool success = signal.SetValue(Convert.ToSingle(value), out string msg);

                if (success == false)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, msg);
                }
            }

            // Output
            DA.SetData(0, signal);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary | GH_Exposure.obscure; }
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
            get { return Properties.Resources.SetAnalogInput_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C799F319-75D6-4F0D-BCAD-8E2ECACEB0B8"); }
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
                this.Params.Input[1].RemoveAllSources();
                HelperMethods.CreatePanel(this, name, 1);
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
            List<Signal> signals = _controller.AnalogInputs;
            name = "";

            if (signals.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No analog input signals found!");
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
                Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnEditor(frm, false);
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