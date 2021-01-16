// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
// RobotComponents Libs
using RobotComponents.Actions;
using RobotComponents.Definitions;
using RobotComponents.Gh.Components.Obsolete;
using RobotComponents.Gh.Components.CodeGeneration;
using RobotComponents.Gh.Components.Definitions;

namespace RobotComponents.Gh.Utils
{
    /// <summary>
    /// The Object Manager keeps track of different variables to enable global funcionalities inside Grashopper
    /// </summary>

    public class ObjectManager
    {
        #region fields
        // RC document id
        private readonly string _id;

        // contains information on all targets in file to notify user about duplicates
        private Dictionary<Guid, JointTargetComponent> _jointTargetsByGuid;
        private Dictionary<Guid, RobotTargetComponent> _robotTargetsByGuid;
        private List<string> _targetNames;

        // contains information on all speedDatas in file to notify the user about duplicates
        private Dictionary<Guid, SpeedDataComponent> _speedDatasByGuid;
        private List<string> _speedDataNames;

        // contains information on all zoneDatas in file to notify the user about duplicates
        private Dictionary<Guid, ZoneDataComponent> _zoneDatasByGuid;
        private List<string> _zoneDataNames;

        // constains information on all external axes in file to notify the user about duplicates
        private Dictionary<Guid, ExternalLinearAxisComponent> _externalLinearAxesByGuid;
        private Dictionary<Guid, ExternalRotationalAxisComponent> _externalRotationalAxesByGuid;
        private List<string> _axisNames;

        // contains information on all robot tools in file for code generation
        private Dictionary<Guid, RobotToolFromPlanesComponent> _toolsPlanesByGuid;
        private Dictionary<Guid, RobotToolFromQuaternionComponent> _toolsQuaternionByGuid;
        private List<string> _toolNames;

        // contains information on all work objects in file for code generation
        private Dictionary<Guid, WorkObjectComponent> _workObjectsByGuid;
        private List<string> _workObjectNames;

        #region OBSOLETE
        private Dictionary<Guid, OldAbsoluteJointMovementComponent> _oldJointTargetsByGuid;
        private Dictionary<Guid, OldAbsoluteJointMovementComponent2> _oldJointTargetsByGuid2;
        private Dictionary<Guid, OldTargetComponent> _oldTargetsByGuid;
        private Dictionary<Guid, OldTargetComponent2> _oldTargetsByGuid2;
        private Dictionary<Guid, OldJointTargetComponent> _oldJointTargetsByGuid3;
        private Dictionary<Guid, OldRobotTargetComponent> _oldRobotTargetsByGuid;
        private Dictionary<Guid, OldSpeedDataComponent> _oldSpeedDatasByGuid;
        private Dictionary<Guid, OldZoneDataComponent> _oldZoneDatasByGuid;
        private Dictionary<Guid, OldExternalLinearAxisComponent2> _oldExternalLinearAxesByGuid2;
        private Dictionary<Guid, OldExternalRotationalAxisComponent> _oldExternalRotationalAxesByGuid;
        private Dictionary<Guid, OldRobotToolFromDataEulerComponent> _oldToolsEulerByGuid;
        private Dictionary<Guid, OldRobotToolFromPlanesComponent> _oldRobotToolFromPlanesGuid;
        private Dictionary<Guid, OldRobotToolFromQuaternionComponent> _oldToolsQuaternionByGuid;
        private Dictionary<Guid, OldRobotToolFromQuaternionComponent2> _oldToolsQuaternionByGuid2;
        private Dictionary<Guid, OldWorkObjectComponent> _oldWorkObjectsByGuid;
        private Dictionary<Guid, OldWorkObjectComponent2> _oldWorkObjectsByGuid2;
        #endregion

        #endregion

