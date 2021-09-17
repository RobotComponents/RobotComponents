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
    /// RobotComponents Action : Zone Data component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldZoneDataComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldZoneDataComponent()
          : base("Zone Data", "ZD",
              "Defines a zone data declaration for robot movements in RAPID program code generation."
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
            pManager.AddTextParameter("Name", "N", "Name of the Zone Data as text", GH_ParamAccess.list, "default_zone");
            pManager.AddBooleanParameter("Fine Point", "FP", "Defines whether the movement is to terminate as a stop point (fine point) or as a fly-by point as a bool.", GH_ParamAccess.list, false);
            pManager.AddNumberParameter("Path Zone TCP", "pzTCP", "The size (the radius) of the TCP zone in mm as a number.", GH_ParamAccess.list, 0);
            pManager.AddNumberParameter("Path Zone Reorientation", "pzORI", "The zone size (the radius) for the tool reorientation in mm as a number. ", GH_ParamAccess.list, 0);
            pManager.AddNumberParameter("Path Zone External Axes", "pzEA", "The zone size (the radius) for external axes in mm as a number.", GH_ParamAccess.list, 0);
            pManager.AddNumberParameter("Zone Reorientation", "zORI", "The zone size for the tool reorientation in degrees as a number.", GH_ParamAccess.list, 0);
            pManager.AddNumberParameter("Zone External Linear Axes", "zELA", "The zone size for linear external axes in mm as a number.", GH_ParamAccess.list, 0);
            pManager.AddNumberParameter("Zone External Rotational Axes", "zERA", "The zone size for rotating external axes in degrees as a number.", GH_ParamAccess.list, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new ZoneDataParameter(), "Zone Data", "ZD", "Resulting Zone Data declaration");
        }

        // Fields
        private readonly List<string> _zoneDataNames = new List<string>();
        private string _lastName = "";
        private bool _namesUnique;
        private List<ZoneData> _zoneDatas = new List<ZoneData>();
        private ObjectManager _objectManager;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Sets inputs and creates target
            List<string> names = new List<string>();
            List<bool> fineps = new List<bool>();
            List<double> pzone_tcps = new List<double>();
            List<double> pzone_oris = new List<double>();
            List<double> pzone_eaxs = new List<double>();
            List<double> zone_oris = new List<double>();
            List<double> zone_leaxs = new List<double>();
            List<double> zone_reaxs = new List<double>();

            // Catch the input data
            if (!DA.GetDataList(0, names)) { return; }
            if (!DA.GetDataList(1, fineps)) { return; }
            if (!DA.GetDataList(2, pzone_tcps)) { return; }
            if (!DA.GetDataList(3, pzone_oris)) { return; }
            if (!DA.GetDataList(4, pzone_eaxs)) { return; }
            if (!DA.GetDataList(5, zone_oris)) { return; }
            if (!DA.GetDataList(6, zone_leaxs)) { return; }
            if (!DA.GetDataList(7, zone_reaxs)) { return; }

            // Replace spaces
            names = HelperMethods.ReplaceSpacesAndRemoveNewLines(names);

            // Get longest Input List
            int[] sizeValues = new int[8];
            sizeValues[0] = names.Count;
            sizeValues[1] = fineps.Count;
            sizeValues[2] = pzone_tcps.Count;
            sizeValues[3] = pzone_oris.Count;
            sizeValues[4] = pzone_eaxs.Count;
            sizeValues[5] = zone_oris.Count;
            sizeValues[6] = zone_leaxs.Count;
            sizeValues[7] = zone_reaxs.Count;
            int biggestSize = HelperMethods.GetBiggestValue(sizeValues);

            // Keeps track of used indicies
            int nameCounter = -1;
            int finepCounter = -1;
            int pzone_tcpCounter = -1;
            int pzone_oriCounter = -1;
            int pzone_eaxCounter = -1;
            int zone_oriCounter = -1;
            int zone_leaxCounter = -1;
            int zone_reaxCounter = -1;

            // Clear list
            _zoneDatas.Clear();

            // Creates Zone Datas
            for (int i = 0; i < biggestSize; i++)
            {
                string name = "";
                bool finep = false;
                double pzone_tcp = 0;
                double pzone_ori = 0;
                double pzone_eax = 0;
                double zone_ori = 0;
                double zone_leax = 0;
                double zone_reax = 0;

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

                // finep
                if (i < sizeValues[1])
                {
                    finep = fineps[i];
                    finepCounter++;
                }
                else
                {
                    finep = fineps[finepCounter];
                }

                // pzone_tcp
                if (i < sizeValues[2])
                {
                    pzone_tcp = pzone_tcps[i];
                    pzone_tcpCounter++;
                }
                else
                {
                    pzone_tcp = pzone_tcps[pzone_tcpCounter];
                }

                // pzone_ori
                if (i < sizeValues[3])
                {
                    pzone_ori = pzone_oris[i];
                    pzone_oriCounter++;
                }
                else
                {
                    pzone_ori = pzone_oris[pzone_oriCounter];
                }

                // pzone_eax
                if (i < sizeValues[4])
                {
                    pzone_eax = pzone_eaxs[i];
                    pzone_eaxCounter++;
                }
                else
                {
                    pzone_eax = pzone_eaxs[pzone_eaxCounter];
                }

                // zone_ori
                if (i < sizeValues[5])
                {
                    zone_ori = zone_oris[i];
                    zone_oriCounter++;
                }
                else
                {
                    zone_ori = zone_oris[zone_oriCounter];
                }

                // zone_leax
                if (i < sizeValues[6])
                {
                    zone_leax = zone_leaxs[i];
                    zone_leaxCounter++;
                }
                else
                {
                    zone_leax = zone_leaxs[zone_leaxCounter];
                }

                // zone_reax
                if (i < sizeValues[7])
                {
                    zone_reax = zone_reaxs[i];
                    zone_reaxCounter++;
                }
                else
                {
                    zone_reax = zone_reaxs[zone_reaxCounter];
                }

                // Construct zone data
                ZoneData zoneData = new ZoneData(name, finep, pzone_tcp, pzone_ori, pzone_eax, zone_ori, zone_leax, zone_reax);
                _zoneDatas.Add(zoneData);
            }

            // Sets Output
            DA.SetDataList(0, _zoneDatas);

            #region Object manager
            // Gets ObjectManager of this document
            _objectManager = DocumentManager.GetDocumentObjectManager(this.OnPingDocument());

            // Clears zoneDataNames
            for (int i = 0; i < _zoneDataNames.Count; i++)
            {
                _objectManager.ZoneDataNames.Remove(_zoneDataNames[i]);
            }
            _zoneDataNames.Clear();

            // Removes lastName from zoneDataNameList
            if (_objectManager.ZoneDataNames.Contains(_lastName))
            {
                _objectManager.ZoneDataNames.Remove(_lastName);
            }

            // Adds Component to ZoneDataByGuid Dictionary
            if (!_objectManager.OldZoneDatasByGuid.ContainsKey(this.InstanceGuid))
            {
                _objectManager.OldZoneDatasByGuid.Add(this.InstanceGuid, this);
            }

            // Checks if Zone Data name is already in use and counts duplicates
            #region Check name in object manager
            _namesUnique = true;
            for (int i = 0; i < names.Count; i++)
            {
                if (_objectManager.ZoneDataNames.Contains(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Zone Data Name already in use.");
                    _namesUnique = false;
                    _lastName = "";
                }
                else
                {
                    // Adds Zone Data Name to list
                    _zoneDataNames.Add(names[i]);
                    _objectManager.ZoneDataNames.Add(names[i]);

                    // Run SolveInstance on other Zone Data with no unique Name to check if their name is now available
                    foreach (KeyValuePair<Guid, OldZoneDataComponent> entry in _objectManager.OldZoneDatasByGuid)
                    {
                        if (entry.Value.LastName == "")
                        {
                            entry.Value.ExpireSolution(true);
                        }
                    }
                    _lastName = names[i];
                }

                // Check variable name: character limit
                if (HelperMethods.VariableExeedsCharacterLimit32(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Zone Data Name exceeds character limit of 32 characters.");
                    break;
                }

                // Check variable name: start with number is not allowed
                if (HelperMethods.VariableStartsWithNumber(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Zone Data Name starts with a number which is not allowed in RAPID Code.");
                    break;
                }
            }
            #endregion

            // Recognizes if Component is Deleted and removes it from Object Managers Zone Data and name list
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
                    for (int i = 0; i < _zoneDataNames.Count; i++)
                    {
                        _objectManager.ZoneDataNames.Remove(_zoneDataNames[i]);
                    }
                }
                _objectManager.OldZoneDatasByGuid.Remove(this.InstanceGuid);

                // Run SolveInstance on other Zone Data instances with no unique Name to check if their name is now available
                _objectManager.UpdateZoneDatas();
            }
        }

        /// <summary>
        /// The Zone Datas created by this component
        /// </summary>
        public List<ZoneData> ZoneDatas
        {
            get { return _zoneDatas; }
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
            { return Properties.Resources.ZoneData_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7C167248-772C-4DF4-B7A9-09A54046B38E"); }
        }
    }
}

