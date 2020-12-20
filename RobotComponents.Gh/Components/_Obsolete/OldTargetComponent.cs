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
using RobotComponents.Actions;
using RobotComponents.Gh.Parameters.Actions;
using RobotComponents.Gh.Utils;

// This component is OBSOLETE!
// It is OBSOLETE since version 0.05.000 (January 2020)
// It is replaced with a new target component.

namespace RobotComponents.Gh.Components.Obsolete
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
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
              "RobotComponents", "RAPID Generation")
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
        public List<RobotTarget> _targets = new List<RobotTarget>();
        private string lastName = "";
        private bool namesUnique;
        private ObjectManager objectManager;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Warning that this component is OBSOLETE
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is OBSOLETE and will be removed " +
                "in the future. Remove this component from your canvas and replace it by picking the new component " +
                "from the ribbon.");

            // Gets the Object Manager of this document
            objectManager = DocumentManager.GetDocumentObjectManager(this.OnPingDocument());

            // Clears targetNames
            for (int i = 0; i < targetNames.Count; i++)
            {
                objectManager.TargetNames.Remove(targetNames[i]);
            }
            targetNames.Clear();

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

            // Clear list
            _targets.Clear();

            // Creates targets
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

                RobotTarget target = new RobotTarget(name, plane, axisConfig);
                _targets.Add(target);
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
                    objectManager.UpdateTargets();

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
            DA.SetDataList(0, _targets);

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

                /// Runs SolveInstance on all other Targets to check if robot tool names are unique.
                objectManager.UpdateTargets();
            }
        }

        /// <summary>
        /// The Targets created by this component
        /// </summary>
        public List<RobotTarget> Targets
        {
            get { return _targets; }
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

        public string LastName { get => lastName; }
    }
}
