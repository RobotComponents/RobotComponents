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
// It is OBSOLETE since version 0.06.000 (January 2020)
// It is replaced with a new component.

namespace RobotComponents.Gh.Components.Definitions
{
    /// <summary>
    /// RobotComponents Robot Info component. An inherent from the GH_Component Class.
    /// </summary>
    public class OldWorkObjectComponent : GH_Component
    {
        public OldWorkObjectComponent()
          : base("Work Object", "WorkObj",
              "Defines a new work object."
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
            pManager.AddTextParameter("Name", "N", "Work Object Name as String", GH_ParamAccess.list, "default_wo");
            pManager.AddPlaneParameter("Plane", "P", "Plane of the Work Object as Plane", GH_ParamAccess.list, Plane.WorldXY);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new WorkObjectParameter(), "Work Object", "WO", "Resulting Work Object");  //Todo: beef this up to be more informative.
        }

        // Fields
        private List<string> _woNames = new List<string>();
        private string _lastName = "";
        private bool _namesUnique;
        private ObjectManager _objectManager;
        private List<WorkObject> _workObjects = new List<WorkObject>();

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

            // Gets the Object Manager of this document
            _objectManager = DocumentManager.GetDocumentObjectManager(this.OnPingDocument());

            // Clears Work Object Name
            for (int i = 0; i < _woNames.Count; i++)
            {
                _objectManager.WorkObjectNames.Remove(_woNames[i]);
            }
            _woNames.Clear();

            // Removes lastName from WorkObjectNameList
            if (_objectManager.WorkObjectNames.Contains(_lastName))
            {
                _objectManager.WorkObjectNames.Remove(_lastName);
            }

            // Clears Work Objects Local List
            _workObjects.Clear();

            // Input variables
            List<string> names = new List<string>();
            List<Plane> planes = new List<Plane>();

            // Catch the input data
            if (!DA.GetDataList(0, names)) { return; }
            if (!DA.GetDataList(1, planes)) { return; }

            // Get longest Input List
            int[] sizeValues = new int[10];
            sizeValues[0] = names.Count;
            sizeValues[1] = planes.Count;
            int biggestSize = HelperMethods.GetBiggestValue(sizeValues);

            // Keeps track of used indicies
            int nameCounter = -1;
            int planesCounter = -1;

            // Creates targets
            List<WorkObject> workObjects = new List<WorkObject>();
            for (int i = 0; i < biggestSize; i++)
            {
                string name = "";
                Plane plane = new Plane();

                // Names counter
                if (i < names.Count)
                {
                    name = names[i];
                    nameCounter++;
                }
                else
                {
                    name = names[nameCounter] + "_" + (i - nameCounter);
                }

                // Target planes counter
                if (i < planes.Count)
                {
                    plane = planes[i];
                    planesCounter++;
                }
                else
                {
                    plane = planes[planesCounter];
                }

              
                WorkObject workObject = new WorkObject(name, plane);
                workObjects.Add(workObject);
            }

            // Checks if the work object name is already in use and counts duplicates
            #region NameCheck
            _namesUnique = true;
            for (int i = 0; i < names.Count; i++)
            {
                if (_objectManager.WorkObjectNames.Contains(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Work Object Name already in use.");
                    _namesUnique = false;
                    _lastName = "";
                    break;
                }
                else
                {
                    // Adds Work Object Name to list
                    _woNames.Add(names[i]);
                    _objectManager.WorkObjectNames.Add(names[i]);

                    // Run SolveInstance on other Work Objects with no unique Name to check if their name is now available
                    _objectManager.UpdateWorkObjects();

                    _lastName = names[i];
                }

                // Checks if variable name exceeds max character limit for RAPID Code
                if (HelperMethods.VariableExeedsCharacterLimit32(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Work Object Name exceeds character limit of 32 characters.");
                    break;
                }

                // Checks if variable name starts with a number
                if (HelperMethods.VariableStartsWithNumber(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Work Object Name starts with a number which is not allowed in RAPID Code.");
                    break;
                }
            }
            #endregion

            // Output
            _workObjects = workObjects;
            DA.SetDataList(0, workObjects);


            // Adds Component to OldWorkObjectsByGuid Dictionary
            if (!_objectManager.OldWorkObjectsByGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.OldWorkObjectsByGuid.Add(this.InstanceGuid, this);
            }

            // Recognizes if Component is Deleted and removes it from Object Managers target and name list
            GH_Document doc = this.OnPingDocument();
            if (doc != null)
            {
                doc.ObjectsDeleted += DocumentObjectsDeleted;
            }
        }

        /// <summary>
        /// Detect if the components gets removed from the canvas and deletes the 
        /// objects created with this components from the object manager. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private void DocumentObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            if (e.Objects.Contains(this))
            {
                if (_namesUnique == true)
                {
                    for (int i = 0; i < _woNames.Count; i++)
                    {
                        _objectManager.WorkObjectNames.Remove(_woNames[i]);
                    }
                }
                _objectManager.OldWorkObjectsByGuid.Remove(this.InstanceGuid);

                // Runs SolveInstance on all other WorkObjects to check if robot tool names are unique.
                _objectManager.UpdateWorkObjects();
            }
        }

        /// <summary>
        /// The work object created by this component
        /// </summary>
        public List<WorkObject> WorkObjects
        {
            get { return _workObjects; }
        }

        public string LastName { get => _lastName; }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.Work_Object_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("60F2B882-E88B-4928-8517-AA5666F8137F"); }
        }
    }
}
