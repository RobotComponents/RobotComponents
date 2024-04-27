﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
// Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Kinematics;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Parameters.Actions;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Instructions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Path Generator component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class PathGeneratorComponent_OBSOLETE2 : GH_Component, IGH_VariableParameterComponent
    {
        #region fields
        private readonly List<PathGenerator> _pathGenerators = new List<PathGenerator>();
        private readonly List<ForwardKinematics> _forwardKinematics = new List<ForwardKinematics>();
        private readonly List<bool> _calculated = new List<bool>();
        private bool _outputMovement = false;
        private bool _outputMovements = false;
        private bool _outputRobotEndPlane = false;
        private bool _outputRobotEndPlanes = false;
        private bool _outputRobotJointPosition = false;
        private bool _outputRobotJointPositions = false;
        private bool _outputExternalAxisPlanes = false;
        private bool _outputExternalJointPosition = false;
        private bool _outputExternalJointPositions = false;
        private bool _outputErrorMessages = false;
        private bool _previewMesh = true;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public PathGeneratorComponent_OBSOLETE2()
          : base("Path Generator", "PG",
              "Generates and displays an approximation of the movement path for a defined ABB robot based on a list of Actions."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Simulation")
        {
            // Create the component label with a message
            Message = "EXTENDABLE";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Robot(), "Robot", "R", "Robot as Robot", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Action(), "Actions", "A", "Actions as a list with Actions", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Interpolations", "I", "Interpolations as Int", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Animation Slider", "AS", "Animation Slider as number (0.0 - 1.0)", GH_ParamAccess.item, 0.0);
            pManager.AddBooleanParameter("Update", "U", "If set to true, path will be constantly recalculated.", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Stores the variable output parameters in an array.
        /// </summary>
        private readonly IGH_Param[] _variableOutputParameters = new IGH_Param[11]
        {
            new Param_Movement() { Name = "Movement", NickName = "M", Description = "The current move object.", Access = GH_ParamAccess.item},
            new Param_Movement() { Name = "Movements", NickName = "Ms", Description = "The move objects of the whole path.", Access = GH_ParamAccess.list},
            new Param_Plane() { Name = "Robot End Plane", NickName = "EP", Description = "The current position and orientation of tool TCP", Access = GH_ParamAccess.item},
            new Param_Plane() { Name = "Robot End Planes", NickName = "EPs", Description = "The positions and orientations of the tool TCP of the whole path", Access = GH_ParamAccess.list},
            new Param_RobotJointPosition() { Name = "Robot Joint Position", NickName = "RJ", Description = "The current Robot Joint Position", Access = GH_ParamAccess.item},
            new Param_RobotJointPosition() { Name = "Robot Joint Positions", NickName = "RJs", Description = "The Robot Joint Positions of the whole path", Access = GH_ParamAccess.list},
            new Param_Plane() { Name = "External Axis Planes", NickName = "EAP", Description = "The current position and orientation of the external axes", Access = GH_ParamAccess.list},
            new Param_ExternalJointPosition() { Name = "External Joint Position", NickName = "EJ", Description = "The current External Joint Position", Access = GH_ParamAccess.item},
            new Param_ExternalJointPosition() { Name = "External Joint Positions", NickName = "EJs", Description = "The External Joint Positions of the whole path", Access = GH_ParamAccess.list},
            new Param_String() { Name = "Error messages", NickName = "E", Description = "The error messages collected during the generation of the path", Access = GH_ParamAccess.list},
            new Param_Curve() { Name = "Path", NickName = "P", Description = "The whole tool path as list with curves", Access = GH_ParamAccess.list},
        };

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddParameter(10);
        }

        /// <summary>
        /// Override this method if you want to be called before the first call to SolveInstance.
        /// </summary>
        protected override void BeforeSolveInstance()
        {
            base.BeforeSolveInstance();

            _forwardKinematics.Clear();
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            Robot robot = new Robot();
            List<RobotComponents.ABB.Actions.IAction> actions = new List<RobotComponents.ABB.Actions.IAction>();
            int interpolations = 0;
            double interpolationSlider = 0;
            bool update = false;

            // Catch the input data
            if (!DA.GetData(0, ref robot)) { return; }
            if (!DA.GetDataList(1, actions)) { return; }
            if (!DA.GetData(2, ref interpolations)) { return; }
            if (!DA.GetData(3, ref interpolationSlider)) { return; }
            if (!DA.GetData(4, ref update)) { return; }

            // Create forward kinematics for mesh display
            ForwardKinematics forwardKinematics = new ForwardKinematics(robot);

            // Fill list if needed
            if (DA.Iteration >= _calculated.Count)
            {
                _calculated.Add(false);
            }

            // Update the path
            if (update == true | _calculated[DA.Iteration] == false)
            {
                // Create the path generator
                if (DA.Iteration >= _pathGenerators.Count)
                {
                    _pathGenerators.Add(new PathGenerator(robot));
                }
                else
                {
                    _pathGenerators[DA.Iteration] = new PathGenerator(robot);
                }

                // Re-calculate the path
                _pathGenerators[DA.Iteration].Calculate(actions, interpolations);

                // Makes sure that there is always a calculated solution
                _calculated[DA.Iteration] = true;
            }

            // Get the index number of the current target
            int index = (int)((_pathGenerators[DA.Iteration].Planes.Count - 1) * interpolationSlider);

            // Calculate foward kinematics
            forwardKinematics.HideMesh = !_previewMesh;
            forwardKinematics.Calculate(_pathGenerators[DA.Iteration].RobotJointPositions[index], _pathGenerators[DA.Iteration].ExternalJointPositions[index]);

            // Show error messages
            if (_pathGenerators[DA.Iteration].ErrorText.Count != 0)
            {
                for (int i = 0; i < _pathGenerators[DA.Iteration].ErrorText.Count; i++)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, _pathGenerators[DA.Iteration].ErrorText[i]);
                    if (i == 30) { break; }
                }
            }

            // Add to list with FK
            _forwardKinematics.Add(forwardKinematics);

            // Output Parameters
            int ind;
            if (Params.Output.Any(x => x.NickName.Equality(_variableOutputParameters[0].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(_variableOutputParameters[0].NickName));
                DA.SetData(ind, _pathGenerators[DA.Iteration].Movements[index]);
            }
            if (Params.Output.Any(x => x.NickName.Equality(_variableOutputParameters[1].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(_variableOutputParameters[1].NickName));
                DA.SetDataList(ind, _pathGenerators[DA.Iteration].Movements);
            }
            if (Params.Output.Any(x => x.NickName.Equality(_variableOutputParameters[2].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(_variableOutputParameters[2].NickName));
                DA.SetData(ind, _pathGenerators[DA.Iteration].Planes[index]);
            }
            if (Params.Output.Any(x => x.NickName.Equality(_variableOutputParameters[3].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(_variableOutputParameters[3].NickName));
                DA.SetDataList(ind, _pathGenerators[DA.Iteration].Planes);
            }
            if (Params.Output.Any(x => x.NickName.Equality(_variableOutputParameters[4].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(_variableOutputParameters[4].NickName));
                DA.SetData(ind, _pathGenerators[DA.Iteration].RobotJointPositions[index]);
            }
            if (Params.Output.Any(x => x.NickName.Equality(_variableOutputParameters[5].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(_variableOutputParameters[5].NickName));
                DA.SetDataList(ind, _pathGenerators[DA.Iteration].RobotJointPositions);
            }
            if (Params.Output.Any(x => x.NickName.Equality(_variableOutputParameters[6].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(_variableOutputParameters[6].NickName));
                DA.SetDataList(ind, forwardKinematics.PosedExternalAxisPlanes);
            }
            if (Params.Output.Any(x => x.NickName.Equality(_variableOutputParameters[7].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(_variableOutputParameters[7].NickName));
                DA.SetData(ind, _pathGenerators[DA.Iteration].ExternalJointPositions[index]);
            }
            if (Params.Output.Any(x => x.NickName.Equality(_variableOutputParameters[8].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(_variableOutputParameters[8].NickName));
                DA.SetDataList(ind, _pathGenerators[DA.Iteration].ExternalJointPositions);
            }
            if (Params.Output.Any(x => x.NickName.Equality(_variableOutputParameters[9].NickName)))
            {
                ind = Params.Output.FindIndex(x => x.NickName.Equality(_variableOutputParameters[9].NickName));
                DA.SetDataList(ind, _pathGenerators[DA.Iteration].ErrorText);
            }
            DA.SetDataList(_variableOutputParameters[10].Name, _pathGenerators[DA.Iteration].Paths);
        }

        /// <summary>
        /// Override this method if you want to be called after the last call to SolveInstance.
        /// </summary>
        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();

            if (RunCount != -1)
            {
                if (_pathGenerators.Count > RunCount)
                {
                    _pathGenerators.RemoveRange(RunCount, _pathGenerators.Count - RunCount);
                }

                if (_calculated.Count > RunCount)
                {
                    _calculated.RemoveRange(RunCount, _calculated.Count - RunCount);
                }
            }
        }

        #region properties
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
            get { return new Guid("0B884DA3-BCB4-4CD5-A0EB-409864B34648"); }
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
            Menu_AppendItem(menu, "Preview Robot Mesh", MenuItemClickPreviewMesh, true, _previewMesh);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Output current Movement", MenuItemClickOutputMovement, true, _outputMovement);
            Menu_AppendItem(menu, "Output all Movements", MenuItemClickOutputMovements, true, _outputMovements);
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
            RecordUndoEvent("Preview Robot Mesh");
            _previewMesh = !_previewMesh;
            ExpireSolution(true);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Output current Movement" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickOutputMovement(object sender, EventArgs e)
        {
            RecordUndoEvent("Output current Movement");
            _outputMovement = !_outputMovement;
            AddParameter(0);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Output all Movements" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickOutputMovements(object sender, EventArgs e)
        {
            RecordUndoEvent("Output all Movements");
            _outputMovements = !_outputMovements;
            AddParameter(1);
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
            AddParameter(2);
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
            AddParameter(3);
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
            AddParameter(4);
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
            AddParameter(5);
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
            AddParameter(6);
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
            AddParameter(7);
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
            AddParameter(8);
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
            AddParameter(9);
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
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Set Preview Mesh", _previewMesh);
            writer.SetBoolean("Output Movement", _outputMovement);
            writer.SetBoolean("Output Movements", _outputMovements);
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
            _outputMovement = reader.GetBoolean("Output Movement");
            _outputMovements = reader.GetBoolean("Output Movements");
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
        /// Adds or destroys the output parameter to the component.
        /// </summary>
        /// <param name="index"> The index number of the parameter that needs to be added. </param>
        private void AddParameter(int index)
        {
            // Pick the parameter
            IGH_Param parameter = _variableOutputParameters[index];
            string name = _variableOutputParameters[index].NickName;

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
                    if (Params.Output.Any(x => x.NickName.Equality(_variableOutputParameters[i].NickName)))
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

        #region custom preview method
        /// <summary>
        /// Gets the clipping box for all preview geometry drawn by this component and all associated parameters.
        /// </summary>
        public override BoundingBox ClippingBox
        {
            get { return GetBoundingBox(); }
        }

        /// <summary>
        /// Returns the bounding box for all preview geometry drawn by this component.
        /// </summary>
        /// <returns></returns>
        private BoundingBox GetBoundingBox()
        {
            BoundingBox result = new BoundingBox();

            // Get bouding box of all the output parameters
            for (int i = 0; i < Params.Output.Count; i++)
            {
                if (Params.Output[i] is IGH_PreviewObject previewObject)
                {
                    result.Union(previewObject.ClippingBox);
                }
            }

            // Add bounding box of custom preview
            if (_previewMesh)
            {
                for (int i = 0; i < _forwardKinematics.Count; i++)
                {
                    result.Union(_forwardKinematics[i].GetBoundingBox(false));
                }
            }

            return result;
        }

        /// <summary>
        /// This method displays the robot pose for the given axis values. 
        /// </summary>
        /// <param name="args"> Preview display arguments for IGH_PreviewObjects. </param>
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            // Default implementation (disabled)
            // base.DrawViewportMeshes(args);

            if (_previewMesh)
            {
                for (int i = 0; i < _forwardKinematics.Count; i++)
                {
                    // Initiate the display color and transparancy of the robot mesh
                    Color color;
                    double trans;

                    // Set the display color and transparancy of the robot mesh
                    if (_forwardKinematics[i].IsInLimits == true)
                    {
                        color = Color.FromArgb(225, 225, 225);
                        trans = 0.0;
                    }
                    else
                    {
                        color = Color.FromArgb(150, 0, 0);
                        trans = 0.5;
                    }

                    // Display internal axis meshes
                    for (int j = 0; j < _forwardKinematics[i].PosedRobotMeshes.Count; j++)
                    {
                        if (_forwardKinematics[i].PosedRobotMeshes[j].IsValid)
                        {
                            args.Display.DrawMeshShaded(_forwardKinematics[i].PosedRobotMeshes[j], new Rhino.Display.DisplayMaterial(color, trans));
                        }
                    }

                    // Display external axis meshes
                    for (int j = 0; j < _forwardKinematics[i].PosedExternalAxisMeshes.Count; j++)
                    {
                        for (int k = 0; k < _forwardKinematics[i].PosedExternalAxisMeshes[j].Count; k++)
                        {
                            if (_forwardKinematics[i].PosedExternalAxisMeshes[j][k].IsValid)
                            {
                                args.Display.DrawMeshShaded(_forwardKinematics[i].PosedExternalAxisMeshes[j][k], new Rhino.Display.DisplayMaterial(color, trans));
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
