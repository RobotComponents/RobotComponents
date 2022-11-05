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
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Gh.Parameters.Definitions;
using RobotComponents.Gh.Utils;

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Robot Info component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is OBSOLETE and will be removed in the future.", false)]
    public class WorkObjectComponent_OBSOLETE : GH_Component, IObjectManager
    {
        #region fields
        private List<string> _registered = new List<string>();
        private List<string> _toRegister = new List<string>();
        private ObjectManager _objectManager;
        private string _lastName = "";
        private bool _isUnique = true;
        #endregion

        public WorkObjectComponent_OBSOLETE()
          : base("Work Object", "WorkObj",
              "Defines a new work object."
               + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Definitions")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Work Object Name as Text", GH_ParamAccess.list, "default_wo");
            pManager.AddPlaneParameter("Plane", "WP", "Plane of the Work Object as Plane", GH_ParamAccess.list, Plane.WorldXY);
            pManager.AddParameter(new Param_ExternalAxis(), "External Axis", "EA", "External Axis as an External Axis", GH_ParamAccess.list);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_WorkObject(), "Work Object", "WO", "Resulting Work Object");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Input variables
            List<string> names = new List<string>();
            List<Plane> planes = new List<Plane>();
            List<ExternalAxis> externalAxes = new List<ExternalAxis>();

            // Catch the input data
            if (!DA.GetDataList(0, names)) { return; }
            if (!DA.GetDataList(1, planes)) { return; }
            if (!DA.GetDataList(2, externalAxes)) { externalAxes = new List<ExternalAxis>() { null }; }

            // Replace spaces
            names = HelperMethods.ReplaceSpacesAndRemoveNewLines(names);

            // Get longest Input List
            int[] sizeValues = new int[3];
            sizeValues[0] = names.Count;
            sizeValues[1] = planes.Count;
            sizeValues[2] = externalAxes.Count;
            int biggestSize = sizeValues.Max();

            // Keeps track of used indicies
            int nameCounter = -1;
            int planesCounter = -1;
            int axisCounter = -1;

            // Creates work objects
            WorkObject workObject;
            List<WorkObject> workObjects = new List<WorkObject>();

            for (int i = 0; i < biggestSize; i++)
            {
                string name = "";
                Plane plane = new Plane();
                ExternalAxis externalAxis = null;

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

                // Planes counter
                if (i < sizeValues[1])
                {
                    plane = planes[i];
                    planesCounter++;
                }
                else
                {
                    plane = planes[planesCounter];
                }

                // Axis counter
                if (i < sizeValues[2])
                {
                    externalAxis = externalAxes[i];
                    axisCounter++;
                }
                else
                {
                    externalAxis = externalAxes[axisCounter];
                }

                // Make work object
                workObject = new WorkObject(name, plane, externalAxis);
                workObjects.Add(workObject);
            }

            // Output
            DA.SetDataList(0, workObjects);

            #region Object manager
            _toRegister.Clear();
            _toRegister = workObjects.ConvertAll(item => item.Name);

            GH_Document doc = this.OnPingDocument();
            _objectManager = DocumentManager.GetDocumentObjectManager(doc);
            _objectManager.CheckVariableNames(this);

            if (doc != null)
            {
                doc.ObjectsDeleted += this.DocumentObjectsDeleted;
            }
            #endregion
        }

        #region properties
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
            get { return new Guid("F892733B-3633-48A6-AAC7-1A244441A774"); }
        }
        #endregion

        #region menu item
        /// <summary>
        /// Adds the additional items to the context menu of the component. 
        /// </summary>
        /// <param name="menu"> The context menu of the component. </param>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
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
    }
}
