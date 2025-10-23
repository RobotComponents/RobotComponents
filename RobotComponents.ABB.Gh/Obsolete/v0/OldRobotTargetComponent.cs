﻿// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// RobotComponents Action : Target component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldRobotTargetComponent : GH_Component, IGH_VariableParameterComponent, IObjectManager
    {
        // Fields
        private List<string> _registered = new List<string>();
        private List<string> _toRegister = new List<string>();
        private ObjectManager _objectManager;
        private string _lastName = "";
        private bool _isUnique = true;
        private bool _setReferencePlane = false;
        private bool _setExternalJointPosition = false;

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldRobotTargetComponent()
          : base("Robot Target", "RT",
              "Defines a Robot Target declaration for an Instruction : Movement or Inverse Kinematics component."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
            // Create the component label with a message
            Message = "EXTENDABLE";
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
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
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name as text", GH_ParamAccess.list, "defaultTar");
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Axis Configuration", "AC", "Axis Configuration as int. This will modify the fourth value of the Robot Configuration Data in the RAPID Movement code line.", GH_ParamAccess.list, 0);
        }

        // Register the number of fixed input parameters
        private readonly int fixedParamNumInput = 3;

        // Create an array with the variable input parameters
        readonly IGH_Param[] parameters = new IGH_Param[2]
        {
            new Param_Plane() { Name = "Reference Plane", NickName = "RP",  Description = "Reference Plane as a Plane", Access = GH_ParamAccess.list, Optional = true },
            new Param_ExternalJointPosition() { Name = "External Joint Position", NickName = "EJ", Description = "The resulting external joint position", Access = GH_ParamAccess.list, Optional = true }
        };

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_RobotTarget(), "Robot Target", "RT", "Resulting Robot Target");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Sets inputs and creates target
            List<string> names = new List<string>();
            List<Plane> planes = new List<Plane>();
            List<Plane> referencePlanes = new List<Plane>();
            List<int> axisConfigs = new List<int>();
            List<ExternalJointPosition> externalJointPositions = new List<ExternalJointPosition>();

            // Catch name input
            if (!DA.GetDataList(0, names)) { return; } // Fixed index

            // Catch target planes input
            if (!DA.GetDataList(1, planes)) { return; } // Fixed index

            // Catch reference plane input
            if (Params.Input.Any(x => x.Name == "Reference Plane"))
            {
                if (!DA.GetDataList("Reference Plane", referencePlanes))
                {
                    referencePlanes = new List<Plane>() { Plane.WorldXY };
                }
            }

            // Catch axis configuration input
            if (Params.Input.Any(x => x.Name == "Axis Configuration"))
            {
                if (!DA.GetDataList("Axis Configuration", axisConfigs)) return;
            }

            // Catch input data for setting the external joint position
            if (Params.Input.Any(x => x.Name == parameters[1].Name))
            {
                if (!DA.GetDataList(parameters[1].Name, externalJointPositions))
                {
                    externalJointPositions = new List<ExternalJointPosition>() { new ExternalJointPosition() };
                }
            }

            // Make sure variable input has a default value
            if (referencePlanes.Count == 0) { referencePlanes.Add(Plane.WorldXY); }
            if (externalJointPositions.Count == 0) { externalJointPositions.Add(new ExternalJointPosition()); }

            // Replace spaces
            names = HelperMethods.ReplaceSpacesAndRemoveNewLines(names);

            // Get longest Input List
            int[] sizeValues = new int[5];
            sizeValues[0] = names.Count;
            sizeValues[1] = planes.Count;
            sizeValues[2] = referencePlanes.Count;
            sizeValues[3] = axisConfigs.Count;
            sizeValues[4] = externalJointPositions.Count;

            int biggestSize = sizeValues.Max();

            // Keeps track of used indicies
            int nameCounter = -1;
            int planesCounter = -1;
            int referencePlaneCounter = -1;
            int axisConfigCounter = -1;
            int externalJointPositionCounter = -1;

            // Initiate list
            List<RobotTarget> targets = new List<RobotTarget>();

            // Creates targets
            for (int i = 0; i < biggestSize; i++)
            {
                string name = "";
                Plane plane = new Plane();
                Plane referencePlane = new Plane();
                int axisConfig = 0;
                ExternalJointPosition externalJointPosition = new ExternalJointPosition();

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

                // Target planes counter
                if (i < sizeValues[1])
                {
                    plane = planes[i];
                    planesCounter++;
                }
                else
                {
                    plane = planes[planesCounter];
                }

                // Reference plane counter
                if (i < sizeValues[2])
                {
                    referencePlane = referencePlanes[i];
                    referencePlaneCounter++;
                }
                else
                {
                    referencePlane = referencePlanes[referencePlaneCounter];
                }

                // Axis configuration counter
                if (i < sizeValues[3])
                {
                    axisConfig = axisConfigs[i];
                    axisConfigCounter++;
                }
                else
                {
                    axisConfig = axisConfigs[axisConfigCounter];
                }

                // External Joint Position
                if (i < sizeValues[4])
                {
                    externalJointPosition = externalJointPositions[i];
                    externalJointPositionCounter++;
                }
                else
                {
                    externalJointPosition = externalJointPositions[externalJointPositionCounter];
                }

                RobotTarget target = new RobotTarget(name, plane, referencePlane, new ConfigurationData(0, 0, 0, axisConfig), externalJointPosition);
                targets.Add(target);
            }

            // Sets Output
            DA.SetDataList(0, targets);

            #region Object manager
            _toRegister.Clear();
            _toRegister = targets.ConvertAll(item => item.Name);

            GH_Document doc = this.OnPingDocument();
            _objectManager = DocumentManager.GetDocumentObjectManager(doc);
            _objectManager.CheckVariableNames(this);

            if (doc != null)
            {
                doc.ObjectsDeleted += this.DocumentObjectsDeleted;
            }
            #endregion
        }

        /// <summary>
        /// Detect if the components gets removed from the canvas and deletes the 
        /// objects created with this components from the object manager. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        public void DocumentObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            if (e.Objects.Contains(this))
            {
                _objectManager.DeleteManagedData(this);
            }
        }

        // Methods for creating custom menu items and event handlers when the custom menu items are clicked
        #region menu items
        /// <summary>
        /// Boolean that indicates if the custom menu item for setting the Reference Plane is checked
        /// </summary>
        public bool SetReferencePlane
        {
            get { return _setReferencePlane; }
            set { _setReferencePlane = value; }
        }

        /// <summary>
        /// Boolean that indicates if the custom menu item for setting the External Joint Position
        /// </summary>
        public bool SetExternalJointPosition
        {
            get { return _setExternalJointPosition; }
            set { _setExternalJointPosition = value; }
        }

        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Set Reference Plane", SetReferencePlane);
            writer.SetBoolean("Set External Joint Position", SetExternalJointPosition);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            SetReferencePlane = reader.GetBoolean("Set Reference Plane");
            SetExternalJointPosition = reader.GetBoolean("Set External Joint Position");
            return base.Read(reader);
        }

        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Reference Plane", MenuItemClickReferencePlane, true, SetReferencePlane);
            Menu_AppendItem(menu, "External Joint Position", MenuItemClickExternalJointPosition, true, SetExternalJointPosition);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Reference Plane" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickReferencePlane(object sender, EventArgs e)
        {
            // Change bool
            RecordUndoEvent("Set Reference Plane");
            SetReferencePlane = !SetReferencePlane;

            // Input parameter
            IGH_Param parameter = parameters[0];

            // If the parameter already exist: unregister it
            if (Params.Input.Any(x => x.Name == parameter.Name))
            {
                // Unregister the parameter
                Params.UnregisterInputParameter(Params.Input.First(x => x.Name == parameter.Name), true);
            }

            // Else add the reference plane parameter
            else
            {
                // The index where the parameter should be added
                int index = 2;

                // Register the input parameter
                Params.RegisterInputParam(parameters[0], index);
            }

            // Expire solution and refresh parameters since they changed
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        /// <summary>
        /// Registers the event when the custom menu item "External Joint Position" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickExternalJointPosition(object sender, EventArgs e)
        {
            // Change bool
            RecordUndoEvent("Set External Joint Position");
            SetExternalJointPosition = !SetExternalJointPosition;

            // Input parameter
            IGH_Param parameter = parameters[1];

            // If the parameter already exist: unregister it
            if (Params.Input.Any(x => x.Name == parameter.Name))
            {
                // Unregister the parameter
                Params.UnregisterInputParameter(Params.Input.First(x => x.Name == parameter.Name), true);
            }

            // Else add the reference plane parameter
            else
            {
                // The index where the parameter should be added
                int index = fixedParamNumInput;

                // Correction for the index number if the reference place was already added
                if (Params.Input.Any(x => x.Name == "Reference Plane"))
                {
                    index += 1;
                }

                // Register the input parameter
                Params.RegisterInputParam(parameter, index);
            }

            // Refresh parameters since they changed
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        #endregion

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
            get { return Properties.Resources.RobTarget_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("78A98F12-EDAF-44C8-ABB0-67D70F8CA391"); }
        }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the variable names that are generated by this component are unique.
        /// </summary>
        public bool IsUnique
        {
            get { return _isUnique; }
            set { _isUnique = value; }
        }

        /// <summary>
        /// Gets or sets the current registered names.
        /// </summary>
        public List<string> Registered
        {
            get { return _registered; }
            set { _registered = value; }
        }

        /// <summary>
        /// Gets the variables names that need to be registered by the object manager.
        /// </summary>
        public List<string> ToRegister
        {
            get { return _toRegister; }
        }
    }
}
