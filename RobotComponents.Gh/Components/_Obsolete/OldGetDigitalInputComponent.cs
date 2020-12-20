// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Gh.Forms;
using RobotComponents.Gh.Goos;
// ABB Robotic Libs
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.IOSystemDomain;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.07.000 (March 2020)
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.ControllerUtility
{
    /// <summary>
    /// RobotComponents Controller Utility : Get and read the Digital Inputs from a defined controller. An inherent from the GH_Component Class.
    /// </summary>
    public class OldGetDigitalInputComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetDigitalInput class.
        /// </summary>
        public OldGetDigitalInputComponent()
          : base("Get Digital Input", "GetDI",
              "OBSOLETE: Gets a digital input from a defined ABB robot controller."
                + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Controller Utility")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return true; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // To do: replace generic parameter with an RobotComponents Parameter
            pManager.AddGenericParameter("Robot Controller", "RC", "Controller to be connected to", GH_ParamAccess.item);
            pManager.AddTextParameter("DI Name", "N", "Digital Input Name as string", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // To do: replace generic parameter with an RobotComponents Parameter
            pManager.AddGenericParameter("Signal", "S", "The Digital Input Signal", GH_ParamAccess.item);
            pManager.AddBooleanParameter("State", "S", "The State of the Digital Input.", GH_ParamAccess.item);
        }

        // Fields
        private int _pickedIndex = 0;
        private static List<GH_Signal> _signalGooList = new List<GH_Signal>();
        private Controller _controller = null;
        private string _currentSignalName = "";
        private Guid _currentGuid = Guid.Empty;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Input variables
            GH_Controller controllerGoo = null;
            string nameIO = "";

            // Catch input data
            if (!DA.GetData(0, ref controllerGoo)) { return; }
            if (!DA.GetData(1, ref nameIO))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "IOName: No Signal Selected");
                nameIO = "";
            }

            // Get controller and logon
            _controller = controllerGoo.Value;
            _controller.Logon(UserInfo.DefaultUser); //TODO: Make user login

            // Output variables
            GH_Signal signalGoo;
            bool signalValue;

            // Check for null returns
            if (nameIO == null || nameIO == "")
            {
                signalGoo = PickSignal();
                CreatePanel(signalGoo.Value.Name.ToString());
            }
            else
            {
                signalGoo = GetSignal(nameIO);
            }

            // Declair Signal
            DigitalSignal signal = signalGoo.Value;

            // Convert Signal to bool 
            if (signal.Value == 1)
            {
                signalValue = true;
            }
            else
            {
                signalValue = false;
            }

            // Output
            DA.SetData(0, signalGoo);
            DA.SetData(1, signalValue);
        }

        // Additional methods
        #region additional methods
        /// <summary>
        /// Pick a signal
        /// </summary>
        /// <returns> The picked signal </returns>
        private GH_Signal PickSignal()
        {
            // Clear the list with signals
            _signalGooList.Clear();

            // Get the signal ins the robot controller
            SignalCollection signalCollection;
            signalCollection = _controller.IOSystem.GetSignals(IOFilterTypes.Input);

            // Initate the list with signal names
            List<string> signalNames = new List<string>();

            // Get all the signal names of available signals
            for (int i = 0; i < signalCollection.Count; i++)
            {
                // Check if there is write acces
                if (_controller.Configuration.Read("EIO", "EIO_SIGNAL", signalCollection[i].Name, "Access") != "ReadOnly")
                {
                    signalNames.Add(signalCollection[i].Name);
                    _signalGooList.Add(new GH_Signal(signalCollection[i] as DigitalSignal));
                }
            }

            // Return nothing if no signals were found
            if (_signalGooList.Count == 0 || _signalGooList == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No signal found with write access!");
                return null;
            }

            // Display the form with signal names and let the used pick one of the available signals
            _pickedIndex = DisplayForm(signalNames);

            // Return the picked signals if the index number of the picked signal is valid
            if (_pickedIndex >= 0)
            {
                return _signalGooList[_pickedIndex];
            }

            // Else return nothing if the user did not pick a signal
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No signal selected in menu");
                return null;
            }
        }

        /// <summary>
        /// Get the signal
        /// </summary>
        /// <param name="name"> The name of the signal. </param>
        /// <returns> The ABB Robotics signal. </returns>
        private GH_Signal GetSignal(string name)
        {
            // Check if the signal name is valid. Only check if the name is valid if the controller or the signal name changed.
            if (name != _currentSignalName || _controller.SystemId != _currentGuid)
            {
                if (!ValidSignal(name))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The Signal " + name + " does not exist in the current Controller");
                    return null;
                }

                // Update the current names
                _currentSignalName = (string)name.Clone();
                _currentGuid = new Guid(_controller.SystemId.ToString());
            }

            // Get the signal from the defined controller
            DigitalSignal signal = _controller.IOSystem.GetSignal(name) as DigitalSignal;

            // Check for null return
            if (signal != null)
            {
                return new GH_Signal(signal);
            }

            // If the signal is null: return nothing and raise a message. 
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Blank, "The picked signal does not exist!");
                return null;
            }
        }

        /// <summary>
        /// Creates the panel with the signal name and connects it to the input parameter
        /// </summary>
        /// <param name="text"> The signal name. </param>
        private void CreatePanel(string text)
        {
            // Create a panel
            Grasshopper.Kernel.Special.GH_Panel panel = new Grasshopper.Kernel.Special.GH_Panel();

            // Set the text with the signal name
            panel.SetUserText(text);

            // Change the color of the panel
            panel.Properties.Colour = System.Drawing.Color.White;

            // Get the current active canvas / document
            GH_Document doc = Grasshopper.Instances.ActiveCanvas.Document;

            // Add the panel to the active canvas
            doc.AddObject(panel, false);

            // Change the size of the panel
            panel.Attributes.Bounds = new System.Drawing.RectangleF(0.0f, 0.0f, 80.0f, 25.0f);

            // Set the location of the panel (relative to the location of the input parameter)
            panel.Attributes.Pivot = new System.Drawing.PointF(
                (float)this.Attributes.DocObject.Attributes.Bounds.Left - panel.Attributes.Bounds.Width - 30,
                (float)this.Params.Input[1].Attributes.Bounds.Y - 2);

            // Connect the panel to the input parameter
            Params.Input[1].AddSource(panel);
        }

        /// <summary>
        /// Check if the single name is valid (checks if the name exist in the controller).
        /// </summary>
        /// <param name="signalName"> The name of the signal. </param>
        /// <returns> Value that indicates if the signal name is valid. </returns>
        private bool ValidSignal(string signalName)
        {
            // Get the signals that are defined in the controller
            SignalCollection signalCollection;
            signalCollection = _controller.IOSystem.GetSignals(IOFilterTypes.Input);

            // Initiate the list with signal names
            List<string> signalNames = new List<string>();

            // Get all the signal names and add these to the list
            for (int i = 0; i < signalCollection.Count; i++)
            {
                // Check if there is write access
                if (_controller.Configuration.Read("EIO", "EIO_SIGNAL", signalCollection[i].Name, "Access") != "ReadOnly")
                {
                    signalNames.Add(signalCollection[i].Name);
                }
            }

            // Check if the signal name exist
            bool result = signalNames.Contains(signalName);

            // Return the value that indicates of the signal name is valid
            return result;
        }

        /// <summary>
        /// Displays the form with the names of the digital inputs and returns the index of the picked one. 
        /// </summary>
        /// <param name="IONames"> The list with names of the digital inputs. </param>
        /// <returns></returns>
        private int DisplayForm(List<string> IONames)
        {
            // Create the form
            PickDIForm frm = new PickDIForm(IONames);

            // Displays the form
            Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnEditor(frm, false);
            frm.ShowDialog();

            // Returns the index of the picked item
            return PickDIForm.SignalIndex;
        }

        /// <summary>
        /// Adds the additional item "Pick Signal" to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            // Add menu separator
            Menu_AppendSeparator(menu);

            // Create the menu item
            menu.Items.Add("Pick Signal", null, MenuItemClick);

            // Add the menu item
            base.AppendAdditionalMenuItems(menu);
        }

        /// <summary>
        /// Registers the event when the custom menu item is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClick(object sender, EventArgs e)
        {
            // Remove all the input source when the menu item is clicked. 
            this.Params.Input[1].RemoveAllSources();

            // Expire solution
            ExpireSolution(true);
        }
        #endregion

        /// <summary>
        /// The list with all the digital input signals
        /// </summary>
        public static List<GH_Signal> SignalGooList
        {
            get { return _signalGooList; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.GetDigitalInput_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("CB18B826-C66E-41AE-A102-B0C6BEB285B3"); }
        }
    }
}