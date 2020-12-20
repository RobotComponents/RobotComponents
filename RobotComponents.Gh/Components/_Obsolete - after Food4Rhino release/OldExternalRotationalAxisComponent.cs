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
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Definitions;
using RobotComponents.Gh.Utils;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.13.000
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Definitions
{
    /// <summary>
    /// RobotComponents External Rotational Axis component. An inherent from the GH_Component Class.
    /// </summary>
    public class OldExternalRotationalAxisComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldExternalRotationalAxisComponent()
          : base("External Rotational Axis", "External Rotational Axis",
              "Defines an External Rotational Axis."
                + System.Environment.NewLine + System.Environment.NewLine +
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
            pManager.AddTextParameter("Name", "N", "Axis name as a Text", GH_ParamAccess.item, "default_era");
            pManager.AddPlaneParameter("Axis Plane", "AP", "Axis Plane as a Plane", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Axis Limits", "AL", "Axis Limits as Domain", GH_ParamAccess.item);
            pManager.AddMeshParameter("Base Mesh", "BM", "Base Mesh as Mesh", GH_ParamAccess.list);
            pManager.AddMeshParameter("Link Mesh", "LM", "Link Mesh as Mesh", GH_ParamAccess.list);

            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

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
            
            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref axisPlane)) { return; }
            if (!DA.GetData(2, ref limits)) { return; }
            if (!DA.GetDataList(3, baseMeshes)) { baseMeshes = new List<Mesh>() { new Mesh() }; }
            if (!DA.GetDataList(4, linkMeshes)) { linkMeshes = new List<Mesh>() { new Mesh() }; }

            // Create the external rotational axis
            _externalRotationalAxis = new ExternalRotationalAxis(name, axisPlane, limits, baseMeshes, linkMeshes);

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
            if (!_objectManager.OldExternalRotationalAxesByGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.OldExternalRotationalAxesByGuid.Add(this.InstanceGuid, this);
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
                _objectManager.OldExternalRotationalAxesByGuid.Remove(this.InstanceGuid);

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
            get { return new Guid("21E3D4EE-18F7-4DCB-AF08-C537A656078D"); }
        }
    }
}