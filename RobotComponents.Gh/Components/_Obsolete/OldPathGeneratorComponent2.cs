// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
// Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Kinematics;
using RobotComponents.Definitions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Parameters.Actions;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.10.000
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Path Generator component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldPathGeneratorComponent2 : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldPathGeneratorComponent2()
          : base("Path Generator", "PG",
              "EXPERIMENTAL: Generates and displays an approximation of the movement path for a defined ABB robot based on a list of Actions."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Simulation")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
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
            pManager.AddParameter(new RobotParameter(), "Robot", "RI", "Robot as Robot", GH_ParamAccess.item);
            pManager.AddParameter(new ActionParameter(), "Actions", "A", "Actions as a list with Actions", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Interpolations", "I", "Interpolations as Int", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Animation Slider", "AS", "Animation Slider as number (0.0 - 1.0)", GH_ParamAccess.item, 0.0);
            pManager.AddBooleanParameter("Update", "U", "If set to true, path will be constantly recalculated.", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_PlaneParam("Plane", "EP", "Current End Plane as a Plane");
            pManager.Register_PlaneParam("External Axis Planes", "EAP", "Current Exernal Axis Planes as a list with Planes");
            pManager.Register_DoubleParam("Internal Axis Values", "IAV", "Current Internal Axis Values as a list with numbers");
            pManager.Register_DoubleParam("External Axis Values", "EAV", "Current External Axis Values as a list with numbers");
            pManager.Register_CurveParam("Tool Path", "TP", "Tool Path as a list with Curves");
        }

        // Fields
        private Robot _robot;
        private PathGenerator _pathGenerator = new PathGenerator();
        private ForwardKinematics _forwardKinematics = new ForwardKinematics();
        private List<Plane> _planes = new List<Plane>();
        private List<Curve> _paths = new List<Curve>();
        private List<RobotJointPosition> _robotJointPositions = new List<RobotJointPosition>();
        private List<ExternalJointPosition> _externalJointPositions = new List<ExternalJointPosition>();
        private int _lastInterpolations = 0;
        private bool _raiseWarnings = false;
        private bool _previewMesh = true;
        private bool _previewCurve = true;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Input variables
            List<RobotComponents.Actions.Action> actions = new List<RobotComponents.Actions.Action>();
            int interpolations = 0;
            double interpolationSlider = 0;
            bool update = false;

            // Catch the input data
            if (!DA.GetData(0, ref _robot)) { return; }
            if (!DA.GetDataList(1, actions)) { return; }
            if (!DA.GetData(2, ref interpolations)) { return; }
            if (!DA.GetData(3, ref interpolationSlider)) { return; }
            if (!DA.GetData(4, ref update)) { return; }

            // Update the path
            if (update == true || _lastInterpolations != interpolations)
            {
                // Create forward kinematics for mesh display
                _forwardKinematics = new ForwardKinematics(_robot);

                // Create the path generator
                _pathGenerator = new PathGenerator(_robot);

                // Re-calculate the path
                _pathGenerator.Calculate(actions, interpolations);

                // Get all the targets
                _planes.Clear();
                _planes = _pathGenerator.Planes;

                // Get the new path curve
                _paths.Clear();
                _paths = _pathGenerator.Paths;

                // Clear the lists with the internal and external axis values
                _robotJointPositions.Clear();
                _externalJointPositions.Clear();
                _robotJointPositions = _pathGenerator.RobotJointPositions;
                _externalJointPositions = _pathGenerator.ExternalJointPositions;

                // Store the number of interpolations that are used, to check if this value is changed. 
                _lastInterpolations = interpolations;

                // Raise warnings
                if (_pathGenerator.ErrorText.Count != 0)
                {
                    _raiseWarnings = true;
                }
                else
                {
                    _raiseWarnings = false;
                }
            }

            // Get the index number of the current target
            int index = (int)(((_planes.Count - 1) * interpolationSlider));

            // Create list with external axis values
            List<double> externalAxisValues = _externalJointPositions[index].ToList();
            externalAxisValues.RemoveAll(val => val == 9e9);

            // Calcualte foward kinematics
            _forwardKinematics.RobotJointPosition = _robotJointPositions[index];
            _forwardKinematics.ExternalJointPosition = _externalJointPositions[index];
            _forwardKinematics.HideMesh = !_previewMesh;
            _forwardKinematics.Calculate();

            // Show error messages
            if (_raiseWarnings == true)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Only axis values of absolute joint movements are checked.");

                for (int i = 0; i < _pathGenerator.ErrorText.Count; i++)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, _pathGenerator.ErrorText[i]);
                    if (i == 30) { break; }
                }
            }

            // Output
            DA.SetData(0, _forwardKinematics.TCPPlane);
            DA.SetDataList(1, _forwardKinematics.PosedExternalAxisPlanes);
            DA.SetDataList(2, _forwardKinematics.RobotJointPosition.ToList());
            DA.SetDataList(3, externalAxisValues);
            if (_previewCurve == true) { DA.SetDataList(4, _paths); }
            else { DA.SetDataList(4, null); }
        }

        #region menu item
        /// <summary>
        /// Boolean that indicates if the custom menu item for hiding the robot mesh is checked
        /// </summary>
        public bool SetPreviewCurve
        {
            get { return _previewCurve; }
            set { _previewCurve = value; }
        }

        /// <summary>
        /// Boolean that indicates if the custom menu item for hiding the robot mesh is checked
        /// </summary>
        public bool SetPreviewMesh
        {
            get { return _previewMesh; }
            set { _previewMesh = value; }
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Set Preview Curve", SetPreviewCurve);
            writer.SetBoolean("Set Preview Mesh", SetPreviewMesh);
            return base.Write(writer);
        }


        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            SetPreviewCurve = reader.GetBoolean("Set Preview Curve");
            SetPreviewMesh = reader.GetBoolean("Set Preview Mesh");
            return base.Read(reader);
        }

        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Preview Curve", MenuItemClickPreviewCurve, true, SetPreviewCurve);
            Menu_AppendItem(menu, "Preview Mesh", MenuItemClickPreviewMesh, true, SetPreviewMesh);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Preview Curve" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickPreviewCurve(object sender, EventArgs e)
        {
            RecordUndoEvent("Set Preview Curve");
            _previewCurve = !_previewCurve;
            ExpireSolution(true);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Preview Mesh" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickPreviewMesh(object sender, EventArgs e)
        {
            RecordUndoEvent("Set Preview Mesh");
            _previewMesh = !_previewMesh;
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
            if (_forwardKinematics.PosedInternalAxisMeshes != null && _previewMesh)
            {
                // Initiate the display color and transparancy of the robot mesh
                Color color;
                double trans;

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
            get { return Properties.Resources.PathGen_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1B3D7921-4B70-44F4-B5E9-0C8659CFAF2B"); }
        }
    }
}
