// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.14.000
// It is replaced with a new component. 

namespace RobotComponents.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Action : Speed Data component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldSpeedDataComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldSpeedDataComponent()
          : base("Speed Data", "SD", 
              "Defines a speed data declaration for Move components."
               + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
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
            pManager.AddTextParameter("Name", "N", "Name of the Speed Data as text", GH_ParamAccess.list, "default_speed");
            pManager.AddNumberParameter("TCP Velocity", "vTCP", "TCP Velocity in mm/s as number", GH_ParamAccess.list);
            pManager.AddNumberParameter("ORI Velocity", "vORI", "Reorientation Velocity of the tool in degree/s as number", GH_ParamAccess.list, 500);
            pManager.AddNumberParameter("LEAX Velocity", "vLEAX", "Linear External Axes Velocity in mm/s", GH_ParamAccess.list, 5000);
            pManager.AddNumberParameter("REAX Velocity", "vREAX", "Reorientation of the External Rotational Axes in degrees/s", GH_ParamAccess.list, 1000);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new SpeedDataParameter(), "Speed Data", "SD", "Resulting Speed Data declaration");
        }

        // Fields
        private readonly List<string> _speedDataNames = new List<string>();
        private string _lastName = "";
        private bool _namesUnique;
        private List<SpeedData> _speedDatas = new List<SpeedData>();
        private ObjectManager _objectManager;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Sets inputs and creates target
            List<string> names = new List<string>();
            List<double> v_tcps = new List<double>();
            List<double> v_oris = new List<double>();
            List<double> v_leaxs = new List<double>();
            List<double> v_reaxs = new List<double>();

            // Catch the input data
            if (!DA.GetDataList(0, names)) { return; }
            if (!DA.GetDataList(1, v_tcps)) { return; }
            if (!DA.GetDataList(2, v_oris)) { return; }
            if (!DA.GetDataList(3, v_leaxs)) { return; }
            if (!DA.GetDataList(4, v_reaxs)) { return; }

            // Replace spaces
            names = HelperMethods.ReplaceSpacesAndRemoveNewLines(names);

            // Get longest Input List
            int[] sizeValues = new int[5];
            sizeValues[0] = names.Count;
            sizeValues[1] = v_tcps.Count;
            sizeValues[2] = v_oris.Count;
            sizeValues[3] = v_leaxs.Count;
            sizeValues[4] = v_reaxs.Count;
            int biggestSize = HelperMethods.GetBiggestValue(sizeValues);

            // Keeps track of used indicies
            int nameCounter = -1;
            int v_tcpCounter = -1;
            int v_oriCounter = -1;
            int v_leaxCounter = -1;
            int v_reaxCounter = -1;

            // Clear list
            _speedDatas.Clear();

            // Creates speed Datas
            for (int i = 0; i < biggestSize; i++)
            {
                string name = "";
                double v_tcp = 0;
                double v_ori = 0;
                double v_leax = 0;
                double v_reax = 0;

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

                // TCP speed counter
                if (i < sizeValues[1])
                {
                    v_tcp = v_tcps[i];
                    v_tcpCounter++;
                }
                else
                {
                    v_tcp = v_tcps[v_tcpCounter];
                }

                // Re-orientation speed counter
                if (i < sizeValues[2])
                {
                    v_ori = v_oris[i];
                    v_oriCounter++;
                }
                else
                {
                    v_ori = v_oris[v_oriCounter];
                }

                // External linear axis speed counter
                if (i < sizeValues[3])
                {
                    v_leax = v_leaxs[i];
                    v_leaxCounter++;
                }
                else
                {
                    v_leax = v_leaxs[v_leaxCounter];
                }

                // External revolving axis counter
                if (i < sizeValues[4])
                {
                    v_reax = v_reaxs[i];
                    v_reaxCounter++;
                }
                else
                {
                    v_reax = v_reaxs[v_reaxCounter];
                }

                // Construct speed data
                SpeedData speedData = new SpeedData(name, v_tcp, v_ori, v_leax, v_reax);
                _speedDatas.Add(speedData);
            }

            // Sets Output
            DA.SetDataList(0, _speedDatas);

            #region Object manager
            // Gets ObjectManager of this document
            _objectManager = DocumentManager.GetDocumentObjectManager(this.OnPingDocument());

            // Clears speedDataNames
            for (int i = 0; i < _speedDataNames.Count; i++)
            {
                _objectManager.SpeedDataNames.Remove(_speedDataNames[i]);
            }
            _speedDataNames.Clear();

            // Removes lastName from speedDataNameList
            if (_objectManager.SpeedDataNames.Contains(_lastName))
            {
                _objectManager.SpeedDataNames.Remove(_lastName);
            }

            // Adds Component to SpeedDataByGuid Dictionary
            if (!_objectManager.OldSpeedDatasByGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.OldSpeedDatasByGuid.Add(this.InstanceGuid, this);
            }

            // Checks if speed Data name is already in use and counts duplicates
            #region Check name in object manager
            _namesUnique = true;
            for (int i = 0; i < names.Count; i++)
            {
                if (_objectManager.SpeedDataNames.Contains(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Speed Data Name already in use.");
                    _namesUnique = false;
                    _lastName = "";
                }
                else
                {
                    // Adds Speed Data Name to list
                    _speedDataNames.Add(names[i]);
                    _objectManager.SpeedDataNames.Add(names[i]);

                    // Run SolveInstance on other Speed Data with no unique Name to check if their name is now available
                    _objectManager.UpdateSpeedDatas();

                    _lastName = names[i];
                }

                // Check variable name: character limit
                if (HelperMethods.VariableExeedsCharacterLimit32(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Speed Data Name exceeds character limit of 32 characters.");
                    break;
                }

                // Check variable name: start with number is not allowed
                if (HelperMethods.VariableStartsWithNumber(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Speed Data Name starts with a number which is not allowed in RAPID Code.");
                    break;
                }
            }
            #endregion

            // Recognizes if Component is Deleted and removes it from Object Managers speed Data and name list
            GH_Document doc = this.OnPingDocument();
            if (doc != null)
            {
                doc.ObjectsDeleted += DocumentObjectsDeleted;
            }
            #endregion
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
                    for (int i = 0; i < _speedDataNames.Count; i++)
                    {
                        _objectManager.SpeedDataNames.Remove(_speedDataNames[i]);
                    }
                }
                _objectManager.OldSpeedDatasByGuid.Remove(this.InstanceGuid);

                // Run SolveInstance on other Speed Data instances with no unique Name to check if their name is now available
                _objectManager.UpdateSpeedDatas();
            }
        }

        /// <summary>
        /// The Speed Datas created by this component
        /// </summary>
        public List<SpeedData> SpeedDatas
        {
            get { return _speedDatas; }
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
            get
            { return Properties.Resources.SpeedData_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5F900CB8-86D0-4429-992A-CC2422BBFBDE"); }
        }
    }
}

