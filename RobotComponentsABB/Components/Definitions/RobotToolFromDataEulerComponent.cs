using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using RobotComponents.BaseClasses;
using RobotComponentsABB.Parameters;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components
{
    /// <summary>
    /// RobotComponents Robot Tool from Euler Data component. An inherent from the GH_Component Class.
    /// </summary>
    public class RobotToolFromDataEulerComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RobotToolFromDataEulerComponent()
          : base("Robot Tool From Data", "RobToool",
              "Defines a robot tool based on translation and rotation values."
              + System.Environment.NewLine +
              "RobotComponents : v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Definitions")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Robot Tool Name as String", GH_ParamAccess.item, "default_tool");
            pManager.AddMeshParameter("Mesh", "M", "Robot Tool Mesh as Mesh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Translation X", "TX", "Translation in X direction", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Translation Y", "TY", "Translation in Y direction", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Translation Z", "TZ", "Translation in Z direction", GH_ParamAccess.item, 0.0);

            pManager.AddNumberParameter("Rotation X", "RX", "Rotation around the X Axis in radians", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Rotation Y", "RY", "Rotation around the Y Axis in radians", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Rotation Z", "RZ", "Rotation around the Z Axis in radians", GH_ParamAccess.item, 0.0);
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
            // Gets Document ID
            string documentID = DocumentManager.GetRobotComponentsDocumentID(this.OnPingDocument());

            // Checks if ObjectManager for this document already exists. If not it creates a new one
            if (!DocumentManager.ObjectManagers.ContainsKey(documentID))
            {
                DocumentManager.ObjectManagers.Add(documentID, new ObjectManager());
            }

            // Gets ObjectManager of this document
            _objectManager = DocumentManager.ObjectManagers[documentID];

            // Clears toolNames
            _objectManager.ToolNames.Remove(_robotTool.Name);

            // Removes lastName from toolNameList
            if (_objectManager.ToolNames.Contains(_lastName))
            {
                _objectManager.ToolNames.Remove(_lastName);
            }

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
            if (!DA.GetDataList(1,  meshes)) { return; }
            if (!DA.GetData(2, ref toolTransX)) { return; }
            if (!DA.GetData(3, ref toolTransY)) { return; }
            if (!DA.GetData(4, ref toolTransZ)) { return; }
            if (!DA.GetData(5, ref toolRotX)) { return; }
            if (!DA.GetData(6, ref toolRotY)) { return; }
            if (!DA.GetData(7, ref toolRotZ)) { return; }

            // Tool mesh
            Mesh mesh = new Mesh();

            // Join the tool mesh to one single mesh
            for (int i = 0; i < meshes.Count; i++)
            {
                mesh.Append(meshes[i]);
            }

            // Create the robot tool
            _robotTool = new RobotTool(name, mesh, toolTransX, toolTransY, toolTransZ, toolRotX, toolRotY, toolRotZ);

            // Checks if tool name is already in use and counts duplicates
            #region NameCheck
            if (_objectManager.ToolNames.Contains(_robotTool.Name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Tool Name already in use.");
                _nameUnique = false;
                _lastName = "";
            }
            else
            {
                // Adds Robot Tool Name to list
                _objectManager.ToolNames.Add(_robotTool.Name);

                // Run SolveInstance on other Tools with no unique Name to check if their name is now available
                foreach (KeyValuePair<Guid, RobotToolFromDataEulerComponent> entry in _objectManager.ToolsEulerByGuid)
                {
                    if (entry.Value.LastName == "")
                    {
                        entry.Value.ExpireSolution(true);
                    }
                }
                foreach (KeyValuePair<Guid, RobotToolFromPlanesComponent> entry in _objectManager.ToolsPlanesByGuid)
                {
                    if (entry.Value.LastName == "")
                    {
                        entry.Value.ExpireSolution(true);
                    }
                }

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

            // Outputs
            DA.SetData(0, _robotTool);
            DA.SetData(1, _robotTool.GetRSToolData());

            // Adds Component to ToolsByGuid Dictionary
            if (!_objectManager.ToolsEulerByGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.ToolsEulerByGuid.Add(this.InstanceGuid, this);
            }

            // Recognizes if Component is Deleted and removes it from Object Managers tool and name list
            GH_Document doc = this.OnPingDocument();
            if (doc != null)
            {
                doc.ObjectsDeleted += DocumentObjectsDeleted;
            }
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
                    _objectManager.ToolNames.Remove(_lastName);
                }
                _objectManager.ToolsEulerByGuid.Remove(this.InstanceGuid);

                // Run SolveInstance on other Tools with no unique Name to check if their name is now available
                foreach (KeyValuePair<Guid, RobotToolFromDataEulerComponent> entry in _objectManager.ToolsEulerByGuid)
                {
                        entry.Value.ExpireSolution(true);
                }
                foreach (KeyValuePair<Guid, RobotToolFromPlanesComponent> entry in _objectManager.ToolsPlanesByGuid)
                {
                        entry.Value.ExpireSolution(true);
                }
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
