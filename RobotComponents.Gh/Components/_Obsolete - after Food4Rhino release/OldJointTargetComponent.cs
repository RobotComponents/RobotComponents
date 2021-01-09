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
    /// RobotComponents Action : Joint Target component. An inherent from the GH_Component Class.
    /// </summary>
    [Obsolete("This component is obsolete and will be removed in the future.", false)]
    public class OldJointTargetComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public constructor without any arguments.
        /// Category represents the Tab in which the component will appear, Subcategory the panel. 
        /// If you use non-existing tab or panel names, new tabs/panels will automatically be created.
        /// </summary>
        public OldJointTargetComponent()
          : base("Joint Target", "JT",
              "Defines a Joint Target for a Move instruction."
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
            pManager.AddTextParameter("Name", "N", "Name as text", GH_ParamAccess.list, "defaultTar");
            pManager.AddParameter(new RobotJointPositionParameter(), "Robot Joint Position", "RJ", "Defines the robot joint position", GH_ParamAccess.list);
            pManager.AddParameter(new ExternalJointPositionParameter(), "External Joint Position", "EJ", "Defines the external axis joint position", GH_ParamAccess.list);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new JointTargetParameter(), "Joint Target", "JT", "The resulting Joint Target");
        }

        // Fields
        private readonly List<string> _targetNames = new List<string>();
        private string _lastName = "";
        private bool _namesUnique;
        private ObjectManager _objectManager;
        private List<JointTarget> _jointTargets = new List<JointTarget>();

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Variables
            List<string> names = new List<string>();
            List<RobotJointPosition> robotJointPositions = new List<RobotJointPosition>();
            List<ExternalJointPosition> externalJointPositions = new List<ExternalJointPosition>();

            // Catch input data
            if (!DA.GetDataList(0, names)) { return; }
            if (!DA.GetDataList(1, robotJointPositions)) { robotJointPositions = new List<RobotJointPosition>() { new RobotJointPosition() }; }
            if (!DA.GetDataList(2, externalJointPositions)) { externalJointPositions = new List<ExternalJointPosition>() { new ExternalJointPosition() }; }

            // Replace spaces
            names = HelperMethods.ReplaceSpacesAndRemoveNewLines(names);

            // Get longest input List
            int[] sizeValues = new int[3];
            sizeValues[0] = names.Count;
            sizeValues[1] = robotJointPositions.Count;
            sizeValues[2] = externalJointPositions.Count;

            int biggestSize = HelperMethods.GetBiggestValue(sizeValues);

            // Keeps track of used indicies
            int nameCounter = -1;
            int robPosCounter = -1;
            int extPosCounter = -1;

            // Clear list
            _jointTargets.Clear();

            // Creates the joint targets
            for (int i = 0; i < biggestSize; i++)
            {
                string name;
                RobotJointPosition robotJointPosition;
                ExternalJointPosition externalJointPosition;

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

                // Robot Joint Position counter
                if (i < sizeValues[1])
                {
                    robotJointPosition = robotJointPositions[i];
                    robPosCounter++;
                }
                else
                {
                    robotJointPosition = robotJointPositions[robPosCounter];
                }

                // External Joint Position counter
                if (i < sizeValues[2])
                {
                    externalJointPosition = externalJointPositions[i];
                    extPosCounter++;
                }
                else
                {
                    externalJointPosition = externalJointPositions[extPosCounter];
                }

                JointTarget jointTarget = new JointTarget(name, robotJointPosition, externalJointPosition);
                _jointTargets.Add(jointTarget);
            }

            // Sets Output
            DA.SetDataList(0, _jointTargets);

            #region Object manager
            // Gets ObjectManager of this document
            _objectManager = DocumentManager.GetDocumentObjectManager(this.OnPingDocument());

            // Clears targetNames
            for (int i = 0; i < _targetNames.Count; i++)
            {
                _objectManager.TargetNames.Remove(_targetNames[i]);
            }
            _targetNames.Clear();

            // Removes lastName from targetNameList
            if (_objectManager.TargetNames.Contains(_lastName))
            {
                _objectManager.TargetNames.Remove(_lastName);
            }

            // Adds Component to TargetByGuid Dictionary
            if (!_objectManager.OldJointTargetsByGuid3.ContainsKey(this.InstanceGuid))
            {
                _objectManager.OldJointTargetsByGuid3.Add(this.InstanceGuid, this);
            }

            // Checks if target name is already in use and counts duplicates
            #region Check name in object manager
            _namesUnique = true;
            for (int i = 0; i < names.Count; i++)
            {
                if (_objectManager.TargetNames.Contains(names[i]))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Target Name already in use.");
                    _namesUnique = false;
                    _lastName = "";
                    break;
                }
                else
                {
                    // Adds Target Name to list
                    _targetNames.Add(names[i]);
                    _objectManager.TargetNames.Add(names[i]);

                    // Run SolveInstance on other Targets with no unique Name to check if their name is now available
                    _objectManager.UpdateTargets();

                    _lastName = names[i];
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

            // Recognizes if Component is Deleted and removes it from Object Managers target and name list
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
                    for (int i = 0; i < _targetNames.Count; i++)
                    {
                        _objectManager.TargetNames.Remove(_targetNames[i]);
                    }
                }
                _objectManager.OldJointTargetsByGuid3.Remove(this.InstanceGuid);

                // Runs SolveInstance on all other Targets to check if robot tool names are unique.
                _objectManager.UpdateTargets();
            }
        }

        /// <summary>
        /// The Targets created by this component
        /// </summary>
        public List<JointTarget> JointTargets
        {
            get { return _jointTargets; }
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
            get { return Properties.Resources.JointTarget_Icon; }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6C77792A-AC5B-4199-8CD2-A36B79D5AA87"); }
        }

    }
}
