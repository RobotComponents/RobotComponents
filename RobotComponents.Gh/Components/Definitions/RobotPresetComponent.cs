// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Enumerations;
using RobotComponents.Definitions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Utils;
using RobotComponents.Gh.Forms;

namespace RobotComponents.Gh.Components.Definitions
{
    /// <summary>
    /// RobotComponents Robot Preset component. An inherent from the GH_Component Class.
    /// </summary>
    public class RobotPresetComponent : GH_Component
    {
        public RobotPresetComponent()
          : base("Robot Preset", "RobPres",
              "Defines a robot which is needed for Code Generation and Simulation"
             + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Definitions")
        {
            this.Message = "-";
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
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
            pManager.AddPlaneParameter("Position Plane", "PP", "Position Plane of the Robot as Plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddParameter(new RobotToolParameter(), "Robot Tool", "RT", "Robot Tool as Robot Tool Parameter", GH_ParamAccess.item);
            pManager.AddParameter(new ExternalAxisParameter(), "External Axis", "EA", "External Axis as External Axis Parameter", GH_ParamAccess.list);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new RobotParameter(), "Robot", "R", "Resulting Robot", GH_ParamAccess.item);
        }

        // fields
        private RobotPreset _robotPreset = RobotPreset.EMPTY;
        private bool _fromMenu = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane positionPlane = Plane.WorldXY;
            RobotTool tool = null;
            List<ExternalAxis> externalAxis = new List<ExternalAxis>();

            if (!DA.GetData(0, ref positionPlane)) { return; }
            if (!DA.GetData(1, ref tool)) { tool = new RobotTool(); }
            if (!DA.GetDataList(2, externalAxis)) { externalAxis = new List<ExternalAxis>() { }; }

            Robot robot = new Robot();

            if (_robotPreset == RobotPreset.EMPTY | _fromMenu)
            {
                _robotPreset = GetRobotPreset();
            }

            try
            {
                robot = Robot.GetRobotPreset(_robotPreset, positionPlane, tool, externalAxis);

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

        #region menu item
        /// <summary>
        /// Gets the Robot preset
        /// </summary>
        /// <returns> The picked Robot preset. </returns>
        private RobotPreset GetRobotPreset()
        {
            // Create the form with all the available robot presets
            List<RobotPreset> robotPresets = Enum.GetValues(typeof(RobotPreset)).Cast<RobotPreset>().ToList();
            robotPresets.Remove(RobotPreset.EMPTY);
            PickRobotForm frm = new PickRobotForm(robotPresets);

            // Display the form
            Grasshopper.GUI.GH_WindowsFormUtil.CenterFormOnEditor(frm, false);
            frm.ShowDialog();

            // Return the index number of the picked controller
            int index = frm.RobotIndex;

            // Return a null value when the picked index is incorrect. 
            if (index < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Robot picked from menu!");
                return RobotPreset.EMPTY;
            }

            // Select the picked robot
            return robotPresets[index];
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            byte[] array = RobotComponents.Utils.HelperMethods.ObjectToByteArray(_robotPreset);
            writer.SetByteArray("Robot Preset", array);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            byte[] array = reader.GetByteArray("Robot Preset");
            _robotPreset = (RobotPreset)RobotComponents.Utils.HelperMethods.ByteArrayToObject(array);
            return base.Read(reader);
        }

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
        #endregion

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
            get { return new Guid("CF1004D8-9598-4B2B-8F8A-5A3E43614579"); }
        }
    }
}