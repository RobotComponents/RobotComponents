// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
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
    public class OldZoneDataComponent : GH_Component, IObjectManager
    {
        // Fields
        private List<string> _registered = new List<string>();
        private List<string> _toRegister = new List<string>();
        private ObjectManager _objectManager;
        private string _lastName = "";
        private bool _isUnique = true;

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
        protected override void RegisterInputParams(GH_InputParamManager pManager)
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
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_ZoneData(), "Zone Data", "ZD", "Resulting Zone Data declaration");
        }

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
            int biggestSize = sizeValues.Max();

            // Keeps track of used indicies
            int nameCounter = -1;
            int finepCounter = -1;
            int pzone_tcpCounter = -1;
            int pzone_oriCounter = -1;
            int pzone_eaxCounter = -1;
            int zone_oriCounter = -1;
            int zone_leaxCounter = -1;
            int zone_reaxCounter = -1;

            // Initialize list
            List<ZoneData> zoneDatas = new List<ZoneData>();

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
                zoneDatas.Add(zoneData);
            }

            // Sets Output
            DA.SetDataList(0, zoneDatas);

            #region Object manager
            _toRegister.Clear();
            _toRegister = zoneDatas.ConvertAll(item => item.Name);

            GH_Document doc = this.OnPingDocument();
            _objectManager = DocumentManager.GetDocumentObjectManager(doc);
            _objectManager.CheckVariableNames(this);

            if (doc != null)
            {
                doc.ObjectsDeleted += this.DocumentObjectsDeleted;
            }
            #endregion
        }

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
    }
}