        #region constructors
        /// <summary>
        /// Creates an empty object manager. 
        /// </summary>
        public ObjectManager(string id)
        {
            _id = id;

            _jointTargetsByGuid = new Dictionary<Guid, JointTargetComponent>();
            _robotTargetsByGuid = new Dictionary<Guid, RobotTargetComponent>();
            _targetNames = new List<string>();

            _speedDatasByGuid = new Dictionary<Guid, SpeedDataComponent>();
            _speedDataNames = new List<string>();
            _speedDataNames.AddRange(SpeedData.ValidPredefinedNames.ToList());

            _zoneDatasByGuid = new Dictionary<Guid, ZoneDataComponent>();
            _zoneDataNames = new List<string>();
            _zoneDataNames.AddRange(ZoneData.ValidPredefinedNames.ToList());

            _externalLinearAxesByGuid = new Dictionary<Guid, ExternalLinearAxisComponent>();
            _externalRotationalAxesByGuid = new Dictionary<Guid, ExternalRotationalAxisComponent>();
            _axisNames = new List<string>();

            _toolsPlanesByGuid = new Dictionary<Guid, RobotToolFromPlanesComponent>();
            _toolsQuaternionByGuid = new Dictionary<Guid, RobotToolFromQuaternionComponent>();
            _toolNames = new List<string>() { "tool0" };

            _workObjectsByGuid = new Dictionary<Guid, WorkObjectComponent>();
            _workObjectNames = new List<string>() { "wobj0" };

            #region OBSOLETE
            _oldJointTargetsByGuid = new Dictionary<Guid, OldAbsoluteJointMovementComponent>();
            _oldJointTargetsByGuid2 = new Dictionary<Guid, OldAbsoluteJointMovementComponent2>();
            _oldTargetsByGuid = new Dictionary<Guid, OldTargetComponent>();
            _oldTargetsByGuid2 = new Dictionary<Guid, OldTargetComponent2>();
            _oldJointTargetsByGuid3 = new Dictionary<Guid, OldJointTargetComponent>();
            _oldRobotTargetsByGuid = new Dictionary<Guid, OldRobotTargetComponent>();
            _oldSpeedDatasByGuid = new Dictionary<Guid, OldSpeedDataComponent>();
            _oldZoneDatasByGuid = new Dictionary<Guid, OldZoneDataComponent>();
            _oldExternalLinearAxesByGuid2 = new Dictionary<Guid, OldExternalLinearAxisComponent2>();
            _oldExternalRotationalAxesByGuid = new Dictionary<Guid, OldExternalRotationalAxisComponent>();
            _oldToolsEulerByGuid = new Dictionary<Guid, OldRobotToolFromDataEulerComponent>();
            _oldRobotToolFromPlanesGuid = new Dictionary<Guid, OldRobotToolFromPlanesComponent>();
            _oldToolsQuaternionByGuid = new Dictionary<Guid, OldRobotToolFromQuaternionComponent>();
            _oldToolsQuaternionByGuid2 = new Dictionary<Guid, OldRobotToolFromQuaternionComponent2>();
            _oldWorkObjectsByGuid = new Dictionary<Guid, OldWorkObjectComponent>();
            _oldWorkObjectsByGuid2 = new Dictionary<Guid, OldWorkObjectComponent2>();
            #endregion
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return "Object Manager (" + _id + ")";
        }

        /// <summary>
        /// Gets all the robot tools that are stored in the object mananger
        /// </summary>
        /// <returns> A list with all the robot tools that are stored in the object mananger. </returns>
        public List<RobotTool> GetRobotTools()
        {
            // Enpty listt
            List<RobotTool> robotTools = new List<RobotTool>();

            // Add robot tools that are created from Planes
            foreach (KeyValuePair<Guid, RobotToolFromPlanesComponent> entry in _toolsPlanesByGuid)
            {
                robotTools.Add(entry.Value.RobotTool);
            }

            // Add robot tools that are created from Quaternion data
            foreach (KeyValuePair<Guid, RobotToolFromQuaternionComponent> entry in _toolsQuaternionByGuid)
            {
                robotTools.Add(entry.Value.RobotTool);
            }

            #region OBSOLETE
            // Add robot tools that are created from Euler data
            foreach (KeyValuePair<Guid, OldRobotToolFromDataEulerComponent> entry in _oldToolsEulerByGuid)
            {
                robotTools.Add(entry.Value.RobotTool);
            }

            // Add robot tools that are created from Planes
            foreach (KeyValuePair<Guid, OldRobotToolFromPlanesComponent> entry in _oldRobotToolFromPlanesGuid)
            {
                robotTools.Add(entry.Value.RobotTool);
            }

            // Add robot tools that are created from Quaternion data
            foreach (KeyValuePair<Guid, OldRobotToolFromQuaternionComponent> entry in _oldToolsQuaternionByGuid)
            {
                robotTools.Add(entry.Value.RobotTool);
            }

            // Add robot tools that are created from Quaternion data
            foreach (KeyValuePair<Guid, OldRobotToolFromQuaternionComponent2> entry in _oldToolsQuaternionByGuid2)
            {
                robotTools.Add(entry.Value.RobotTool);
            }
            #endregion

            // Sort based on name
            robotTools = robotTools.OrderBy(x => x.Name).ToList();

            // Return
            return robotTools;
        }

