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
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Utils;
using RobotComponents.ABB.Presets;
using RobotComponents.ABB.Presets.Forms;
using RobotComponents.ABB.Presets.Enumerations;

namespace RobotComponents.ABB.Gh.Components.Definitions
{
    /// <summary>
    /// RobotComponents Robot Preset component. An inherent from the GH_Component Class.
    /// </summary>
    public class RobotPresetComponent : GH_Component
    {
        #region fields
        private RobotPreset _robotPreset = RobotPreset.EMPTY;
        private bool _fromMenu = false;
        #endregion

        public RobotPresetComponent()
          : base("Robot Preset", "RobPres",
              "Defines a robot which is needed for code generation and simulation"
             + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Definitions")
        {
            this.Message = "-";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Position Plane", "PP", "Position Plane of the Robot as Plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddParameter(new Param_RobotTool(), "Robot Tool", "RT", "Robot Tool as Robot Tool Parameter", GH_ParamAccess.item);
            pManager.AddParameter(new Param_ExternalAxis(), "External Axis", "EA", "External Axis as External Axis Parameter", GH_ParamAccess.list);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Robot(), "Robot", "R", "Resulting Robot", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane positionPlane = Plane.WorldXY;
            RobotTool tool = null;
            List<IExternalAxis> externalAxis = new List<IExternalAxis>();

            if (!DA.GetData(0, ref positionPlane)) { return; }
            if (!DA.GetData(1, ref tool)) { tool = new RobotTool(); }
            if (!DA.GetDataList(2, externalAxis)) { externalAxis = new List<IExternalAxis>() { }; }

            Robot robot = new Robot();

            if (_robotPreset == RobotPreset.EMPTY | _fromMenu)
            {
                _robotPreset = GetRobotPreset();
            }

            try
            {
                robot = Factory.GetRobotPreset(_robotPreset, positionPlane, tool, externalAxis);

                int position = robot.Name.IndexOf("-");

                if (position < 0)
                {
                    this.Message = robot.Name;
                }
                else
                {
                    this.Message = robot.Name.Substring(0, position);
                }

                this.ExpirePreview(true);
            }
            catch (Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, ex.Message);
            }

            DA.SetData(0, robot);
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
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
            get { return Properties.Resources.RobotPreset_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("33D7467A-17B3-43DB-B26E-2AF153AA8491"); }
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
            Menu_AppendItem(menu, "Pick Robot Preset", MenuItemClick);
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

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("Robot Preset Number", (int)_robotPreset);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            int index = reader.GetInt32("Robot Preset Number");
            _robotPreset = (RobotPreset)index;

            return base.Read(reader);
        }
        #endregion

        #region additional methods
        /// <summary>
        /// Gets the Robot preset
        /// </summary>
        /// <returns> The picked Robot preset. </returns>
        private RobotPreset GetRobotPreset()
        {
            RobotPreset robotPreset;

            PickRobotForm form = new PickRobotForm();
            bool result = form.ShowModal(Grasshopper.Instances.EtoDocumentEditor);

            if (result)
            {
                robotPreset = form.RobotPreset;
            }
            else
            {
                robotPreset = _robotPreset;
            }

            if (robotPreset == RobotPreset.EMPTY)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Robot picked from menu!");
            }

            return robotPreset;
        }
        #endregion
    }
}