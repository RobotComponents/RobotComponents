// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Utils;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.08.000
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Robot Tool from Euler Data component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldRobotToolFromDataEulerComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldRobotToolFromDataEulerComponent()
          : base("Robot Tool From Data", "RobToool",
              "Defines a robot tool based on translation and rotation values."
              + System.Environment.NewLine +
              "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Definitions")
        {
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
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Robot Tool Name as Text", GH_ParamAccess.item, "default_tool");
            pManager.AddMeshParameter("Mesh", "M", "Robot Tool Mesh as Mesh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Translation X", "TX", "Translation in X direction", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Translation Y", "TY", "Translation in Y direction", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Translation Z", "TZ", "Translation in Z direction", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Rotation X", "RX", "Rotation around the X Axis in radians", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Rotation Y", "RY", "Rotation around the Y Axis in radians", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Rotation Z", "RZ", "Rotation around the Z Axis in radians", GH_ParamAccess.item, 0.0);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new RobotToolParameter(), "Robot Tool", "RT", "Resulting Robot Tool");  //Todo: beef this up to be more informative.
            pManager.Register_StringParam("Robot Tool Code", "RTC", "Robot Tool Code as a string");
        }

        // Fields
        private string _toolName = String.Empty;
        private string _lastName = "";
        private bool _nameUnique;
        private RobotTool _robotTool = new RobotTool();
        private ObjectManager _objectManager;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed in the future.");

            // Input variables
            string name = "default_tool";
            List<Mesh> meshes = new List<Mesh>();
            double toolTransX = 0.0;
            double toolTransY = 0.0;
            double toolTransZ = 0.0;
            double toolRotX = 0.0;
            double toolRotY = 0.0;
            double toolRotZ = 0.0;

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetDataList(1, meshes)) { meshes = new List<Mesh>() { new Mesh() }; }
            if (!DA.GetData(2, ref toolTransX)) { return; }
            if (!DA.GetData(3, ref toolTransY)) { return; }
            if (!DA.GetData(4, ref toolTransZ)) { return; }
            if (!DA.GetData(5, ref toolRotX)) { return; }
            if (!DA.GetData(6, ref toolRotY)) { return; }
            if (!DA.GetData(7, ref toolRotZ)) { return; }

            // Create the robot tool
            _robotTool = new RobotTool(name, meshes, toolTransX, toolTransY, toolTransZ, toolRotX, toolRotY, toolRotZ);

            // Outputs
            DA.SetData(0, _robotTool);
            DA.SetData(1, _robotTool.ToRAPIDDeclaration());

            #region Object manager
            // Gets ObjectManager of this document
            _objectManager = DocumentManager.GetDocumentObjectManager(this.OnPingDocument());

            // Clears toolNames
            _objectManager.ToolNames.Remove(_toolName);
            _toolName = String.Empty;

            // Removes lastName from toolNameList
            if (_objectManager.ToolNames.Contains(_lastName))
            {
                _objectManager.ToolNames.Remove(_lastName);
            }

            // Adds Component to ToolsByGuid Dictionary
            if (!_objectManager.OldToolsEulerByGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.OldToolsEulerByGuid.Add(this.InstanceGuid, this);
            }

            // Checks if tool name is already in use and counts duplicates
            #region Check name in object manager
            if (_objectManager.ToolNames.Contains(_robotTool.Name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Tool Name already in use.");
                _nameUnique = false;
                _lastName = "";
            }
            else
            {
                // Adds Robot Tool Name to list
                _toolName = _robotTool.Name;
                _objectManager.ToolNames.Add(_robotTool.Name);

                // Run SolveInstance on other Tools with no unique Name to check if their name is now available
                _objectManager.UpdateRobotTools();

                _lastName = _robotTool.Name;
                _nameUnique = true;
            }

            // Checks if variable name exceeds max character limit for RAPID Code
            if (HelperMethods.VariableExeedsCharacterLimit32(name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Robot Tool Name exceeds character limit of 32 characters.");
            }

            // Checks if variable name starts with a number
            if (HelperMethods.VariableStartsWithNumber(name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Robot Tool Name starts with a number which is not allowed in RAPID Code.");
            }
            #endregion

            // Recognizes if Component is Deleted and removes it from Object Managers tool and name list
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
                    _objectManager.ToolNames.Remove(_toolName);
                }
                _objectManager.OldToolsEulerByGuid.Remove(this.InstanceGuid);

                // Runs SolveInstance on all other Robot Tools to check if robot tool names are unique.
                _objectManager.UpdateRobotTools();
            }
        }

        /// <summary>
        /// The robot tool created by this component
        /// </summary>
        public RobotTool RobotTool
        {
            get { return _robotTool; }
        }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.ToolData_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("647AD530-2800-4653-96B4-C5C50D1243CA"); }
        }
    }
}