        /// <summary>
        /// Gets all the work objects that are stored in the object mananger
        /// </summary>
        /// <returns> A list with all the work objects that are stored in the object mananger. </returns>
        public List<WorkObject> GetWorkObjects()
        {
            // Empty list
            List<WorkObject> workObjects = new List<WorkObject>();

            // Add all work objects from new component
            foreach (KeyValuePair<Guid, WorkObjectComponent> entry in _workObjectsByGuid)
            {
                workObjects.AddRange(entry.Value.WorkObjects);
            }

            // Add all the work objects from old component
            foreach (KeyValuePair<Guid, OldWorkObjectComponent> entry in _oldWorkObjectsByGuid)
            {
                workObjects.AddRange(entry.Value.WorkObjects);
            }

            // Add all the work objects from old component 2
            foreach (KeyValuePair<Guid, OldWorkObjectComponent2> entry in _oldWorkObjectsByGuid2)
            {
                workObjects.AddRange(entry.Value.WorkObjects);
            }

            // Sort based on name
            workObjects = workObjects.OrderBy(x => x.Name).ToList();

            // Return
            return workObjects;
        }

        /// <summary>
        /// Gets all the external axes that are stored in the object mananger
        /// </summary>
        /// <returns> A list with all external axes that are stored in the object mananger. </returns>
        public List<ExternalAxis> GetExternalAxes()
        {
            // Empty list
            List<ExternalAxis> externalAxes = new List<ExternalAxis>();

            // Adds all the external linear axes as external axis
            foreach (KeyValuePair<Guid, ExternalLinearAxisComponent> entry in _externalLinearAxesByGuid)
            {
                externalAxes.Add(entry.Value.ExternalAxis);
            }

            // Adds all the external rotational axes as external axis
            foreach (KeyValuePair<Guid, ExternalRotationalAxisComponent> entry in _externalRotationalAxesByGuid)
            {
                externalAxes.Add(entry.Value.ExternalAxis);
            }

            #region OBSOLETE components
            // Adds all the external linear axes as external axis
            foreach (KeyValuePair<Guid, OldExternalLinearAxisComponent2> entry in _oldExternalLinearAxesByGuid2)
            {
                externalAxes.Add(entry.Value.ExternalAxis);
            }

            // Adds all the external rotational axes as external axis
            foreach (KeyValuePair<Guid, OldExternalRotationalAxisComponent> entry in _oldExternalRotationalAxesByGuid)
            {
                externalAxes.Add(entry.Value.ExternalAxis);
            }
            #endregion

            // Sort based on name
            externalAxes = externalAxes.OrderBy(x => x.Name).ToList();

            // Return
            return externalAxes;
        }

        /// <summary>
        /// Gets all the targets that are stored in the object mananger
        /// </summary>
        /// <returns> A list with all the targets that are stored in the object mananger. </returns>
        public List<ITarget> GetTargets()
        {
            // Empty list
            List<ITarget> targets = new List<ITarget>();

            // Add alltargets
            targets.AddRange(this.GetRobotTargets());
            targets.AddRange(this.GetJointTargets());

            // Sort based on name
            targets = targets.OrderBy(x => x.Name).ToList();

            // Return
            return targets;
        }

