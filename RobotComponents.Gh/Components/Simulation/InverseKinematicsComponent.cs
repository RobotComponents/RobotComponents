// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Drawing;
using System.Windows.Forms;
//Grasshopper Libs
using Grasshopper.Kernel;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Definitions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Kinematics;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Simulation
{
    /// <summary>
    /// RobotComponents Inveser Kinematics component. An inherent from the GH_Component Class.
    /// </summary>
    public class InverseKinematicsComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public InverseKinematicsComponent()
          : base("Inverse Kinematics", "IK",
              "Computes the axis values for a defined ABB robot based on an Action: Target."
      + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Simulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new RobotParameter(), "Robot", "R", "Robot as Robot", GH_ParamAccess.item);
            pManager.AddParameter(new MovementParameter(), "Movement", "M", "Movement or target input. A target will automatically be casted to a movement.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new RobotJointPositionParameter(), "Robot Joint Position", "RJ", "The calculated Robot Joint Position");
            pManager.RegisterParam(new ExternalJointPositionParameter(), "External Joint Position", "EJ", "The calculated External Joint Position");
        }

        // Fields
        private InverseKinematics _inverseKinematics = new InverseKinematics();
        private ForwardKinematics _forwardKinematics = new ForwardKinematics();
        private bool _hideMesh = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            Robot robot = null;
            Movement Movement = null;

            // Catch the input data
            if (!DA.GetData(0, ref robot)) { return; }
            if (!DA.GetData(1, ref Movement)) { return; }

            // Calculate the robot pose
            _inverseKinematics = new InverseKinematics(Movement, robot);
            _inverseKinematics.Calculate();

            // Check the values
            for (int i = 0; i < _inverseKinematics.ErrorText.Count; i++)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, _inverseKinematics.ErrorText[i]);
            }

            // Set forward kinematics (for visualization)
            _forwardKinematics = new ForwardKinematics(robot);
            
            // Output
            DA.SetData(0, _inverseKinematics.RobotJointPosition);
            DA.SetData(1, _inverseKinematics.ExternalJointPosition);
        }

        #region menu item
        /// <summary>
        /// Boolean that indicates if the custom menu item for hding the robot mesh is checked
        /// </summary>
        public bool SetHideMesh
        {
            get { return _hideMesh; }
            set { _hideMesh = value; }
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Set Hide Mesh", SetHideMesh);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            SetHideMesh = reader.GetBoolean("Set Hide Mesh");
            return base.Read(reader);
        }

        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Preview Mesh", MenuItemClickHideMesh, true, !SetHideMesh);
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
        /// Handles the event when the custom menu item "Hide Mesh" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickHideMesh(object sender, EventArgs e)
        {
            RecordUndoEvent("Set Hide Mesh");
            _hideMesh = !_hideMesh;
            ExpireSolution(true);
        }
        #endregion

        /// <summary>
        /// This method displays the robot pose for the given axis values. 
        /// </summary>
        /// <param name="args"> Preview display arguments for IGH_PreviewObjects. </param>
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            // Check if there is a mesh available to display and the onlyTCP function not active
            if (!_hideMesh)
            {
                // Initiate the display color and transparancy of the robot mesh
                Color color;
                double trans;

                // Calculate forward kinematics
                _forwardKinematics.HideMesh = _hideMesh;
                _forwardKinematics.Calculate(_inverseKinematics.RobotJointPosition, _inverseKinematics.ExternalJointPosition);

                // Set the display color and transparancy of the robot mesh
                if (_forwardKinematics.InLimits == true)
                {
                    color = Color.FromArgb(225, 225, 225);
                    trans = 0.0;
                }
                else
                {
                    color = Color.FromArgb(150, 0, 0);
                    trans = 0.5;
                }

                // Display the internal axes of the robot
                for (int i = 0; i != _forwardKinematics.PosedInternalAxisMeshes.Count; i++)
                {
                    args.Display.DrawMeshShaded(_forwardKinematics.PosedInternalAxisMeshes[i], new Rhino.Display.DisplayMaterial(color, trans));
                }

                // Display the external axes
                for (int i = 0; i != _forwardKinematics.PosedExternalAxisMeshes.Count; i++)
                {
                    for (int j = 0; j != _forwardKinematics.PosedExternalAxisMeshes[i].Count; j++)
                    {
                        args.Display.DrawMeshShaded(_forwardKinematics.PosedExternalAxisMeshes[i][j], new Rhino.Display.DisplayMaterial(color, trans));
                    }
                }
            }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.InverseKinematics_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("EAC8F3EF-CA07-49A5-8E90-64BA6D9BDE2E"); }
        }
    }

}