using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using RobotComponents.BaseClasses;
using RobotComponentsABB.Goos;
using RobotComponentsABB.Parameters;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components
{
    /// <summary>
    /// RobotComponents Robot Info component. An inherent from the GH_Component Class.
    /// </summary>
    public class WorkObjectComponent : GH_Component
    {
        public WorkObjectComponent()
          : base("Work Object", "WorkObj",
              "Defines a new work object."
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
            pManager.AddTextParameter("Name", "N", "Work Object Name as String", GH_ParamAccess.item, "default_wo");
            pManager.AddPlaneParameter("Plane", "P", "Plane of the Work Object as Plane", GH_ParamAccess.item, Plane.WorldXY);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new WorkObjectParameter(), "Work Object", "WO", "Resulting Work Object", GH_ParamAccess.item);  //Todo: beef this up to be more informative.
        }

        // Fields
        private string _lastName = "";
        private bool _nameUnique;
        private WorkObject _workObject = new WorkObject();
        private ObjectManager _objectManager;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
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

            // Clears Work Object Name
            _objectManager.WorkObjectNames.Remove(_workObject.Name);


            // Removes lastName from WorkObjectNameList
            if (_objectManager.WorkObjectNames.Contains(_lastName))
            {
                _objectManager.WorkObjectNames.Remove(_lastName);
            }

            // Input variables
            string name = "default_wo";
            Plane plane = Plane.WorldXY;

            // Catch the input data
            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref plane)) { return; }

            _workObject = new WorkObject(name, plane);

            // Checks if the work object name is already in use and counts duplicates
            #region NameCheck
            if (_objectManager.WorkObjectNames.Contains(_workObject.Name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Work Object Name already in use.");
                _nameUnique = false;
                _lastName = "";
            }
            else
            {
                // Adds Work Object name to list
                _objectManager.WorkObjectNames.Add(_workObject.Name);

                // Run SolveInstance on other Work Objects with no unique Name to check if their name is now available
                foreach (KeyValuePair<Guid, WorkObjectComponent> entry in _objectManager.WorkObjectsByGuid)
                {
                    if (entry.Value._lastName == "")
                    {
                        entry.Value.ExpireSolution(true);
                    }
                }

                _lastName = _workObject.Name;
                _nameUnique = true;
            }

            // Checks if variable name exceeds max character limit for RAPID Code
            if (HelperMethods.VariableExeedsCharacterLimit32(_workObject.Name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Work Object Name exceeds character limit of 32 characters.");
            }

            // Checks if variable name starts with a number
            if (HelperMethods.VariableStartsWithNumber(_workObject.Name))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Work Object Name starts with a number which is not allowed in RAPID Code.");
            }
            #endregion

            // Output
            DA.SetData(0, _workObject);


            // Adds Component to WorkObjectsByGuid Dictionary
            if (!_objectManager.WorkObjectsByGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.WorkObjectsByGuid.Add(this.InstanceGuid, this);
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
                    if (_nameUnique == true)
                    {
                        _objectManager.WorkObjectNames.Remove(_lastName);
                    }
                    _objectManager.WorkObjectsByGuid.Remove(this.InstanceGuid);


                // Run SolveInstance on other Work Objects with no unique Name to check if their name is now available
                foreach (KeyValuePair<Guid, WorkObjectComponent> entry in _objectManager.WorkObjectsByGuid)
                {
                        entry.Value.ExpireSolution(true);
                }
            }
        }

        /// <summary>
        /// The work object created by this component
        /// </summary>
        public WorkObject WorkObject
        {
            get { return _workObject; }
        }
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