        /// <summary>
        /// Gets all the robot targets that are stored in the object mananger
        /// </summary>
        /// <returns> A list with all the robot targets that are stored in the object mananger. </returns>
        public List<RobotTarget> GetRobotTargets()
        {
            // Empty list
            List<RobotTarget> targets = new List<RobotTarget>();

            // Add all the robot targets
            foreach (KeyValuePair<Guid, RobotTargetComponent> entry in _robotTargetsByGuid)
            {
                targets.AddRange(entry.Value.RobotTargets);
            }

            #region OBSOLETE components
            foreach (KeyValuePair<Guid, OldTargetComponent> entry in _oldTargetsByGuid)
            {
                targets.AddRange(entry.Value.Targets);
            }

            foreach (KeyValuePair<Guid, OldTargetComponent2> entry in _oldTargetsByGuid2)
            {
                targets.AddRange(entry.Value.Targets);
            }

            // Add all the robot targets
            foreach (KeyValuePair<Guid, OldRobotTargetComponent> entry in _oldRobotTargetsByGuid)
            {
                targets.AddRange(entry.Value.RobotTargets);
            }
            #endregion

            // Sort based on name
            targets = targets.OrderBy(x => x.Name).ToList();

            // Return
            return targets;
        }

        /// <summary>
        /// Gets all the robot targets that are stored in the object mananger
        /// </summary>
        /// <returns> A list with all the robot targets that are stored in the object mananger. </returns>
        public List<JointTarget> GetJointTargets()
        {
            // Empty list
            List<JointTarget> targets = new List<JointTarget>();

            // Add all the joint targets
            foreach (KeyValuePair<Guid, JointTargetComponent> entry in _jointTargetsByGuid)
            {
                targets.AddRange(entry.Value.JointTargets);
            }

            #region OBSOLETE components
            foreach (KeyValuePair<Guid, OldAbsoluteJointMovementComponent> entry in _oldJointTargetsByGuid)
            {
                for (int i = 0; i < entry.Value.AbsoluteJointMovements.Count; i++)
                {
                    targets.Add(entry.Value.AbsoluteJointMovements[i].ConvertToJointTarget());
                }
            }

            foreach (KeyValuePair<Guid, OldAbsoluteJointMovementComponent2> entry in _oldJointTargetsByGuid2)
            {
                for (int i = 0; i < entry.Value.AbsoluteJointMovements.Count; i++)
                {
                    targets.Add(entry.Value.AbsoluteJointMovements[i].ConvertToJointTarget());
                }
            }

            foreach (KeyValuePair<Guid, OldJointTargetComponent> entry in _oldJointTargetsByGuid3)
            {
                targets.AddRange(entry.Value.JointTargets);
            }
            #endregion

            // Sort based on name
            targets = targets.OrderBy(x => x.Name).ToList();

            // Return
            return targets;
        }

        /// <summary>
        /// Gets all the speed datas that are stored in the object mananger
        /// </summary>
        /// <returns> A list with all the speed datas that are stored in the object mananger. </returns>
        public List<SpeedData> GetSpeedDatas()
        {
            // Empty list
            List<SpeedData> speeddatas = new List<SpeedData>();

            // Add all the speed datas
            foreach (KeyValuePair<Guid, SpeedDataComponent> entry in _speedDatasByGuid)
            {
                speeddatas.AddRange(entry.Value.SpeedDatas);
            }

            #region OBSOLETE components
            foreach (KeyValuePair<Guid, OldSpeedDataComponent> entry in _oldSpeedDatasByGuid)
            {
                speeddatas.AddRange(entry.Value.SpeedDatas);
            }
            #endregion

            // Sort based on name
            speeddatas = speeddatas.OrderBy(x => x.Name).ToList();

            // Return
            return speeddatas;
        }

        /// <summary>
        /// Gets all the zone datas that are stored in the object mananger
        /// </summary>
        /// <returns> A list with all the zone datas that are stored in the object mananger. </returns>
        public List<ZoneData> GetZoneDatas()
        {
            // Empty list
            List<ZoneData> zonedatas = new List<ZoneData>();

            // Add all the zone datas
            foreach (KeyValuePair<Guid, ZoneDataComponent> entry in _zoneDatasByGuid)
            {
                zonedatas.AddRange(entry.Value.ZoneDatas);
            }

            #region OBSOLETE components
            foreach (KeyValuePair<Guid, OldZoneDataComponent> entry in _oldZoneDatasByGuid)
            {
                zonedatas.AddRange(entry.Value.ZoneDatas);
            }
            #endregion

            // Sort based on name
            zonedatas = zonedatas.OrderBy(x => x.Name).ToList();

            // Return
            return zonedatas;
        }

