// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.Kinematics;
using RobotComponents.Definitions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Simulation
{
    /// <summary>
    /// RobotComponents Path Generator component. An inherent from the GH_Component Class.
    /// </summary>
    public class PathGeneratorComponent : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public PathGeneratorComponent()
          : base("Path Generator", "PG",
              "EXPERIMENTAL: Generates and displays an approximation of the movement path for a defined ABB robot based on a list of Actions."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Simulation")
        {
            // Create the component label with a message
            Message = "EXTENDABLE";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new RobotParameter(), "Robot", "R", "Robot as Robot", GH_ParamAccess.item);
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
            AddParameter(8);
        }

        // Create an array with the variable input parameters
        readonly IGH_Param[] outputParameters = new IGH_Param[9]
        {
            new Param_Plane() { Name = "Robot End Plane", NickName = "EP", Description = "The current position and orientation of tool TCP", Access = GH_ParamAccess.item},
            new Param_Plane() { Name = "Robot End Planes", NickName = "EPs", Description = "The positions and orientations of the tool TCP of the whole path", Access = GH_ParamAccess.list},
            new RobotJointPositionParameter() { Name = "Robot Joint Position", NickName = "RJ", Description = "The current Robot Joint Position", Access = GH_ParamAccess.item},
            new RobotJointPositionParameter() { Name = "Robot Joint Positions", NickName = "RJs", Description = "The Robot Joint Positions of the whole path", Access = GH_ParamAccess.list},
            new Param_Plane() { Name = "External Axis Planes", NickName = "EAP", Description = "The current position and orientation of the external axes", Access = GH_ParamAccess.list},
            new ExternalJointPositionParameter() { Name = "External Joint Position", NickName = "EJ", Description = "The current External Joint Position", Access = GH_ParamAccess.item},
            new ExternalJointPositionParameter() { Name = "External Joint Positions", NickName = "EJs", Description = "The External Joint Positions of the whole path", Access = GH_ParamAccess.list},
            new Param_String() { Name = "Error messages", NickName = "E", Description = "The error messages collected during the generation of the path", Access = GH_ParamAccess.list},
            new Param_Curve() { Name = "Path", NickName = "P", Description = "The whole tool path as list with curves", Access = GH_ParamAccess.list},
        };

        // Fields
        private Robot _robot;
        private PathGenerator _pathGenerator = new PathGenerator();
        private ForwardKinematics _forwardKinematics = new ForwardKinematics();
        private bool _outputRobotEndPlane = false;
        private bool _outputRobotEndPlanes = false;
        private bool _outputRobotJointPosition = false;
        private bool _outputRobotJointPositions = false;
        private bool _outputExternalAxisPlanes = false;
        private bool _outputExternalJointPosition = false;
        private bool _outputExternalJointPositions = false;
        private bool _outputErrorMessages = false;
        private bool _previewMesh = true;
        private bool _calculated = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
            if (update == true | _calculated == false)
            {
                // Create forward kinematics for mesh display
                _forwardKinematics = new ForwardKinematics(_robot);

                // Create the path generator
                _pathGenerator = new PathGenerator(_robot);

                // Re-calculate the path
                _pathGenerator.Calculate(actions, interpolations);

                // Makes sure that there is always a calculated solution
                _calculated = true;
            }

            // Get the index number of the current target
            int index = (int)(((_pathGenerator.Planes.Count - 1) * interpolationSlider));

            // Calculate foward kinematics
            _forwardKinematics.HideMesh = !_previewMesh;
            _forwardKinematics.Calculate(_pathGenerator.RobotJointPositions[index], _pathGenerator.ExternalJointPositions[index]);

            // Show error messages
            if (_pathGenerator.ErrorText.Count != 0)
            {
                for (int i = 0; i < _pathGenerator.ErrorText.Count; i++)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, _pathGenerator.ErrorText[i]);
                    if (i == 30) { break; }
                }
            }

            // Output Parameters
            int ind;
            if (Params.Output.Any(x => x.NickName.Equality(outputParameters[0].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(outputParameters[0].NickName));
                DA.SetData(ind, _pathGenerator.Planes[index]);
            }
            if (Params.Output.Any(x => x.NickName.Equality(outputParameters[1].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(outputParameters[1].NickName));
                DA.SetDataList(ind, _pathGenerator.Planes);
            }
            if (Params.Output.Any(x => x.NickName.Equality(outputParameters[2].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(outputParameters[2].NickName));
                DA.SetData(ind, _pathGenerator.RobotJointPositions[index]);
            }
            if (Params.Output.Any(x => x.NickName.Equality(outputParameters[3].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(outputParameters[3].NickName));
                DA.SetDataList(ind, _pathGenerator.RobotJointPositions);
            }
            if (Params.Output.Any(x => x.NickName.Equality(outputParameters[4].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(outputParameters[4].NickName));
                DA.SetDataList(ind, _forwardKinematics.PosedExternalAxisPlanes);
            }
            if (Params.Output.Any(x => x.NickName.Equality(outputParameters[5].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(outputParameters[5].NickName));
                DA.SetData(ind, _pathGenerator.ExternalJointPositions[index]);
            }
            if (Params.Output.Any(x => x.NickName.Equality(outputParameters[6].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(outputParameters[6].NickName));
                DA.SetDataList(ind, _pathGenerator.ExternalJointPositions);
            }
            if (Params.Output.Any(x => x.NickName.Equality(outputParameters[7].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(outputParameters[7].NickName));
                DA.SetDataList(ind, _pathGenerator.ErrorText);
            }
            DA.SetDataList(outputParameters[8].Name, _pathGenerator.Paths);
        }

        #region menu item
        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Set Preview Mesh", _previewMesh);
            writer.SetBoolean("Output Robot End Plane", _outputRobotEndPlane);
            writer.SetBoolean("Output Robot End Planes", _outputRobotEndPlanes);
            writer.SetBoolean("Output Robot Joint Position", _outputRobotJointPosition);
            writer.SetBoolean("Output Robot Joint Positions", _outputRobotJointPositions);
            writer.SetBoolean("Output External Axis Planes", _outputExternalAxisPlanes);
            writer.SetBoolean("Output External Joint Position", _outputExternalJointPosition);
            writer.SetBoolean("Output External Joint Positions", _outputExternalJointPositions);
            writer.SetBoolean("Output Error Messages", _outputErrorMessages);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            _previewMesh = reader.GetBoolean("Set Preview Mesh");
            _outputRobotEndPlane = reader.GetBoolean("Output Robot End Plane");
            _outputRobotEndPlanes = reader.GetBoolean("Output Robot End Planes");
            _outputRobotJointPosition = reader.GetBoolean("Output Robot Joint Position");
            _outputRobotJointPositions = reader.GetBoolean("Output Robot Joint Positions");
            _outputExternalAxisPlanes = reader.GetBoolean("Output External Axis Planes");
            _outputExternalJointPosition = reader.GetBoolean("Output External Joint Position");
            _outputExternalJointPositions = reader.GetBoolean("Output External Joint Positions");
            _outputErrorMessages = reader.GetBoolean("Output Error Messages");
            return base.Read(reader);
        }

        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Preview Mesh", MenuItemClickPreviewMesh, true, _previewMesh);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Output current Robot End Plane", MenuItemClickOutputRobotEndPlane, true, _outputRobotEndPlane);
            Menu_AppendItem(menu, "Output all Robot End Planes", MenuItemClickOutputRobotEndPlanes, true, _outputRobotEndPlanes);
            Menu_AppendItem(menu, "Output current Robot Joint Position", MenuItemClickOutputRobotJointPosition, true, _outputRobotJointPosition);
            Menu_AppendItem(menu, "Output all Robot Joint Positions", MenuItemClickOutputRobotJointPositions, true, _outputRobotJointPositions);
            Menu_AppendItem(menu, "Output current External Axis Planes", MenuItemClickOutputExternalAxisPlanes, true, _outputExternalAxisPlanes);
            Menu_AppendItem(menu, "Output current External Joint Position", MenuItemClickOutputExternalJointPosition, true, _outputExternalJointPosition);
            Menu_AppendItem(menu, "Output all External Joint Positions", MenuItemClickOutputExternalJointPositions, true, _outputExternalJointPositions);
            Menu_AppendItem(menu, "Output all Error Messages", MenuItemClickOutputErrorMessages, true, _outputErrorMessages);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Documentation", MenuItemClickComponentDoc, Properties.Resources.WikiPage_MenuItem_Icon);
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

        /// <summary>
        /// Handles the event when the custom menu item "Output Robot End Plane" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickOutputRobotEndPlane(object sender, EventArgs e)
        {
            RecordUndoEvent("Output current Robot End Plane");
            _outputRobotEndPlane = !_outputRobotEndPlane;
            AddParameter(0);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Output Robot End Planes" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickOutputRobotEndPlanes(object sender, EventArgs e)
        {
            RecordUndoEvent("Output all Robot End Planes");
            _outputRobotEndPlanes = !_outputRobotEndPlanes;
            AddParameter(1);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Output Robot Joint Position" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickOutputRobotJointPosition(object sender, EventArgs e)
        {
            RecordUndoEvent("Output current Robot Joint Position");
            _outputRobotJointPosition = !_outputRobotJointPosition;
            AddParameter(2);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Output Robot Joint Positions" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickOutputRobotJointPositions(object sender, EventArgs e)
        {
            RecordUndoEvent("Output all Robot Joint Positions");
            _outputRobotJointPositions = !_outputRobotJointPositions;
            AddParameter(3);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Output External Axis Planes" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickOutputExternalAxisPlanes(object sender, EventArgs e)
        {
            RecordUndoEvent("Output current External Axis Planes");
            _outputExternalAxisPlanes = !_outputExternalAxisPlanes;
            AddParameter(4);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Output External Joint Position" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickOutputExternalJointPosition(object sender, EventArgs e)
        {
            RecordUndoEvent("Output current External Joint Position");
            _outputExternalJointPosition = !_outputExternalJointPosition;
            AddParameter(5);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Output External Joint Positions" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickOutputExternalJointPositions(object sender, EventArgs e)
        {
            RecordUndoEvent("Output all External Joint Positions");
            _outputExternalJointPositions = !_outputExternalJointPositions;
            AddParameter(6);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Output Error Messages" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickOutputErrorMessages(object sender, EventArgs e)
        {
            RecordUndoEvent("Output all Error Messages");
            _outputErrorMessages = !_outputErrorMessages;
            AddParameter(7);
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
        /// Adds or destroys the output parameter to the component.
        /// </summary>
        /// <param name="index"> The index number of the parameter that needs to be added. </param>
        private void AddParameter(int index)
        {
            // Pick the parameter
            IGH_Param parameter = outputParameters[index];
            string name = outputParameters[index].NickName;

            // If the parameter already exist: remove it
            if (Params.Output.Any(x => x.NickName.Equality(parameter.NickName)))
            {
                Params.UnregisterOutputParameter(Params.Output.First(x => x.NickName.Equality(parameter.NickName)), true);
            }

            // Else remove the variable input parameter
            else
            {
                // The index where the parameter should be added
                int insertIndex = 0;

                // Check if other parameters are already added and correct the insert index
                for (int i = 0; i < index; i++)
                {
                    if (Params.Output.Any(x => x.NickName.Equality(outputParameters[i].NickName)))
                    {
                        insertIndex += 1;
                    }
                }

                // Register the input parameter
                Params.RegisterOutputParam(parameter, insertIndex);
            }

            // Expire solution and refresh parameters since they changed
            Params.OnParametersChanged();
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

        // Methods of variable parameter interface which handles (de)serialization of the variable input parameters
        #region variable input parameters
        /// <summary>
        /// This function will get called before an attempt is made to insert a parameter. 
        /// Since this method is potentially called on Canvas redraws, it must be fast.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if your component supports a variable parameter at the given location </returns>
        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This function will get called before an attempt is made to insert a parameter. 
        /// Since this method is potentially called on Canvas redraws, it must be fast.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if your component supports a variable parameter at the given location. </returns>
        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This function will be called when a new parameter is about to be inserted. 
        /// You must provide a valid parameter or insertion will be skipped. 
        /// You do not, repeat not, need to insert the parameter yourself.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> A valid IGH_Param instance to be inserted. In our case a null value. </returns>
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {
            return null;
        }

        /// <summary>
        /// This function will be called when a parameter is about to be removed. 
        /// You do not need to do anything, but this would be a good time to remove 
        /// any event handlers that might be attached to the parameter in question.
        /// </summary>
        /// <param name="side"> Parameter side (input or output). </param>
        /// <param name="index"> Insertion index of parameter. Index=0 means the parameter will be in the topmost spot. </param>
        /// <returns> Return True if the parameter in question can indeed be removed. Note, this is only in emergencies, 
        /// typically the CanRemoveParameter function should return false if the parameter in question is not removable. </returns>
        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This method will be called when a closely related set of variable parameter operations completes. 
        /// This would be a good time to ensure all Nicknames and parameter properties are correct. 
        /// This method will also be called upon IO operations such as Open, Paste, Undo and Redo.
        /// </summary>
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {

        }
        #endregion

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
            get { return new Guid("3274D235-082A-445A-BA77-75CD3A7926E0"); }
        }
    }
}
