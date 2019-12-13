using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Resources;
using RobotComponents.Goos;
// ABB Robotic Libs
using ABB.Robotics.Controllers;

namespace RobotComponents.Components
{
    public class SetDigitalOutputComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetDigitalOutput class.
        /// </summary>
        public SetDigitalOutputComponent()
          : base("Set Digital Output", "SetDO",
              "Sets the signal of a digital output for the defined ABB robot controller."
                + System.Environment.NewLine + 
                "RobotComponent V : " + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Controller Utility")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Robot Controller", "RC", "Controller to be connected to", GH_ParamAccess.item);
            pManager.AddGenericParameter("DO Name", "N", "Name of the Digital Output", GH_ParamAccess.item);
            pManager.AddBooleanParameter("State", "S", "State of the Digital Output", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Update", "U", "Updates the Digital Input", GH_ParamAccess.item, false);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
            {
                pManager.AddGenericParameter("Signal", "S", "The Signal of the Digital Output", GH_ParamAccess.item);
            }

        // Global component variables
        public int pickedIndex = 0;
        public static List<SignalGoo> signalGooList = new List<SignalGoo>();
        ABB.Robotics.Controllers.Controller controller = null;
        SignalGoo signalGoo;

        string currentSignalName = "";
        string currentSystemName = "";
        string currentCtrName = "";

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables      
            ControllerGoo controllerGoo = null;
            string nameIO = "";
            bool signalValue = false;
            bool update = false;

            // Catch input data
            if (!DA.GetData(0, ref controllerGoo)) { return; }
            if (!DA.GetData(1, ref nameIO))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "IOName: No Signal Selected");
                nameIO = "";
            }
            if (!DA.GetData(2, ref signalValue)) { return; }
            if (!DA.GetData(3, ref update)) { return; }

            // Get controller and logon
            controller = controllerGoo.Value;
            controller.Logon(UserInfo.DefaultUser);

            // Initiate signal values
            signalGoo = null;
            ABB.Robotics.Controllers.IOSystemDomain.DigitalSignal signal; 

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

            // Update the signal value: send value to the controller when the input is toggled
            if (update == true)
            {
                signal = signalGoo.Value;

                // Set Digital Output Value
                if (signalValue)
                {
                    signal.Value = 1;
                }
                else
                {
                    signal.Value = 0;
                }
            }

            // Output
            DA.SetData(0, signalGoo);
        }

        // Addtional methods
        #region addiotnal methods
        /// <summary>
        /// Pick a signal
        /// </summary>
        /// <returns> The picked signal </returns>
        private SignalGoo PickSignal()
        {
            // Clear the list with signals
            signalGooList.Clear();

            // Get the signal ins the robot controller
            ABB.Robotics.Controllers.IOSystemDomain.SignalCollection signalCollection;
            signalCollection = controller.IOSystem.GetSignals(ABB.Robotics.Controllers.IOSystemDomain.IOFilterTypes.Output);

            // Initate the list with signal names
            List<string> signalNames = new List<string>();

            // Get all the signal names of available signals
            for (int i = 0; i < signalCollection.Count; i++)
            {
                // Check if there is write acces
                if (controller.Configuration.Read("EIO", "EIO_SIGNAL", signalCollection[i].Name, "Access") != "ReadOnly")
                {
                    signalNames.Add(signalCollection[i].Name);
                    signalGooList.Add(new SignalGoo(signalCollection[i] as ABB.Robotics.Controllers.IOSystemDomain.DigitalSignal));
                }
            }

            // Return nothing if no signals were found
            if (signalGooList.Count == 0 || signalGooList == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No signal found with write access!");
                return null;
            }

            // Display the form with signal names and let the used pick one of the available signals
            pickedIndex = DisplayForm(signalNames);

            // Return the picked signals if the index number of the picked signal is valid
            if (pickedIndex >= 0)
            {
                return signalGooList[pickedIndex];
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
        private SignalGoo GetSignal(string name)
        {
            // Check if the signal name is valid. Only check if the name is valid if the controller or the signal name changed.
            if (name != currentSignalName || controller.SystemName != currentSystemName || controller.Name != currentCtrName)
            {
                if (!ValidSignal(name))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The Signal " + name + " does not exist in the current Controller");
                    return null;
                }

                // Update the current names
                currentSignalName = (string)name.Clone();
                currentSystemName = (string)controller.SystemName.Clone();
                currentCtrName = (string)controller.Name.Clone();
            }

            // Get the signal from the defined controller
            ABB.Robotics.Controllers.IOSystemDomain.Signal signal = controller.IOSystem.GetSignal(name) as ABB.Robotics.Controllers.IOSystemDomain.Signal;

            // Check for null return
            if (signal != null)
            {
                // Check the write acces of the signal
                if (controller.Configuration.Read("EIO", "EIO_SIGNAL", signal.Name, "Access") == "ReadOnly")
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Blank, "The picked signal is ReadOnly!");
                }

                // Return the selected signal
                return new SignalGoo(signal as ABB.Robotics.Controllers.IOSystemDomain.DigitalSignal);
            }
            // If the signal is null: return nothing and reaise a message. 
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
            ABB.Robotics.Controllers.IOSystemDomain.SignalCollection signalCollection;
            signalCollection = controller.IOSystem.GetSignals(ABB.Robotics.Controllers.IOSystemDomain.IOFilterTypes.Output);

            // Initiate the list with signal names
            List<string> signalNames = new List<string>();

            // Get all the signal names and add these to the list
            for (int i = 0; i < signalCollection.Count; i++)
            {
                // Check if there is write access
                if (controller.Configuration.Read("EIO", "EIO_SIGNAL", signalCollection[i].Name, "Access") != "ReadOnly")
                {
                    signalNames.Add(signalCollection[i].Name);
                }
            }

            // Check if the signal name exist
            bool result = signalNames.Contains(signalName);

            // Return the value that indicates if the signal name is valid
            return result;
        }

        /// <summary>
        /// Displays the form with the names of the digital outputs and returns the index of the picked one. 
        /// </summary>
        /// <param name="IONames"> The list with names of the digital outputs. </param>
        /// <returns></returns>
        private int DisplayForm(List<string> IONames)
        {
            // Create the form
            PickDOForm frm = new PickDOForm(IONames);

            // Displays the form
            Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnEditor(frm, false);
            frm.ShowDialog();

            // Returns the index of the picked item
            return PickDOForm.stationIndex;
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
        public void MenuItemClick(object sender, EventArgs e)
        {
            // Remove all the input source when the menu item is clicked. 
            this.Params.Input[1].RemoveAllSources();

            // Expire solution
            ExpireSolution(true);
        }
        #endregion

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
            get { return new Guid("8fbb97b5-cdc4-41de-a33d-df1d6f7bda21"); }
        }
    }
}