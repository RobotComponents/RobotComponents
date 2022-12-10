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
using GH_IO.Serialization;
// Robot Components Libs
using RobotComponents.ABB.Controllers;
using RobotComponents.ABB.Controllers.Gh.Parameters.Controllers;
using RobotComponents.ABB.Controllers.Forms;

namespace RobotComponents.ABB.Controllers.Gh.Components.ControllerUtility
{
    /// <summary>
    /// RobotComponents Controller Utility : Upload program component. An inherent from the GH_Component Class.
    /// </summary>
    public class UploadProgramComponent : GH_Component
    {
        #region fields
        private Controller _controller;
        private bool _fromMenu = false;
        private string _taskName = "-";
        #endregion

        /// <summary>
        /// Initializes a new instance of the UploadProgram class.
        /// </summary>
        public UploadProgramComponent()
          : base("Upload Program", "UP",
              "Uploads RAPID modules directly to a real or virtual robot controller."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.ABB.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Controller Utility 2.0-beta")
        {
            this.Message = _taskName;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Controller(), "Controller", "C", "Controller as Controller", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Upload", "U", "Upload as bool", GH_ParamAccess.item, false);
            pManager.AddTextParameter("Module", "M", "Module as a list with code lines", GH_ParamAccess.list);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "S", "Controller status", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare input variables
            bool upload = false;
            List<string> module = new List<string>();

            // Catch the input data
            if (!DA.GetData(0, ref _controller)) { return; }
            if (!DA.GetData(1, ref upload)) { upload = false; }
            if (!DA.GetDataList(2, module)) { module = new List<string>(); }

            // Declare output variables
            string status = "-";

            if (_fromMenu)
            {
                this.GetTaskName();
                this.Message = _taskName;
                this.ExpirePreview(true);
                status = "Task picked from controller.";
            }

            if (upload)
            {
                if (module.Count != 0 & _taskName != "-")
                {
                    _controller.UploadModule(_taskName, module, out status);
                }
                else if (module.Count == 0)
                {
                    status = "No module defined.";
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, status);
                }
                else if (_taskName == "-")
                {
                    status = "No task defined.";
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, status);
                }
            }

            // Output
            DA.SetDataList(0, status);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
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
            get { return Properties.Resources.Upload_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("30FF28E6-B93E-4AAD-B821-F6FC77C5DADF"); }
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
            Menu_AppendItem(menu, "Pick Task", MenuItemClick);
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
            writer.SetString("Task Name", _taskName);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            reader.GetString("Task Name");
            return base.Read(reader);
        }
        #endregion

        #region pick task
        /// <summary>
        /// Get the task name
        /// </summary>
        /// <returns> Indicates whether or not a task was picked successfully. </returns>
        private bool GetTaskName()
        {
            if (_controller.Tasks.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No task found!");
                return false;
            }

            else if (_controller.Tasks.Count == 1)
            {
                _taskName = _controller.Tasks[0].Name;
                return true;
            }

            else if (_controller.Tasks.Count > 1)
            {
                PickTaskForm frm = new PickTaskForm(_controller);
                Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnEditor(frm, false);
                frm.ShowDialog();
                int index = frm.Index;

                if (index < 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No task picked from the menu!");
                    return false;
                }
                else
                {
                    _taskName = _controller.Tasks[index].Name;
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}