// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2023-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2023-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Gh.Parameters.Definitions;
using RobotComponents.ABB.Gh.Goos.Definitions;
using RobotComponents.ABB.Gh.Utils;

namespace RobotComponents.ABB.Gh.Components.Definitions
{
    /// <summary>
    /// RobotComponents Load Data component.
    /// </summary>
    public class LoadDataComponent : GH_RobotComponent, IObjectManager
    {
        #region fields
        private GH_Structure<GH_LoadData> _tree = new GH_Structure<GH_LoadData>();
        private List<string> _registered = new List<string>();
        private readonly List<string> _toRegister = new List<string>();
        private ObjectManager _objectManager;
        private string _lastName = "";
        private bool _isUnique = true;
        #endregion

        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public LoadDataComponent() : base("Load Data", "LoDa", "Definitions",
              "Defines load data.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Load Data name as Text.", GH_ParamAccess.item, string.Empty);
            pManager.AddNumberParameter("Mass", "M", "The weight of the load in kg as a Number.", GH_ParamAccess.item, 0.001);
            pManager.AddPointParameter("Center of Gravity", "CG", "The center of gravity of the load as a Point.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Axes of Moment", "AM", "The orientation of the load coordinate system defined by the principal inertial axes of the tool as a Plane.", GH_ParamAccess.item);
            pManager.AddVectorParameter("Inertial Moments", "IM", "the moment of inertia of the load in kgm2 as a Vector.", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_LoadData(), "Load Data", "LD", "Resulting Load Data");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            string name = string.Empty;
            double mass = 0.001;
            Point3d centerOfGravity = new Point3d(0, 0, 0.001);
            Plane axesOfMoment = Plane.WorldXY;
            Vector3d inertialMoments = new Vector3d(0, 0, 0);

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref mass)) { mass = 0.001; }
            if (!DA.GetData(2, ref centerOfGravity)) { centerOfGravity = new Point3d(0, 0, 0.001); }
            if (!DA.GetData(3, ref axesOfMoment)) { axesOfMoment = Plane.WorldXY; }
            if (!DA.GetData(4, ref inertialMoments)) { inertialMoments = new Vector3d(0, 0, 0); }

            // Replace spaces
            name = HelperMethods.ReplaceSpacesAndRemoveNewLines(name);

            // Check tool mass
            if (mass < 0.0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Tool mass cannot be negative.");
            }

            // Convert to quaternion
            Quaternion quaternion = ABB.Utils.HelperMethods.PlaneToQuaternion(axesOfMoment);

            // Make work object
            LoadData loadData = new LoadData(name, mass, centerOfGravity, quaternion, inertialMoments);

            // Output
            DA.SetData(0, loadData);
        }

        /// <summary>
        /// Override this method if you want to be called after the last call to SolveInstance.
        /// </summary>
        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();

            _tree = this.Params.Output[0].VolatileData as GH_Structure<GH_LoadData>;

            if (_tree.Branches.Count != 0)
            {
                if (_tree.Branches[0][0].Value.Name != string.Empty)
                {
                    UpdateVariableNames();
                }

                #region Object manager
                _toRegister.Clear();

                for (int i = 0; i < _tree.Branches.Count; i++)
                {
                    _toRegister.AddRange(_tree.Branches[i].ConvertAll(item => item.Value.Name));
                }

                GH_Document doc = this.OnPingDocument();
                _objectManager = DocumentManager.GetDocumentObjectManager(doc);
                _objectManager.CheckVariableNames(this);

                if (doc != null)
                {
                    doc.ObjectsDeleted += this.DocumentObjectsDeleted;
                }
                #endregion
            }
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary, dropdown and obscure
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
            get { return Properties.Resources.LoadData_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4BC58335-0CBE-47D6-986B-61F6ABF1D199"); }
        }
        #endregion

        #region object manager
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
        #endregion

        #region additional methods
        /// <summary>
        /// Updates the variable names in the data tree
        /// </summary>
        private void UpdateVariableNames()
        {
            // Check if it is a datatree with multiple branches that have one item
            bool check = true;
            for (int i = 0; i < _tree.Branches.Count; i++)
            {
                if (_tree.Branches[i].Count != 1)
                {
                    check = false;
                    break;
                }
            }

            if (_tree.Branches.Count == 1)
            {
                if (_tree.Branches[0].Count == 1)
                {
                    // Do nothing: there is only one item in the whole datatree
                }
                else
                {
                    // Only rename the items in this single branche with + "_0", "_1" etc...
                    for (int i = 0; i < _tree.Branches[0].Count; i++)
                    {
                        _tree.Branches[0][i].Value.Name = _tree.Branches[0][i].Value.Name + "_" + i.ToString();
                    }
                }

            }

            else if (check == true)
            {
                // Multiple branches with only one item per branch
                for (int i = 0; i < _tree.Branches.Count; i++)
                {
                    _tree.Branches[i][0].Value.Name = _tree.Branches[i][0].Value.Name + "_" + i.ToString();
                }
            }

            else
            {
                // Rename everything. There are multiple branches with branches that have multiple items. 
                List<GH_Path> originalPaths = new List<GH_Path>();
                for (int i = 0; i < _tree.Paths.Count; i++)
                {
                    originalPaths.Add(_tree.Paths[i]);
                }

                _tree.Simplify(GH_SimplificationMode.CollapseLeadingOverlaps);

                List<GH_Path> simplifiedPaths = new List<GH_Path>();
                for (int i = 0; i < _tree.Paths.Count; i++)
                {
                    simplifiedPaths.Add(_tree.Paths[i]);
                }

                for (int i = 0; i < _tree.Branches.Count; i++)
                {
                    _tree.ReplacePath(simplifiedPaths[i], originalPaths[i]);
                }

                for (int i = 0; i < _tree.Branches.Count; i++)
                {
                    GH_Path iPath = simplifiedPaths[i];
                    string pathString = iPath.ToString();
                    pathString = pathString.Replace("{", "").Replace(";", "_").Replace("}", "");

                    for (int j = 0; j < _tree.Branches[i].Count; j++)
                    {
                        _tree.Branches[i][j].Value.Name = _tree.Branches[i][j].Value.Name + "_" + pathString + "_" + j;
                    }
                }
            }
        }
        #endregion
    }
}
