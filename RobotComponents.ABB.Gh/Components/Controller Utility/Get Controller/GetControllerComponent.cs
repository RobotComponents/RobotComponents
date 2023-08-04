// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using GH_IO.Serialization;
// Robot Components Libs
using RobotComponents.ABB.Controllers;
using RobotComponents.ABB.Controllers.Forms;
using RobotComponents.ABB.Gh.Parameters.Controllers;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.ControllerUtility
{
    /// <summary>
    /// Represents the component that gets an ABB controller from the network. An inherent from the GH_Component Class.
    /// </summary>
    public class GetControllerComponent : GH_Component
    {
        #region fields
        private Controller _controller = new Controller();
        private bool _fromMenu = false;
        private bool _picked = false;
        #endregion

        /// <summary>
        /// Initializes a new instance of the GetControllerComponent class.
        /// </summary>
        public GetControllerComponent()
          : base("Get Controller", "GetCont",
              "Connects to a real or virtual ABB controller."
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
            pManager.AddBooleanParameter("Update", "U", "Update Controller as bool", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_Controller(), "Controller", "C", "Resulting Controller", GH_ParamAccess.item);
        }

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

            // Pick a new controller when the input is toggled or the user selects one from the menu
            if (update || _fromMenu)
            {
                bool succeeded = this.GetController();
                
                if (succeeded)
                {
                    _picked = true;
                    _controller.Logon();

                    GH_Document doc = this.OnPingDocument();
                    doc.ContextChanged += OnContextChanged;
                }
                else
                {
                    _picked = false;
                }

                _fromMenu = false;
            }
            else
            {
                if (_picked == false)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No controller picked from the menu!");
                }
            }

            // Output
            DA.SetData(0, _controller);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
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
            get { return Properties.Resources.GetController_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B3515FD1-290E-4B1D-997E-AC551FE0E04C"); }
        }
        #endregion

        #region menu items
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

        #region serialization
        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("From Menu", _fromMenu);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            _fromMenu = reader.GetBoolean("From Menu");
            return base.Read(reader);
        }
        #endregion

        #region additional methods
        /// <summary>
        /// This method will be called when an object is removed from a document. Override this method if you want to handle such events.
        /// </summary>
        /// <param name="document"> Document that now no longer owns this object. </param>
        public override void RemovedFromDocument(GH_Document document)
        {
            base.RemovedFromDocument(document);

            if (_controller != null)
            {
                if (_controller.IsValid)
                {
                    _controller.Logoff();
                    _controller.Dispose();
                }
            }
        }

        /// <summary>
        /// Method that is called when then document context changed. 
        /// This will log off and dispose the controller when the document is changed. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void OnContextChanged(object sender, GH_DocContextEventArgs e)
        {
            if (e.Context == GH_DocumentContext.Close)
            {
                if (_controller != null)
                {
                    _controller.Logoff();
                    _controller.Dispose();
                }
            }
        }

        /// <summary>
        /// Get the controller
        /// </summary>
        /// <returns> Indicates whether or not a controller was picked successfully. </returns>
        private bool GetController()
        {
            Controller.GetControllers();

            if (Controller.Controllers.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No controller found!");
                return false;
            }

            else if (Controller.Controllers.Count == 1)
            {
                _controller = Controller.Controllers[0];
                _controller.Initiliaze();
                return true;
            }

            else if (Controller.Controllers.Count > 1)
            {
                PickControllerForm form = new PickControllerForm();
                //bool result = form.ShowModal(Instances.EtoDocumentEditor); // Rhino 7 or higher
                bool result = form.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow);

                if (result)
                {
                    _controller = form.Controller;
                    _controller.Initiliaze();
                    return true;
                }
                else if (_controller.IsEmpty)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No controller picked from menu!");
                    return false;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
        #endregion
    }
}