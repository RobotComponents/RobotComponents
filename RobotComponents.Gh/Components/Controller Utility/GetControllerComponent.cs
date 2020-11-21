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
using RobotComponents.Gh.Utils;
// ABB Libs
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;

namespace RobotComponents.Gh.Components.ControllerUtility
{
    /// <summary>
    /// RobotComponents Controller Utility : Get and connect to an ABB controller. An inherent from the GH_Component Class.
    /// </summary>
    public class GetControllerComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetController class.
        /// </summary>
        public GetControllerComponent()
          : base("Get Controller", "GC",
              "Connects to a real or virtual ABB IRC5 robot controller and extracts data from it."
                + System.Environment.NewLine + System.Environment.NewLine +
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
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Update", "U", "Update Controller as bool", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // To do: replace generic parameter with an RobotComponents Parameter
            pManager.AddGenericParameter("Robot Controller", "RC", "Resulting Robot Controller", GH_ParamAccess.item);
        }

        // Fields
        private int _pickedIndex = 0;
        private static List<Controller> _controllerInstance = new List<Controller>();
        private GH_Controller _controllerGoo;
        private bool _fromMenu = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            bool update = false;

            // Catch the input data
            if (!DA.GetData(0, ref update)) { return; }

            // Pick a new controller when the input is toggled or the user selects one sfrom the menu
            if (update || _fromMenu)
            {
                var controllerNow = GetController();
                if (controllerNow != null)
                {
                    _controllerGoo = new GH_Controller(controllerNow as Controller);
                }
                else
                {
                    return;
                }
            }

            // Output
            DA.SetData(0, _controllerGoo);

            // Recognizes if the component is deleted
            GH_Document doc = this.OnPingDocument();
            if (doc != null)
            {
                doc.ObjectsDeleted += DocumentObjectsDeleted;
            }
        }

        /// <summary>
        /// Detect if the components gets removed from the canvase and disposes the controller
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void DocumentObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            if (e.Objects.Contains(this))
            {
                if (_controllerGoo != null)
                {
                    if (_controllerGoo.Value != null)
                    {
                        Controller controller = _controllerGoo.Value;

                        if (controller.Connected == true)
                        {
                            controller.Logoff();
                        }
                        
                        controller.Dispose();
                    }
                }
            }
        }

        //  Additional methods
        #region additional methods
        /// <summary>
        /// Get the controller
        /// </summary>
        /// <returns> The picked controller. </returns>
        private Controller GetController()
        {
            // Initiate and clear variables
            _controllerInstance.Clear();
            ControllerInfo[] controllers;
            List<string> controllerNames = new List<string>() { };

            // Scan for a network with controller
            NetworkScanner scanner = new NetworkScanner();
            scanner.Scan();

            // Try to get the controllers from the netwerok
            try
            {
                controllers = scanner.GetControllers();
            }
            // Else return no controllers
            catch (Exception)
            {
                controllers = null;
            }
            
            // Get the names of all the controllers in the scanned network
            for (int i = 0; i < controllers.Length; i++)
            {
                _controllerInstance.Add(Controller.Connect(controllers[i], ConnectionType.Standalone));
                controllerNames.Add(_controllerInstance[i].Name);
            }

            // Automatically pick the controller when one controller is available. 
            if (controllerNames.Count == 1)
            {
                _pickedIndex = 0;
            }
            // Display the form and let the user pick a controller when more then one controller is available. 
            else if (controllerNames.Count > 1)
            {
                // Display the form and return the index of the picked controller. 
                _pickedIndex = DisplayForm(controllerNames);

                // Return a null value when the picked index is incorrect. 
                if (_pickedIndex < 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Station picked from menu!");
                    return null;
                }
            }

            // Return a null value when no controller was found
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Station found!");
                return null;
            }

            // Select the picked controller
            return _controllerInstance[_pickedIndex];
        }

        /// <summary>
        /// This method displays the form and return the index number of the picked controlller.
        /// </summary>
        /// <param name="controllerNames"> A list with controller names. </param>
        /// <returns> The index number of the picked controller. </returns>
        private int DisplayForm(List<string> controllerNames)
        {
            // Create the form with all the available controller names
            PickControllerForm frm = new PickControllerForm(controllerNames);

            // Display the form
            Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnEditor(frm, false);
            frm.ShowDialog();

            // Return the index number of the picked controller
            return PickControllerForm.StationIndex;
        }

        /// <summary>
        /// Adds the additional item "Pick controller" to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Pick Controller", MenuItemClick);
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
            _fromMenu = true;
            ExpireSolution(true);
            _fromMenu = false;
        }
        #endregion

        /// <summary>
        /// List with all the ABB controllers in the network
        /// </summary>
        public static List<Controller> ControllerInstance
        {
            get { return _controllerInstance; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.GetController_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6fd61c34-c262-4d10-b6e5-5c1762411aac"); }
        }
    }
}