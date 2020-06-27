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
using RobotComponents.BaseClasses.Actions;
using RobotComponentsABB.Parameters.Actions;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Joint Target component. An inherent from the GH_Component Class.
    /// </summary>
    public class JointTargetComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public JointTargetComponent()
          : base("Joint Target", "JT",
              "Defines a Joint Target for an Instruction : Movement."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
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
            pManager.AddTextParameter("Name", "N", "Name as text", GH_ParamAccess.list, "defaultTar");
            pManager.AddParameter(new RobotJointPositionParameter(), "Robot Joint Position", "RJ", "Defines the robot joint position", GH_ParamAccess.list);
            pManager.AddParameter(new ExternalJointPositionParameter(), "External Joint Position", "EJ", "Defines the external axis joint position", GH_ParamAccess.list);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new JointTargetParameter(), "Joint Target", "JT", "The resulting Joint Target");
        }

        // Fields
        private readonly List<JointTarget> _jointTargets = new List<JointTarget>();

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Variables
            List<string> names = new List<string>();
            List<RobotJointPosition> robotJointPositions = new List<RobotJointPosition>();
            List<ExternalJointPosition> externalJointPositions = new List<ExternalJointPosition>();

            // Catch input data
            if (!DA.GetDataList(0, names)) { return; }
            if (!DA.GetDataList(1, robotJointPositions)) { robotJointPositions = new List<RobotJointPosition>() { new RobotJointPosition() }; }
            if (!DA.GetDataList(2, externalJointPositions)) { externalJointPositions = new List<ExternalJointPosition>() { new ExternalJointPosition() }; }

            // Get longest input List
            int[] sizeValues = new int[3];
            sizeValues[0] = names.Count;
            sizeValues[1] = robotJointPositions.Count;
            sizeValues[2] = externalJointPositions.Count;

            int biggestSize = HelperMethods.GetBiggestValue(sizeValues);

            // Keeps track of used indicies
            int nameCounter = -1;
            int robPosCounter = -1;
            int extPosCounter = -1;

            // Clear list
            _jointTargets.Clear();

            // Creates the joint targets
            for (int i = 0; i < biggestSize; i++)
            {
                string name;
                RobotJointPosition robotJointPosition;
                ExternalJointPosition externalJointPosition;

                // Names counter
                if (i < sizeValues[0])
                {
                    name = names[i];
                    nameCounter++;
                }
                else
                {
                    name = names[nameCounter] + "_" + (i - nameCounter);
                }

                // Robot Joint Position counter
                if (i < sizeValues[1])
                {
                    robotJointPosition = robotJointPositions[i];
                    robPosCounter++;
                }
                else
                {
                    robotJointPosition = robotJointPositions[robPosCounter];
                }

                // External Joint Position counter
                if (i < sizeValues[2])
                {
                    externalJointPosition = externalJointPositions[i];
                    extPosCounter++;
                }
                else
                {
                    externalJointPosition = externalJointPositions[extPosCounter];
                }

                JointTarget jointTarget = new JointTarget(name, robotJointPosition, externalJointPosition);
                _jointTargets.Add(jointTarget);
            }

            // Sets Output
            DA.SetDataList(0, _jointTargets);
        }

        // Methods for creating custom menu items and event handlers when the custom menu items are clicked
        #region menu items
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
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
        #endregion

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return null; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6C77792A-AC5B-4199-8CD2-A36B79D5AA87"); }
        }

    }
}
