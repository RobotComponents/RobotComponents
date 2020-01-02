using System;
using System.Collections.Generic;

using Grasshopper.Kernel;

using RobotComponents.BaseClasses;

using RobotComponentsABB.Parameters;
using RobotComponentsABB.Utils;

namespace RobotComponentsABB.Components
{
    /// <summary>
    /// RobotComponents Action : Speed Data component. An inherent from the GH_Component Class.
    /// </summary>
    public class SpeedDataComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public SpeedDataComponent()
          : base("Action: Speed Data", "SD",
              "Defines a speed data for robot movements."
                + System.Environment.NewLine +
                "RobotComponents: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "Code Generation")
        {
        }

        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name as string", GH_ParamAccess.list, "default_speed");
            pManager.AddNumberParameter("TCP Velocity", "vTCP", "tcp velocity in mm/s as integer", GH_ParamAccess.list);
            pManager.AddNumberParameter("ORI Velocity", "vORI", "reorientation of the tool in degree/s as integer", GH_ParamAccess.list, 500);
            pManager.AddNumberParameter("LEAX Velocity", "vLEAX", "linear external axes velocity in mm/s", GH_ParamAccess.list, 5000);
            pManager.AddNumberParameter("REAX Velocity", "vREAX", "reorientation of the external rotational axes in degrees/s", GH_ParamAccess.list, 1000);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new SpeedDataParameter(), "Speed Data", "SD", "Resulting Speed Data");
        }

        // Global component variables
        private List<string> speedDataNames = new List<string>();
        private string lastName = "";
        private bool namesUnique;
        private ObjectManager objectManager;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Clears speedDataNames
            for (int i = 0; i < speedDataNames.Count; i++)
            {
                objectManager.SpeedDataNames.Remove(speedDataNames[i]);
            }
            speedDataNames.Clear();

            // Gets Document ID
            string documentID = DocumentManager.GetRobotComponentsDocumentID(this.OnPingDocument());

            // Checks if ObjectManager for this document already exists. If not it creates a new one
            if (!DocumentManager.ObjectManagers.ContainsKey(documentID))
            {
                DocumentManager.ObjectManagers.Add(documentID, new ObjectManager());
            }

            // Gets ObjectManager of this document
            objectManager = DocumentManager.ObjectManagers[documentID];

            // Adds Component to SpeedDataByGuid Dictionary
            if (!objectManager.SpeedDatasByGuid.ContainsKey(this.InstanceGuid))
            {
                objectManager.SpeedDatasByGuid.Add(this.InstanceGuid, this);
            }

            // Removes lastName from speedDataNameList
            if (objectManager.SpeedDataNames.Contains(lastName))
            {
                objectManager.SpeedDataNames.Remove(lastName);
            }

            // Sets inputs and creates target
            Guid instanceGUID = this.InstanceGuid;
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

            // Creates speed Datas
            List<SpeedData> speedDatas = new List<SpeedData>();
            for (int i = 0; i < biggestSize; i++)
            {
                string name = "";
                double v_tcp = 0;
                double v_ori = 0;
                double v_leax = 0;
                double v_reax = 0;

                if(i < names.Count)
                {
                    name = names[i];
                    nameCounter++;
                }
                else
                {
                    name = names[nameCounter] + "_" + (i - nameCounter);
                }

                if (i < v_tcps.Count)
                {
                    v_tcp = v_tcps[i];
                    v_tcpCounter++;
                }
                else
                {
                    v_tcp = v_tcps[v_tcpCounter];
                }

                if (i < v_oris.Count)
                {
                    v_ori = v_oris[i];
                    v_oriCounter++;
                }
                else
                {
                    v_ori = v_oris[v_oriCounter];
                }

                if (i < v_leaxs.Count)
                {
                    v_leax = v_leaxs[i];
                    v_leaxCounter++;
                }
                else
                {
                    v_leax = v_leaxs[v_leaxCounter];
                }

                if (i < v_reaxs.Count)
                {
                    v_reax = v_reaxs[i];
                    v_reaxCounter++;
                }
                else
                {
                    v_reax = v_reaxs[v_reaxCounter];
                }

                SpeedData speedData = new SpeedData(name, v_tcp, v_ori, v_leax, v_reax);
                speedDatas.Add(speedData);
            }

            namesUnique = true;
            for (int i = 0; i < names.Count; i++)
            {
                // Checks if speed Data name is already in use and counts duplicates
                #region NameCheck
                if (objectManager.SpeedDataNames.Contains(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Speed Data Name already in use.");
                    namesUnique = false;
                    lastName = "";
                }
                else
                {
                    // Adds Speed Data Name to list
                    speedDataNames.Add(names[i]);
                    objectManager.SpeedDataNames.Add(names[i]);

                    // Run SolveInstance on other Speed Data with no unique Name to check if their name is now available
                    foreach (KeyValuePair<Guid, SpeedDataComponent> entry in objectManager.SpeedDatasByGuid)
                    {
                        if (entry.Value.lastName == "")
                        {
                            entry.Value.ExpireSolution(true);
                        }
                    }
                    lastName = names[i];
                }

                // Checks if Speed Data Name exceeds max character limit for RAPID Code
                if (HelperMethods.VariableExeedsCharacterLimit32(speedDatas[i].Name))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Speed Data Name exceeds character limit of 32 characters.");
                }

                // Checks if variable name starts with a number
                if (HelperMethods.VariableStartsWithNumber(speedDatas[i].Name))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Speed Data Name starts with a number which is not allowed in RAPID Code.");
                }
            }
            #endregion

            // Sets Output
            DA.SetDataList(0, speedDatas);

            // Recognizes if Component is Deleted and removes it from Object Managers speed Data and name list
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
                if (namesUnique == true)
                {
                    for (int i = 0; i < speedDataNames.Count; i++)
                    {
                        objectManager.SpeedDataNames.Remove(speedDataNames[i]);
                    }
                }
                objectManager.SpeedDatasByGuid.Remove(this.InstanceGuid);

                // Run SolveInstance on other Speed Data instances with no unique Name to check if their name is now available
                foreach (KeyValuePair<Guid, SpeedDataComponent> entry in objectManager.SpeedDatasByGuid)
                {
                    if (entry.Value.lastName == "")
                    {
                        entry.Value.ExpireSolution(true);
                    }
                }
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

    }
}

