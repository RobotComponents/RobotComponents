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
    public class OldJointTargetComponent : GH_Component, IObjectManager
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
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name as text", GH_ParamAccess.list, "defaultTar");
            pManager.AddParameter(new Param_RobotJointPosition(), "Robot Joint Position", "RJ", "Defines the robot joint position", GH_ParamAccess.list);
            pManager.AddParameter(new Param_ExternalJointPosition(), "External Joint Position", "EJ", "Defines the external axis joint position", GH_ParamAccess.list);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_JointTarget(), "Joint Target", "JT", "The resulting Joint Target");
        }

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

            // Initiate list
            List<JointTarget> jointTargets = new List<JointTarget>();

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
                jointTargets.Add(jointTarget);
            }

            // Sets Output
            DA.SetDataList(0, jointTargets);

            #region Object manager
            _toRegister.Clear();
            _toRegister = jointTargets.ConvertAll(item => item.Name);

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
