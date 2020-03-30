// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.BaseClasses.Definitions;
using RobotComponentsABB.Parameters.Definitions;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components.Definitions
{
    /// <summary>
    /// RobotComponents Robot Tool from Quaternion Data component. An inherent from the GH_Component Class.
    /// </summary>
    public class RobotToolFromQuaternionComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public RobotToolFromQuaternionComponent()
          : base("Robot Tool From Quaternion Data", "RobTool",
              "Defines a robot tool based on TCP coorindate and quarternion values."
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
            pManager.AddNumberParameter("Coord X", "X", "The x-coordinate of the tool center point.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Coord Y", "Y", "The y-coordinate of the tool center point.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Coord Z", "Z", "The z-coordinate of the tool center point.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Quaternion A", "A", "The real part of the quaternion.", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Quaternion B", "B", "The first imaginary coefficient of the quaternion.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Quaternion C", "C", "The second imaginary coefficient of the quaternion.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Quaternion D", "D", "The third imaginary coefficient of the quaternion.", GH_ParamAccess.item, 0.0);

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
            // Input variables
            string name = "default_tool";
            List<Mesh> meshes = new List<Mesh>();
            double x = 0.0;
            double y = 0.0;
            double z = 0.0;
            double quat1 = 0.0;
            double quat2 = 0.0;
            double quat3 = 0.0;
            double quat4 = 0.0;

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetDataList(1, meshes)) { meshes = new List<Mesh>() { new Mesh() }; }
            if (!DA.GetData(2, ref x)) { return; }
            if (!DA.GetData(3, ref y)) { return; }
            if (!DA.GetData(4, ref z)) { return; }
            if (!DA.GetData(5, ref quat1)) { return; }
            if (!DA.GetData(6, ref quat2)) { return; }
            if (!DA.GetData(7, ref quat3)) { return; }
            if (!DA.GetData(7, ref quat4)) { return; }

            // Tool mesh
            Mesh mesh = new Mesh();

            // Join the tool mesh to one single mesh
            for (int i = 0; i < meshes.Count; i++)
            {
                mesh.Append(meshes[i]);
            }

            // Create the robot tool
            _robotTool = new RobotTool(name, mesh, x, y, z, quat1, quat2, quat3, quat4);

            // Outputs
            DA.SetData(0, _robotTool);
            DA.SetData(1, _robotTool.GetRSToolData());

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
            if (!_objectManager.ToolsQuaternionByGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.ToolsQuaternionByGuid.Add(this.InstanceGuid, this);
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
                foreach (KeyValuePair<Guid, RobotToolFromQuaternionComponent> entry in _objectManager.ToolsQuaternionByGuid)
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

            // Recognizes if Component is Deleted and removes it from Object Managers tool and name list
            GH_Document doc = this.OnPingDocument();
            if (doc != null)
            {
                doc.ObjectsDeleted += DocumentObjectsDeleted;
            }
            #endregion
        }

        #region menu item
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            // Add menu separator
            Menu_AppendSeparator(menu);

            // Add custom menu items
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
            System.Diagnostics.Process.Start(url);
        }
        #endregion

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
                _objectManager.ToolsQuaternionByGuid.Remove(this.InstanceGuid);

                // Run SolveInstance on other Tools with no unique Name to check if their name is now available
                foreach (KeyValuePair<Guid, RobotToolFromDataEulerComponent> entry in _objectManager.ToolsEulerByGuid)
                {
                        entry.Value.ExpireSolution(true);
                }
                foreach (KeyValuePair<Guid, RobotToolFromPlanesComponent> entry in _objectManager.ToolsPlanesByGuid)
                {
                        entry.Value.ExpireSolution(true);
                }
                foreach (KeyValuePair<Guid, RobotToolFromQuaternionComponent> entry in _objectManager.ToolsQuaternionByGuid)
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
            get { return Properties.Resources.ToolQuaternion_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("037FD832-E3DF-41AB-AE03-23CE4FFEF075"); }
        }
    }
}