        /// <summary>
        /// Runs SolveInstance on all other Robot Tools to check if tool names are unique.
        /// </summary>
        public void UpdateRobotTools()
        {
            // Run SolveInstance on other Tools with no unique Name to check if their name is now available
            foreach (KeyValuePair<Guid, RobotToolFromPlanesComponent> entry in ToolsPlanesByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }
            foreach (KeyValuePair<Guid, RobotToolFromQuaternionComponent> entry in ToolsQuaternionByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            // Run SolveInstance on other obsolete Tools with no unique Name to check if their name is now available
            foreach (KeyValuePair<Guid, OldRobotToolFromDataEulerComponent> entry in OldToolsEulerByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            foreach (KeyValuePair<Guid, OldRobotToolFromPlanesComponent> entry in OldRobotToolFromPlanesGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            foreach (KeyValuePair<Guid, OldRobotToolFromQuaternionComponent> entry in OldToolsQuaternionByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            foreach (KeyValuePair<Guid, OldRobotToolFromQuaternionComponent2> entry in OldToolsQuaternionByGuid2)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }
        }

        /// <summary>
        /// Runs SolveInstance on all other Targets to check if target names are unique.
        /// </summary>
        public void UpdateTargets()
        {
            // Run SolveInstance on other Targets with no unique Name to check if their name is now available
            foreach (KeyValuePair<Guid, RobotTargetComponent> entry in RobotTargetsByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            foreach (KeyValuePair<Guid, JointTargetComponent> entry in JointTargetsByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            // Run SolveInstance on other obsolete Targets with no unique Name to check if their name is now available
            foreach (KeyValuePair<Guid, OldTargetComponent> entry in OldTargetsByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            foreach (KeyValuePair<Guid, OldTargetComponent2> entry in OldTargetsByGuid2)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            foreach (KeyValuePair<Guid, OldAbsoluteJointMovementComponent> entry in OldJointTargetsByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            foreach (KeyValuePair<Guid, OldAbsoluteJointMovementComponent2> entry in OldJointTargetsByGuid2)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            foreach (KeyValuePair<Guid, OldRobotTargetComponent> entry in OldRobotTargetsByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            foreach (KeyValuePair<Guid, OldJointTargetComponent> entry in OldJointTargetsByGuid3)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }
        }

        /// <summary>
        /// Runs SolveInstance on all other Speed Datas to check if speed data names are unique.
        /// </summary>
        public void UpdateSpeedDatas()
        {
            // Run SolveInstance on other Speed Data with no unique Name to check if their name is now available
            foreach (KeyValuePair<Guid, SpeedDataComponent> entry in SpeedDatasByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            foreach (KeyValuePair<Guid, OldSpeedDataComponent> entry in OldSpeedDatasByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }
        }

        /// <summary>
        /// Runs SolveInstance on all other WorkObjects to check if work object names are unique.
        /// </summary>
        public void UpdateWorkObjects()
        {
            // Run SolveInstance on other Targets with no unique Name to check if their name is now available
            foreach (KeyValuePair<Guid, WorkObjectComponent> entry in WorkObjectsByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            // Run SolveInstance on other obsolete Work Objects with no unique Name to check if their name is now available
            foreach (KeyValuePair<Guid, OldWorkObjectComponent> entry in OldWorkObjectsByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            // Run SolveInstance on other obsolete Work Objects with no unique Name to check if their name is now available
            foreach (KeyValuePair<Guid, OldWorkObjectComponent2> entry in OldWorkObjectsByGuid2)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }
        }

        /// <summary>
        /// Runs SolveInstance on all other ZoneDatas to check if zone data names are unique.
        /// </summary>
        public void UpdateZoneDatas()
        {
            // Run SolveInstance on other Zone Data instances with no unique Name to check if their name is now available
            foreach (KeyValuePair<Guid, ZoneDataComponent> entry in ZoneDatasByGuid)
            {
                if (entry.Value.LastName == "") 
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            foreach (KeyValuePair<Guid, OldZoneDataComponent> entry in OldZoneDatasByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }
        }

        /// <summary>
        /// Runs SolveInstance on all other ExternalAxis components to check if external axis names are unique.
        /// </summary>
        public void UpdateExternalAxis()
        {
            // Run SolveInstance on other External Axes with no unique Name to check if their name is now available
            foreach (KeyValuePair<Guid, ExternalLinearAxisComponent> entry in ExternalLinearAxesByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }
            foreach (KeyValuePair<Guid, ExternalRotationalAxisComponent> entry in ExternalRotationalAxesByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }

            // Run SolveInstance on other obsolete External Axes with no unique Name to check if their name is now available
            foreach (KeyValuePair<Guid, OldExternalLinearAxisComponent2> entry in OldExternalLinearAxesByGuid2)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }
            foreach (KeyValuePair<Guid, OldExternalRotationalAxisComponent> entry in OldExternalRotationalAxesByGuid)
            {
                if (entry.Value.LastName == "")
                {
                    entry.Value.ExpireSolution(true);
                }
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the Robot Components document ID
        /// </summary>
        public string ID
        {
            get { return _id; }
        }

        /// <summary>
        /// Dictionary with all the Robot Target components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, RobotTargetComponent> RobotTargetsByGuid
        {
            get { return _robotTargetsByGuid; }
        }

        /// <summary>
        /// Dictionary with all the Joint Target components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, JointTargetComponent> JointTargetsByGuid
        {
            get { return _jointTargetsByGuid; }
        }

        /// <summary>
        /// A list with all the unique 'Target names in this object manager
        /// </summary>
        public List<string> TargetNames
        {
            get { return _targetNames; }
        }

        /// <summary>
        /// Dictionary with all the External Linear Axis components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, ExternalLinearAxisComponent> ExternalLinearAxesByGuid
        {
            get { return _externalLinearAxesByGuid; }
        }

        /// <summary>
        /// Dictionary with all the External Rotational Axis components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, ExternalRotationalAxisComponent> ExternalRotationalAxesByGuid
        {
            get { return _externalRotationalAxesByGuid; }
        }

        /// <summary>
        /// A list with all the unique External Axis names in this object manager
        /// </summary>
        public List<string> ExternalAxisNames
        {
            get { return _axisNames; }
        }

        /// <summary>
        /// Dictionary with all the Speed Data components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, SpeedDataComponent> SpeedDatasByGuid
        {
            get { return _speedDatasByGuid; }
        }

        /// <summary>
        /// A list with all the unique Speed Data names in this object manager
        /// </summary>
        public List<string> SpeedDataNames
        {
            get { return _speedDataNames; }
        }

        /// <summary>
        /// Dictionary with all the Zone Data components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, ZoneDataComponent> ZoneDatasByGuid
        {
            get { return _zoneDatasByGuid; }
        }

        /// <summary>
        /// A list with all the unique Zone Data names in this object manager
        /// </summary>
        public List<string> ZoneDataNames
        {
            get { return _zoneDataNames; }
        }

        /// <summary>
        /// Dictionary with all the Robot Tools created from Planes components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, RobotToolFromPlanesComponent> ToolsPlanesByGuid
        {
            get { return _toolsPlanesByGuid; }
        }

        /// <summary>
        /// Dictionary with all the Robot Tools created from Quaternion data components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, RobotToolFromQuaternionComponent> ToolsQuaternionByGuid
        {
            get { return _toolsQuaternionByGuid; }
        }

        /// <summary>
        /// A list with all the unique Robot Tool names in this object manager
        /// </summary>
        public List<string> ToolNames
        {
            get { return _toolNames; }
        }

        /// <summary>
        /// Dictionary with all the Work Object components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, WorkObjectComponent> WorkObjectsByGuid
        {
            get { return _workObjectsByGuid; }
        }

        /// <summary>
        /// A list with all the unique Work Object names in this object manager
        /// </summary>
        public List<string> WorkObjectNames 
        {
            get { return _workObjectNames; }
        }

        #region OBSOLETE
        /// <summary>
        /// OBSOLETE: Used for old Absolute Joint Movement component. Will be removed in the future.
        /// </summary>
        public Dictionary<Guid, OldAbsoluteJointMovementComponent> OldJointTargetsByGuid
        {
            get { return _oldJointTargetsByGuid; }
        }

        /// <summary>
        /// OBSOLETE: Used for old Absolute Joint Movement component. Will be removed in the future.
        /// </summary>
        public Dictionary<Guid, OldAbsoluteJointMovementComponent2> OldJointTargetsByGuid2
        {
            get { return _oldJointTargetsByGuid2; }
        }

        /// <summary>
        /// OBSOLETE: Used for old Target component. Will be removed in the future.
        /// </summary>
        public Dictionary<Guid, OldTargetComponent> OldTargetsByGuid
        {
            get { return _oldTargetsByGuid; }
        }

        /// <summary>
        /// OBSOLETE: Used for old Target component. Will be removed in the future.
        /// </summary>
        public Dictionary<Guid, OldTargetComponent2> OldTargetsByGuid2
        {
            get { return _oldTargetsByGuid2; }
        }

        /// <summary>
        /// OBSOLETE: Used for old Robot Target component. Will be removed in the future.
        /// </summary>
        public Dictionary<Guid, OldRobotTargetComponent> OldRobotTargetsByGuid
        {
            get { return _oldRobotTargetsByGuid; }
        }

        /// <summary>
        /// OBSOLETE: Used for old Joint Target component. Will be removed in the future.
        /// </summary>
        public Dictionary<Guid, OldJointTargetComponent> OldJointTargetsByGuid3
        {
            get { return _oldJointTargetsByGuid3; }
        }

        /// <summary>
        /// OBSOLETE: Used for old Speed Data component. Will be removed in the future
        /// </summary>
        public Dictionary<Guid, OldSpeedDataComponent> OldSpeedDatasByGuid
        {
            get { return _oldSpeedDatasByGuid; }
        }

        /// <summary>
        /// OBSOLETE: Used for old Zone Data component. Will be removed in the future
        /// </summary>
        public Dictionary<Guid, OldZoneDataComponent> OldZoneDatasByGuid
        {
            get { return _oldZoneDatasByGuid; }
        }

        /// <summary>
        /// OBSOLETE: Used for old External Linear Axis component. Will be removed in the future. 
        /// </summary>
        public Dictionary<Guid, OldExternalLinearAxisComponent2> OldExternalLinearAxesByGuid2
        {
            get { return _oldExternalLinearAxesByGuid2; }
        }

        /// <summary>
        /// OBSOLETE: Used for old External Rotational Axis component. Will be removed in the future. 
        /// </summary>
        public Dictionary<Guid, OldExternalRotationalAxisComponent> OldExternalRotationalAxesByGuid
        {
            get { return _oldExternalRotationalAxesByGuid; }
        }

        /// <summary>
        /// OBSOLETE: Used for old Tool components. Will be removed in the future.
        /// </summary>
        public Dictionary<Guid, OldRobotToolFromDataEulerComponent> OldToolsEulerByGuid
        {
            get { return _oldToolsEulerByGuid; }
        }

        public Dictionary<Guid, OldRobotToolFromPlanesComponent> OldRobotToolFromPlanesGuid
        {
            get { return _oldRobotToolFromPlanesGuid; }
        }

        public Dictionary<Guid, OldRobotToolFromQuaternionComponent> OldToolsQuaternionByGuid
        {
            get { return _oldToolsQuaternionByGuid; }
        }

        public Dictionary<Guid, OldRobotToolFromQuaternionComponent2> OldToolsQuaternionByGuid2 
        {
            get { return _oldToolsQuaternionByGuid2; }
        }

        /// <summary>
        /// OBSOLETE: Used for old Work Object component.Will be removed in the future.
        /// </summary>
        public Dictionary<Guid, OldWorkObjectComponent> OldWorkObjectsByGuid
        {
            get { return _oldWorkObjectsByGuid; }
        }

        /// <summary>
        /// OBSOLETE: Used for old Work Object component.Will be removed in the future.
        /// </summary>
        public Dictionary<Guid, OldWorkObjectComponent2> OldWorkObjectsByGuid2
        {
            get { return _oldWorkObjectsByGuid2; }
        }
        #endregion

        #endregion
    }
}
