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

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Robot Tool from Planes component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldRobotToolFromPlanesComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear,  Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldRobotToolFromPlanesComponent()
          : base("Robot Tool From Planes", "RobToool",
              "Generates a robot tool based on attachment and effector planes."
            + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Definitions")
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
            pManager.AddTextParameter("Name", "N", "Robot Tool Name as Text", GH_ParamAccess.item, "default_tool");
            pManager.AddMeshParameter("Mesh", "M", "Robot Tool Mesh as Mesh", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Attachment Plane", "AP", "Robot Tool Attachment Plane as Plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddPlaneParameter("Tool Plane", "TP", "Robot Tool Plane as Plane", GH_ParamAccess.item, Plane.WorldXY);

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
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Input variables
            string name = "default_tool";
            List<Mesh> meshes = new List<Mesh>();
            Plane attachmentPlane = Plane.Unset;
            Plane toolPlane = Plane.Unset;

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetDataList(1, meshes)) { meshes = new List<Mesh>() { new Mesh() }; }
            if (!DA.GetData(2, ref attachmentPlane)) { return; }
            if (!DA.GetData(3, ref toolPlane)) { return; };

            // Create the Robot Tool
            _robotTool = new RobotTool(name, meshes, attachmentPlane, toolPlane);

            // Outputs
            DA.SetData(0, _robotTool);
            DA.SetData(1, _robotTool.ToRAPIDDeclaration());

            #region Object manager
            // Gets ObjectManager of this document
            _objectManager = DocumentManager.GetDocumentObjectManager(this.OnPingDocument());

            // Clears tool name
            _objectManager.ToolNames.Remove(_toolName);
            _toolName = String.Empty;

            // Removes lastName from toolNameList
            if (_objectManager.ToolNames.Contains(_lastName))
            {
                _objectManager.ToolNames.Remove(_lastName);
            }

            // Adds Component to ToolsByGuid Dictionary
            if (!_objectManager.OldRobotToolFromPlanesGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.OldRobotToolFromPlanesGuid.Add(this.InstanceGuid, this);
            }

            // Checks if the tool name is already in use and counts duplicates
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
                _objectManager.OldRobotToolFromPlanesGuid.Remove(this.InstanceGuid);

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
            get { return Properties.Resources.ToolPlane_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6d430719-5c45-4b66-a676-71be54f6ee93"); }
        }
    }

}
