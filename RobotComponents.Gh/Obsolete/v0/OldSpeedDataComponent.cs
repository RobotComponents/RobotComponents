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
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Gh.Parameters.Actions;
using RobotComponents.ABB.Gh.Utils;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.14.000
// It is replaced with a new component. 

namespace RobotComponents.ABB.Gh.Components.Obsolete
{
    /// <summary>
    /// RobotComponents Action : Speed Data component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldSpeedDataComponent : GH_Component, IObjectManager
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
        public OldSpeedDataComponent()
          : base("Speed Data", "SD", 
              "Defines a speed data declaration for Move components."
               + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.ABB.Utils.VersionNumbering.CurrentVersion,
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
            pManager.AddTextParameter("Name", "N", "Name of the Speed Data as text", GH_ParamAccess.list, "default_speed");
            pManager.AddNumberParameter("TCP Velocity", "vTCP", "TCP Velocity in mm/s as number", GH_ParamAccess.list);
            pManager.AddNumberParameter("ORI Velocity", "vORI", "Reorientation Velocity of the tool in degree/s as number", GH_ParamAccess.list, 500);
            pManager.AddNumberParameter("LEAX Velocity", "vLEAX", "Linear External Axes Velocity in mm/s", GH_ParamAccess.list, 5000);
            pManager.AddNumberParameter("REAX Velocity", "vREAX", "Reorientation of the External Rotational Axes in degrees/s", GH_ParamAccess.list, 1000);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_SpeedData(), "Speed Data", "SD", "Resulting Speed Data declaration");
        }

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
            int biggestSize = sizeValues.Max();

            // Keeps track of used indicies
            int nameCounter = -1;
            int v_tcpCounter = -1;
            int v_oriCounter = -1;
            int v_leaxCounter = -1;
            int v_reaxCounter = -1;

            // Clear list
            List<SpeedData> speedDatas = new List<SpeedData>();

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
                speedDatas.Add(speedData);
            }

            // Sets Output
            DA.SetDataList(0, speedDatas);

            #region Object manager
            _toRegister.Clear();
            _toRegister = speedDatas.ConvertAll(item => item.Name);

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

