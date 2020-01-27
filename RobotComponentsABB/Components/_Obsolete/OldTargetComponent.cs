using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using RobotComponents.BaseClasses.Actions;

using RobotComponentsABB.Parameters;
using RobotComponentsABB.Utils;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.05.000 (January 2020)
// It is replaced with a new target component.

namespace RobotComponentsABB.Components.Obsolete
{
    /// <summary>
    /// OBSOLETE: RobotComponents Action : Target component. Will be removed in the future. An inherent from the GH_Component Class.
    /// </summary>
    public class OldTargetComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldTargetComponent()
          : base("Action: Target", "T",
              "OBSOLETE: Defines a target for an Action: Movement or Inverse Kinematics component."
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
            pManager.AddTextParameter("Name", "N", "Name as string", GH_ParamAccess.list, "defaultTar");
            pManager.AddPlaneParameter("Plane", "P", "Plane as Plane", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Axis Configuration", "AC", "Axis Configuration as int. This will modify the fourth value of the Robot Configuration Data in the RAPID Movement code line.", GH_ParamAccess.list, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new TargetParameter(), "Target", "T", "Resulting Target");  //Todo: beef this up to be more informative.
        }

        // Fields
        public List<string> targetNames = new List<string>();
        private string lastName = "";
        private bool namesUnique;
        private ObjectManager objectManager;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Clears targetNames
            for (int i = 0; i < targetNames.Count; i++)
            {
                objectManager.TargetNames.Remove(targetNames[i]);
            }
            targetNames.Clear();

            // Gets Document ID
            string documentGUID = DocumentManager.GetRobotComponentsDocumentID(this.OnPingDocument());

            // Checks if ObjectManager for this document already exists. If not it creates a new ObjectManager in DocumentManger Dictionary
            if (!DocumentManager.ObjectManagers.ContainsKey(documentGUID)) {
                DocumentManager.ObjectManagers.Add(documentGUID, new ObjectManager());
            }

            // Gets ObjectManager of this document
            objectManager = DocumentManager.ObjectManagers[documentGUID];

            // Adds Component to TargetByGuid Dictionary
            if (!objectManager.OldTargetsByGuid.ContainsKey(this.InstanceGuid))
            {
                objectManager.OldTargetsByGuid.Add(this.InstanceGuid, this);
            }

            // Removes lastName from targetNameList
            if (objectManager.TargetNames.Contains(lastName))
            {
                objectManager.TargetNames.Remove(lastName);
            }

            // Sets inputs and creates target
            Guid instanceGUID = this.InstanceGuid;
            List<string> names = new List<string>();
            List<Plane> planes = new List<Plane>();
            List<int> axisConfigs = new List<int>();

            // Catch the input data
            if (!DA.GetDataList(0, names)) { return; }
            if (!DA.GetDataList(1, planes)) { return; }
            if (!DA.GetDataList(2, axisConfigs)) { return; }

            // Get longest Input List
            int[] sizeValues = new int[3];
            sizeValues[0] = names.Count;
            sizeValues[1] = planes.Count;
            sizeValues[2] = axisConfigs.Count;
            int biggestSize = HelperMethods.GetBiggestValue(sizeValues);

            // Keeps track of used indicies
            int nameCounter = -1;
            int planesCounter = -1;
            int axisConfigCounter = -1;

            // Creates targets
            List<Target> targets = new List<Target>();
            for (int i = 0; i < biggestSize; i++)
            {
                string name = "";
                Plane plane = new Plane();
                int axisConfig = 0;

                if (i < names.Count)
                {
                    name = names[i];
                    nameCounter++;
                }
                else
                {
                    name = names[nameCounter] + "_" + (i - nameCounter);
                }

                if (i < planes.Count)
                {
                    plane = planes[i];
                    planesCounter++;
                }
                else
                {
                    plane = planes[planesCounter];
                }

                if (i < axisConfigs.Count)
                {
                    axisConfig = axisConfigs[i];
                    axisConfigCounter++;
                }
                else
                {
                    axisConfig = axisConfigs[axisConfigCounter];
                }

                Target target = new Target(name, plane, axisConfig);
                targets.Add(target);
            }

            // Checks if target name is already in use and counts duplicates
            #region NameCheck
            namesUnique = true;
            for (int i = 0; i < names.Count; i++)
            {
                if (objectManager.TargetNames.Contains(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Target Name already in use.");
                    namesUnique = false;
                    lastName = "";
                    break;
                }
                else
                {
                    // Adds Target Name to list
                    targetNames.Add(names[i]);
                    objectManager.TargetNames.Add(names[i]);

                    // Run SolveInstance on other Targets with no unique Name to check if their name is now available
                    foreach (KeyValuePair<Guid, OldTargetComponent> entry in objectManager.OldTargetsByGuid)
                    {
                        if (entry.Value.lastName == "")
                        {
                            entry.Value.ExpireSolution(true);
                        }
                    }

                    lastName = names[i];
                }

                // Checks if variable name exceeds max character limit for RAPID Code
                if (HelperMethods.VariableExeedsCharacterLimit32(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Target Name exceeds character limit of 32 characters.");
                    break;
                }

                // Checks if variable name starts with a number
                if (HelperMethods.VariableStartsWithNumber(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Target Name starts with a number which is not allowed in RAPID Code.");
                    break;
                }
            }
            #endregion

            // Sets Output
            DA.SetDataList(0, targets);

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
                if (namesUnique == true)
                {
                    for (int i = 0; i < targetNames.Count; i++)
                    {
                        objectManager.TargetNames.Remove(targetNames[i]);
                    }
                }
                objectManager.OldTargetsByGuid.Remove(this.InstanceGuid);

                // Run SolveInstance on other Targets with no unique Name to check if their name is now available
                foreach (KeyValuePair<Guid, OldTargetComponent> entry in objectManager.OldTargetsByGuid)
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
            get { return Properties.Resources.Target_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("E1AED1B2-3D79-41E7-AA12-C096F79FEE5E"); }
        }

    }
}
