// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2018-2020 EDEK Uni Kassel
// Copyright (c) 2020-2025 Arjen Deetman
//
// Authors:
//   - Gabriel Rumph (2018-2020)
//   - Benedikt Wannemacher (2018-2020)
//   - Arjen Deetman (2019-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Linq;
using System.Windows.Forms;
// Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Instructions;
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Gh.Parameters.Actions.Instructions;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.CodeGeneration
{
    /// <summary>
    /// RobotComponents Action : Movement component.
    /// </summary>
    public class MoveComponent : GH_RobotComponent, IGH_VariableParameterComponent
    {
        #region fields
        private bool _add = false;
        private bool _expire = false;
        private bool _cirPointInputParam = false;
        private bool _movementTimeInputParam = false;
        private bool _overrideRobotToolInputParam = false;
        private bool _overrideWorkObjectInputParam = false;
        private bool _digitalOutputInputParam = false;
        private readonly int _fixedParamNumInput = 1;
        private bool _isCircularMovement = false;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, subcategory the panel. 
        /// If you use non-existing tab or panel names new tabs/panels will automatically be created.
        /// </summary>
        public MoveComponent() : base("Move", "M", "Code Generation",
              "Defines a joint, linear or circular movement instruction.")

        {
            // Create the component label with a message
            Message = "EXTENDABLE";
        }

        /// <summary>
        /// Stores the variable input parameters in an array.
        /// </summary>
        private readonly IGH_Param[] _variableInputParameters = new IGH_Param[8]
        {
            new Param_RobotTarget() { Name = "Circular Point", NickName = "CP", Description = "Circular Point for MoveC instructions as Robot Target.", Access = GH_ParamAccess.item, Optional = true},
            new Param_Target() { Name = "Target", NickName = "TA", Description = "Target of the movement as Target.", Access = GH_ParamAccess.item, Optional = true},
            new Param_SpeedData() { Name = "Speed Data", NickName = "SD", Description = "Speed Data as Speed Data or as a number (vTCP).", Access = GH_ParamAccess.item, Optional = true},
            new Param_Number() { Name = "Time", NickName = "TI", Description = "The total movement time in seconds. This overwrites the defined speeddata value.", Access = GH_ParamAccess.item, Optional = true},
            new Param_ZoneData() { Name = "Zone Data", NickName = "ZD", Description = "Zone Data as Zone Data or as a number (path zone TCP).", Access = GH_ParamAccess.item, Optional = true},
            new Param_RobotTool() { Name = "Robot Tool", NickName = "RT", Description = "Overrides the default Robot Tool.", Access = GH_ParamAccess.item, Optional = true},
            new Param_WorkObject() { Name = "Work Object", NickName = "WO", Description = "Overrides the default Work Object.", Access = GH_ParamAccess.item, Optional = true },
            new Param_SetDigitalOutput() { Name = "Digital Output", NickName = "DO", Description = "Set a Digital Output for creation of MoveLDO and MoveJDO instructions.", Access = GH_ParamAccess.item, Optional = true }
        };

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Type", "TY", "Type as integer. Use 0 for MoveAbsJ, 1 for MoveL and 2 for MoveJ.", GH_ParamAccess.item, 0);
            AddParameter(1);
            AddParameter(2);
            AddParameter(4);

            pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_Movement(), "Movement", "M", "Resulting Move instruction");
        }

        /// <summary>
        /// Override this method if you want to be called before the first call to SolveInstance.
        /// </summary>
        protected override void BeforeSolveInstance()
        {
            base.BeforeSolveInstance();

            _isCircularMovement = false;
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Creates the input value list and attachs it to the input parameter
            if (Params.Input[0].SourceCount == 0 & _add == true)
            {
                _expire = true;
                HelperMethods.CreateValueList(this, typeof(MovementType), 0);
            }

            // Expire solution of this component
            if (_expire == true)
            {
                _expire = false;
                ExpireSolution(true);
            }

            // Input variables
            int movementType = 0;
            RobotTarget cirPoint = new RobotTarget(Plane.Unset);
            ITarget target = new JointTarget();
            SpeedData speedData = new SpeedData();
            double time = -1;
            ZoneData zoneData = new ZoneData();
            RobotTool robotTool = RobotTool.GetEmptyRobotTool();
            WorkObject workObject = new WorkObject();
            SetDigitalOutput digitalOutput = new SetDigitalOutput();

            // Catch the input data from the fixed parameters
            if (!DA.GetData(0, ref movementType)) { movementType = 0; }

            // Catch the input data from the variable parameteres
            if (Params.Input.Any(x => x.Name == _variableInputParameters[0].Name))
            {
                if (!DA.GetData(_variableInputParameters[0].Name, ref cirPoint))
                {
                    cirPoint = new RobotTarget(Plane.Unset);
                }
            }
            if (Params.Input.Any(x => x.Name == _variableInputParameters[1].Name))
            {
                if (!DA.GetData(_variableInputParameters[1].Name, ref target))
                {
                    target = new JointTarget(new RobotJointPosition());
                }
            }
            if (Params.Input.Any(x => x.Name == _variableInputParameters[2].Name))
            {
                if (!DA.GetData(_variableInputParameters[2].Name, ref speedData))
                {
                    speedData = new SpeedData(5);
                }
            }
            if (Params.Input.Any(x => x.Name == _variableInputParameters[3].Name))
            {
                if (!DA.GetData(_variableInputParameters[3].Name, ref time))
                {
                    time = -1;
                }
            }
            if (Params.Input.Any(x => x.Name == _variableInputParameters[4].Name))
            {
                if (!DA.GetData(_variableInputParameters[4].Name, ref zoneData))
                {
                    zoneData = new ZoneData(0);
                }
            }
            if (Params.Input.Any(x => x.Name == _variableInputParameters[5].Name))
            {
                if (!DA.GetData(_variableInputParameters[5].Name, ref robotTool))
                {
                    robotTool = RobotTool.GetEmptyRobotTool();
                }
            }
            if (Params.Input.Any(x => x.Name == _variableInputParameters[6].Name))
            {
                if (!DA.GetData(_variableInputParameters[6].Name, ref workObject))
                {
                    workObject = new WorkObject();
                }
            }
            if (Params.Input.Any(x => x.Name == _variableInputParameters[7].Name))
            {
                if (!DA.GetData(_variableInputParameters[7].Name, ref digitalOutput))
                {
                    digitalOutput = new SetDigitalOutput();
                }
            }

            // Movement constructor
            Movement movement = new Movement((MovementType)movementType, target, speedData, zoneData, robotTool, workObject, digitalOutput);
            movement.CircularPoint = cirPoint;
            movement.Time = time;

            // Check if a circular movement is used
            if (movementType == 3)
            {
                _isCircularMovement = true;
            }

            // Check if a right value is used for the movement type
            if (movementType != 0 && movementType != 1 && movementType != 2 && movementType != 3)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Movement type value is invalid. " +
                    "In can only be set to 0, 1, 2 and 3. Use 0 for MoveAbsJ, 1 for MoveL, 2 for MoveJ and 3 for MoveC.");
            }

            // Check if an exact predefined zonedata value is used
            if (zoneData.IsExactPredefinedValue == false & zoneData.IsPreDefined == true)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Predefined zonedata value is invalid. " +
                    "The nearest valid predefined speeddata value is used. Valid predefined zonedata values are -1, " +
                    "0, 1, 5, 10, 15, 20, 30, 40, 50, 60, 80, 100, 150 or 200. " +
                    "A value of -1 will be interpreted as fine movement in RAPID Code.");
            }

            // Check if an exact predefined speeddata value is used
            if (speedData.IsExactPredefinedValue == false & speedData.IsPreDefined == true)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Predefined speeddata value is invalid. " +
                    "The nearest valid predefined speed data value is used. Valid predefined speeddata values are 5, 10, " +
                    "20, 30, 40, 50, 60, 80, 100, 150, 200, 300, 400, 500, 600, 800, 1000, 1500, 2000, 2500, 3000, 4000, " +
                    "5000, 6000 and 7000.");
            }

            // Check target and movement combination
            if (movement.MovementType == MovementType.MoveAbsJ && movement.Target is RobotTarget)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "An Absolute Joint Movement instruction is combined with " +
                    "a Robot Target. The Robot Target will be converted to a Joint Target.");
            }

            // Movement time
            if (movement.Time > 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "A movement time is defined. This overwrites the defined speeddata value.");
            }

            // Output
            DA.SetData(0, movement);
        }

        /// <summary>
        /// Override this method if you want to be called after the last call to SolveInstance.
        /// </summary>
        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();

            if (_add == false)
            {
                _add = true;

                if (Params.Input[0].SourceCount == 0)
                {
                    ExpireSolution(true);
                }
            }

            if (_isCircularMovement == true)
            {
                if (Params.Input.Any(x => x.Name == _variableInputParameters[0].Name) == false)
                {
                    _cirPointInputParam = !_cirPointInputParam;
                    AddParameter(0);
                }
            }
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
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.Movement_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("151B9D23-5B05-40CA-8DBD-F182FBEA8ABA"); }
        }
        #endregion

        #region menu items
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Set Circular Point", MenuItemClickCircularPoint, true, _cirPointInputParam);
            Menu_AppendItem(menu, "Set Movement Time", MenuItemClickMovementTime, true, _movementTimeInputParam);
            Menu_AppendItem(menu, "Override Robot Tool", MenuItemClickRobotTool, true, _overrideRobotToolInputParam);
            Menu_AppendItem(menu, "Override Work Object", MenuItemClickWorkObject, true, _overrideWorkObjectInputParam);
            Menu_AppendItem(menu, "Set Digital Output", MenuItemClickDigitalOutput, true, _digitalOutputInputParam);

            base.AppendAdditionalComponentMenuItems(menu);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Circular Point" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickCircularPoint(object sender, EventArgs e)
        {
            RecordUndoEvent("Set Circular Point");
            _cirPointInputParam = !_cirPointInputParam;
            AddParameter(0);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Movement Time" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickMovementTime(object sender, EventArgs e)
        {
            RecordUndoEvent("Set Movement Time");
            _movementTimeInputParam = !_movementTimeInputParam;
            AddParameter(3);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Robot Tool" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickRobotTool(object sender, EventArgs e)
        {
            RecordUndoEvent("Override Robot Tool");
            _overrideRobotToolInputParam = !_overrideRobotToolInputParam;
            AddParameter(5);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Work Object" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickWorkObject(object sender, EventArgs e)
        {
            RecordUndoEvent("Override Work Object");
            _overrideWorkObjectInputParam = !_overrideWorkObjectInputParam;
            AddParameter(6);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Digital Output" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickDigitalOutput(object sender, EventArgs e)
        {
            RecordUndoEvent("Set Digital Output");
            _digitalOutputInputParam = !_digitalOutputInputParam;
            AddParameter(7);
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Set Circular Point", _cirPointInputParam);
            writer.SetBoolean("Set Movement Time", _movementTimeInputParam);
            writer.SetBoolean("Override Robot Tool", _overrideRobotToolInputParam);
            writer.SetBoolean("Override Work Object", _overrideWorkObjectInputParam);
            writer.SetBoolean("Set Digital Output", _digitalOutputInputParam);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            _cirPointInputParam = reader.GetBoolean("Set Circular Point");
            _movementTimeInputParam = reader.GetBoolean("Set Movement Time");
            _overrideRobotToolInputParam = reader.GetBoolean("Override Robot Tool");
            _overrideWorkObjectInputParam = reader.GetBoolean("Override Work Object");
            _digitalOutputInputParam = reader.GetBoolean("Set Digital Output");
            return base.Read(reader);
        }

        /// <summary>
        /// Adds or destroys the input parameter to the component.
        /// </summary>
        /// <param name="index"> The index number of the parameter that needs to be added. </param>
        private void AddParameter(int index)
        {
            // Pick the parameter
            IGH_Param parameter = _variableInputParameters[index];
            string name = _variableInputParameters[index].Name;

            // If the parameter already exists: remove it
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
                    if (Params.Input.Any(x => x.Name == _variableInputParameters[i].Name))
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
    }
}