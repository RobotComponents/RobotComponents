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
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Definitions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Definitions
{
    /// <summary>
    /// RobotComponents External Rotational Axis component. An inherent from the GH_Component Class.
    /// </summary>
    public class ExternalRotationalAxisComponent : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public ExternalRotationalAxisComponent()
          : base("External Rotational Axis", "External Rotational Axis",
              "Defines an External Rotational Axis."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Definitions")
        {
            // Create the component label with a message
            Message = "EXTENDABLE";
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
            pManager.AddTextParameter("Name", "N", "Axis name as a Text", GH_ParamAccess.item, "default_era");
            pManager.AddPlaneParameter("Axis Plane", "AP", "Axis Plane as a Plane", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Domain", GH_ParamAccess.item);
            pManager.AddMeshParameter("Base Mesh", "BM", "Base Mesh as Mesh", GH_ParamAccess.list);
            pManager.AddMeshParameter("Link Mesh", "LM", "Link Mesh as Mesh", GH_ParamAccess.list);

            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        // Register the number of fixed input parameters
        private readonly int fixedParamNumInput = 5;

        // Create an array with the variable input parameters
        readonly IGH_Param[] variableInputParameters = new IGH_Param[2]
        {
            new Param_String() { Name = "Axis Logic Number", NickName = "AL", Description = "Axis Logic Number as Text", Access = GH_ParamAccess.item, Optional = true},
            new Param_Boolean() { Name = "Moves Robot", NickName = "MR", Description = "Moves Robot as Boolean", Access = GH_ParamAccess.item, Optional = true },
        };

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new ExternalRotationalAxisParameter(), "External Rotational Axis", "ERA", "Resulting External Rotational Axis");  //Todo: beef this up to be more informative.
        }

        // Fields
        private string _axisName = String.Empty;
        private string _lastName = "";
        private bool _nameUnique;
        private ObjectManager _objectManager;
        private ExternalRotationalAxis _externalRotationalAxis;
        private bool _axisLogicNumber = false;
        private bool _movesRobot = false;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            string name = "";
            Plane axisPlane= Plane.WorldXY;
            Interval limits = new Interval(0, 0);
            List<Mesh> baseMeshes = new List<Mesh>();
            List<Mesh> linkMeshes = new List<Mesh>();
            string axisLogic = "-1";
            bool movesRobot = false;

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref axisPlane)) { return; }
            if (!DA.GetData(2, ref limits)) { return; }
            if (!DA.GetDataList(3, baseMeshes)) { baseMeshes = new List<Mesh>() { new Mesh() }; }
            if (!DA.GetDataList(4, linkMeshes)) { linkMeshes = new List<Mesh>() { new Mesh() }; }

            // Catch the input data from the variable parameteres
            if (Params.Input.Any(x => x.Name == variableInputParameters[0].Name))
            {
                if (!DA.GetData(variableInputParameters[0].Name, ref axisLogic))
                {
                    axisLogic = "-1";
                }
            }
            if (Params.Input.Any(x => x.Name == variableInputParameters[1].Name))
            {
                if (!DA.GetData(variableInputParameters[1].Name, ref movesRobot))
                {
                    movesRobot = false;
                }
            }

            // Create the external rotational axis
            _externalRotationalAxis = new ExternalRotationalAxis(name, axisPlane, limits, baseMeshes, linkMeshes, axisLogic, movesRobot);

            // Output
            DA.SetData(0, _externalRotationalAxis);

            #region Object manager
            // Gets ObjectManager of this document
            _objectManager = DocumentManager.GetDocumentObjectManager(this.OnPingDocument());

            // Clears ExternalAxisNames
            _objectManager.ExternalAxisNames.Remove(_axisName);
            _axisName = String.Empty;

            // Removes lastName from ExternalAxisNames List
            if (_objectManager.ExternalAxisNames.Contains(_lastName))
            {
                _objectManager.ExternalAxisNames.Remove(_lastName);
            }

            // Adds Component to ExternalLinarAxesByGuid Dictionary
            if (!_objectManager.ExternalRotationalAxesByGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.ExternalRotationalAxesByGuid.Add(this.InstanceGuid, this);
            }

            // Checks if axis name is already in use and counts duplicates
            #region Check name in object manager
            if (_objectManager.ExternalAxisNames.Contains(_externalRotationalAxis.Name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "External Axis Name already in use.");
                _nameUnique = false;
                _lastName = "";
            }
            else
            {
                // Adds Robot Axis Name to list
                _axisName = _externalRotationalAxis.Name;
                _objectManager.ExternalAxisNames.Add(_externalRotationalAxis.Name);

                // Run SolveInstance on other External Axes with no unique Name to check if their name is now available
                _objectManager.UpdateExternalAxis();

                _lastName = _externalRotationalAxis.Name;
                _nameUnique = true;
            }
            #endregion

            // Recognizes if Component is Deleted and removes it from Object Managers axis and name list
            GH_Document doc = this.OnPingDocument();
            if (doc != null)
            {
                doc.ObjectsDeleted += DocumentObjectsDeleted;
            }
            #endregion
        }

        /// <summary>
        /// This method detects if the user deletes the component from the Grasshopper canvas. 
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void DocumentObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            if (e.Objects.Contains(this))
            {
                if (_nameUnique == true)
                {
                    _objectManager.ExternalAxisNames.Remove(_axisName);
                }
                _objectManager.ExternalRotationalAxesByGuid.Remove(this.InstanceGuid);

                // Runs SolveInstance on all other ExternalAxis components to check if external axis names are unique.
                _objectManager.UpdateExternalAxis();
            }
        }

        /// <summary>
        /// The external rotational axis created by this component
        /// </summary>
        public ExternalRotationalAxis ExternalRotationalAxis
        {
            get { return _externalRotationalAxis; }
        }

        /// <summary>
        /// The external rotational axis created by this component as External Axis
        /// </summary>
        public ExternalAxis ExternalAxis
        {
            get { return _externalRotationalAxis as ExternalAxis; }
        }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
        }

        #region menu item
        /// <summary>
        /// Add our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="writer"> Provides access to a subset of GH_Chunk methods used for writing archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Set Axis Logic Number", _axisLogicNumber);
            writer.SetBoolean("Set Moves Robot", _movesRobot);
            return base.Write(writer);
        }

        /// <summary>
        /// Read our own fields. Needed for (de)serialization of the variable input parameters.
        /// </summary>
        /// <param name="reader"> Provides access to a subset of GH_Chunk methods used for reading archives. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            _axisLogicNumber = reader.GetBoolean("Set Axis Logic Number");
            _movesRobot = reader.GetBoolean("Set Moves Robot");
            return base.Read(reader);
        }

        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Set Axis Logic Number", MenuItemClickAxisLogic, true, _axisLogicNumber);
            Menu_AppendItem(menu, "Set Moves Robot", MenuItemClickMovesRobot, true, _movesRobot);
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
        /// Handles the event when the custom menu item "Set Axis Logic Number" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickAxisLogic(object sender, EventArgs e)
        {
            RecordUndoEvent("Set Axis Logic Number");
            _axisLogicNumber = !_axisLogicNumber;
            AddParameter(0);
        }

        /// <summary>
        /// Handles the event when the custom menu item "Moves Robot" is clicked. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void MenuItemClickMovesRobot(object sender, EventArgs e)
        {
            RecordUndoEvent("Set Moves Robot");
            _movesRobot = !_movesRobot;
            AddParameter(1);
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
                int insertIndex = fixedParamNumInput;

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
            get { return Properties.Resources.ExternalRotationalAxis_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("81B3F16F-1E26-4E30-BC5B-1FEA030D837F"); }
        }
    }
}