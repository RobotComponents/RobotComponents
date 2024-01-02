﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
//Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Instructions;
using RobotComponents.ABB.Kinematics;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Invesere Kinematics component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class InverseKinematicsComponent_OBSOLETE3 : GH_Component, IGH_VariableParameterComponent
    {
        #region fields
        private readonly List<InverseKinematics> _inverseKinematics = new List<InverseKinematics>();
        private readonly List<ForwardKinematics> _forwardKinematics = new List<ForwardKinematics>();
        private readonly List<RobotJointPosition> _previousRobotJointPositions = new List<RobotJointPosition>();
        private bool _closestRobotJointPosition = false;
        private bool _hideMesh = false;
        private readonly int _fixedParamNumInput = 2;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public InverseKinematicsComponent_OBSOLETE3()
          : base("Inverse Kinematics", "IK",
              "Computes the axis values for a defined ABB robot based on an Action: Movement."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "Robot Components ABB", "Simulation")
        {
            // Create the component label with a message
            Message = "EXTENDABLE";
        }

        /// <summary>
        /// Stores the variable input parameters in an array.
        /// </summary>
        private readonly IGH_Param[] variableInputParameters = new IGH_Param[2]
        {
            new Param_Boolean() { Name = "Closest", NickName = "C", Description = "Calculate closest Robot Joint Position to previous Robot Joint Position as a bool.", Access = GH_ParamAccess.item, Optional = true},
            new Param_Boolean() { Name = "Reset", NickName = "R", Description = "Reset Robot Joint Position as a bool.", Access = GH_ParamAccess.item, Optional = true },
        };

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Robot(), "Robot", "R", "Robot as Robot", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Movement(), "Movement", "M", "Movement or target input. A target will automatically be casted to a movement.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_RobotJointPosition(), "Robot Joint Position", "RJ", "The calculated Robot Joint Position");
            pManager.RegisterParam(new Param_ExternalJointPosition(), "External Joint Position", "EJ", "The calculated External Joint Position");
        }

        /// <summary>
        /// Override this method if you want to be called before the first call to SolveInstance.
        /// </summary>
        protected override void BeforeSolveInstance()
        {
            base.BeforeSolveInstance();

            _inverseKinematics.Clear();
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
            Robot robot = null;
            Movement movement = null;
            bool closestRobotJointPosition = false;
            bool reset = false;

            // Catch the input data
            if (!DA.GetData(0, ref robot)) { return; }
            if (!DA.GetData(1, ref movement)) { return; }

            // Catch the input data from the variable parameteres
            if (Params.Input.Any(x => x.Name == variableInputParameters[0].Name))
            {
                if (!DA.GetData(variableInputParameters[0].Name, ref closestRobotJointPosition))
                {
                    closestRobotJointPosition = false;
                }
            }
            if (Params.Input.Any(x => x.Name == variableInputParameters[1].Name))
            {
                if (!DA.GetData(variableInputParameters[1].Name, ref reset))
                {
                    reset = false;
                }
            }

            // Default previous robot joint position
            if (DA.Iteration > _previousRobotJointPositions.Count - 1)
            {
                _previousRobotJointPositions.Add(new RobotJointPosition());
            }
            
            // Calculate the robot pose
            _inverseKinematics.Add(new InverseKinematics(robot, movement));
            _inverseKinematics[DA.Iteration].Calculate();

            // Closest Robot Joint Position
            if (closestRobotJointPosition == true && reset == false && movement.Target is RobotTarget && movement.MovementType != MovementType.MoveAbsJ)
            {
                _inverseKinematics[DA.Iteration].CalculateClosestRobotJointPosition(_previousRobotJointPositions[DA.Iteration]);
            }

            _previousRobotJointPositions[DA.Iteration] = _inverseKinematics[DA.Iteration].RobotJointPosition.Duplicate();

            // Check the values
            for (int i = 0; i < _inverseKinematics[DA.Iteration].ErrorText.Count; i++)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, _inverseKinematics[DA.Iteration].ErrorText[i]);
            }

            // Add to list with FK for visualization
            if (!_hideMesh)
            {
                ForwardKinematics forwardKinematics = new ForwardKinematics(robot);
                forwardKinematics.Calculate(_inverseKinematics[DA.Iteration].RobotJointPosition, _inverseKinematics[DA.Iteration].ExternalJointPosition); 
                _forwardKinematics.Add(forwardKinematics);
            }

            // Output
            DA.SetData(0, _inverseKinematics[DA.Iteration].RobotJointPosition);
            DA.SetData(1, _inverseKinematics[DA.Iteration].ExternalJointPosition);
        }

        /// <summary>
        /// Override this method if you want to be called after the last call to SolveInstance.
        /// </summary>
        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();

            if (_previousRobotJointPositions.Count > RunCount)
            {
                _previousRobotJointPositions.RemoveRange(RunCount, _previousRobotJointPositions.Count - 1);
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
            get { return Properties.Resources.InverseKinematics_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F786750C-FB68-4E1A-A55D-9968C4F4A6C4"); }
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
            Menu_AppendItem(menu, "Preview Robot Mesh", MenuItemClickHideMesh, true, !_hideMesh);
            Menu_AppendItem(menu, "Closest Robot Joint Position", MenuItemClickClosestRobotJointPosition, true, _closestRobotJointPosition);
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
            RecordUndoEvent("Preview Robot Mesh");
            _hideMesh = !_hideMesh;
            ExpireSolution(true);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Closest Robot Joint Position" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickClosestRobotJointPosition(object sender, EventArgs e)
        {
            RecordUndoEvent("Set Closest Robot Joint Position");
            _closestRobotJointPosition = !_closestRobotJointPosition;
            AddParameter(0);
            AddParameter(1);
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Set Hide Mesh", _hideMesh);
            writer.SetBoolean("Closest Robot Joint Position", _closestRobotJointPosition);
            writer.SetByteArray("Previous Robot Joint Positions", RobotComponents.Utils.Serialization.ObjectToByteArray(_previousRobotJointPositions));
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            _hideMesh = reader.GetBoolean("Set Hide Mesh");
            _closestRobotJointPosition = reader.GetBoolean("Closest Robot Joint Position");
            _previousRobotJointPositions.Clear();

            try
            {
                _previousRobotJointPositions.AddRange((List<RobotJointPosition>)RobotComponents.Utils.Serialization.ByteArrayToObject(reader.GetByteArray("Previous Robot Joint Positions")));
            }

            catch
            {
                _previousRobotJointPositions.Clear();
            }

            return base.Read(reader);
        }

        /// <summary>
        /// Adds or destroys the input parameter to the component.
        /// </summary>
        /// <param name="index"> The index number of the parameter that needs to be added. </param>
        private void AddParameter(int index)
        {
            // Pick the parameter
            IGH_Param parameter = variableInputParameters[index];
            string name = variableInputParameters[index].Name;

            // If the parameter already exist: remove it
            if (Params.Input.Any(x => x.Name == name))
            {
                Params.UnregisterInputParameter(Params.Input.First(x => x.Name == name), true);
            }

            // Else remove the variable input parameter
            else
            {
                // The index where the parameter should be added
                int insertIndex = _fixedParamNumInput;

                // Check if other parameters are already added and correct the insert index
                for (int i = 0; i < index; i++)
                {
                    if (Params.Input.Any(x => x.Name == variableInputParameters[i].Name))
                    {
                        insertIndex += 1;
                    }
                }

                // Register the input parameter
                Params.RegisterInputParam(parameter, insertIndex);
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
            if (!_hideMesh)
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

            if (!_hideMesh)
            {
                for (int i = 0; i < _forwardKinematics.Count; i++)
                {
                    // Initiate the display color and transparancy of the robot mesh
                    Color color;
                    double trans;

                    // Set the display color and transparancy of the robot mesh
                    if (_forwardKinematics[i].InLimits == true)
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