// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
// RobotComponents Libs
using RobotComponents.BaseClasses.Definitions;
using RobotComponentsABB.Components.Obsolete;
using RobotComponentsABB.Components.CodeGeneration;
using RobotComponentsABB.Components.Definitions;

namespace RobotComponentsABB.Utils
{
    /// <summary>
    /// The ObjectManager keeps track of different variables to enable global funcionalities
    /// </summary>

    public class ObjectManager
    {
        #region fields
        // contains information on all targets in file to notify user about duplicates
        private Dictionary<Guid, AbsoluteJointMovementComponent> _jointTargetsByGuid;
        private Dictionary<Guid, OldTargetComponent> _oldTargetsByGuid;
        private Dictionary<Guid, TargetComponent> _targetsByGuid;
        private List<string> _targetNames;

        // contains information on all speedDatas in file to notify the user about duplicates
        private Dictionary<Guid, SpeedDataComponent> _speedDatasByGuid;
        private List<string> _speedDataNames;

        // contains information on all robot tools in file for code generation
        private Dictionary<Guid, RobotToolFromDataEulerComponent> _toolsEulerByGuid;
        private Dictionary<Guid, RobotToolFromPlanesComponent> _toolsPlanesByGuid;
        private Dictionary<Guid, RobotToolFromQuaternionComponent> _toolsQuaternionByGuid;
        private List<string> _toolNames;

        // contains information on all work objects in file for code generation
        private Dictionary<Guid, OldWorkObjectComponent> _oldWorkObjectsByGuid;
        private Dictionary<Guid, WorkObjectComponent> _workObjectsByGuid;
        private List<string> _workObjectNames;
        #endregion

        #region constructors
        /// <summary>
        /// Creates an empty object manager. 
        /// </summary>
        public ObjectManager()
        {
            _jointTargetsByGuid = new Dictionary<Guid, AbsoluteJointMovementComponent>();
            _oldTargetsByGuid = new Dictionary<Guid, OldTargetComponent>();
            _targetsByGuid = new Dictionary<Guid, TargetComponent>();
            _targetNames = new List<string>();

            _speedDatasByGuid = new Dictionary<Guid, SpeedDataComponent>();
            _speedDataNames = new List<string>();

            _toolsEulerByGuid = new Dictionary<Guid, RobotToolFromDataEulerComponent>();
            _toolsPlanesByGuid = new Dictionary<Guid, RobotToolFromPlanesComponent>();
            _toolsQuaternionByGuid = new Dictionary<Guid, RobotToolFromQuaternionComponent>();
            _toolNames = new List<string>() { "tool0" };

            _oldWorkObjectsByGuid = new Dictionary<Guid, OldWorkObjectComponent>();
            _workObjectsByGuid = new Dictionary<Guid, WorkObjectComponent>();
            _workObjectNames = new List<string>() { "wobj0" };
        }
        #endregion

        #region methods
        /// <summary>
        /// Gets all the robot tools that are stored in the object mananger
        /// </summary>
        /// <returns> A list with all the robot tools that are stored in the object mananger. </returns>
        public List<RobotTool> GetRobotTools()
        {
            // Enpty listt
            List<RobotTool> robotTools = new List<RobotTool>();

            // Add robot tools that are created from Euler data
            foreach (KeyValuePair<Guid, RobotToolFromDataEulerComponent> entry in _toolsEulerByGuid)
            {
                robotTools.Add(entry.Value.RobotTool);
            }

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

            // Add all the work objects from old component
            foreach (KeyValuePair<Guid, OldWorkObjectComponent> entry in _oldWorkObjectsByGuid)
            {
                for (int i = 0; i < entry.Value.WorkObjects.Count; i++)
                {
                    workObjects.Add(entry.Value.WorkObjects[i]);
                }
              
            }

            // Add all work objects from new component
            foreach (KeyValuePair<Guid, WorkObjectComponent> entry in _workObjectsByGuid)
            {
                for (int i = 0; i < entry.Value.WorkObjects.Count; i++)
                {
                    workObjects.Add(entry.Value.WorkObjects[i]);
                }

            }

            // Sort based on name
            workObjects = workObjects.OrderBy(x => x.Name).ToList();

            // Return
            return workObjects;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Dictionary with all the Target components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, AbsoluteJointMovementComponent> JointTargetsByGuid
        {
            get { return _jointTargetsByGuid; }
        }

        /// <summary>
        /// OBSOLETE: Used for old Target component. Will be removed in the future.
        /// </summary>
        public Dictionary<Guid, OldTargetComponent> OldTargetsByGuid
        {
            get { return _oldTargetsByGuid; }
        }

        /// <summary>
        /// Dictionary with all the Target components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, TargetComponent> TargetsByGuid
        {
            get { return _targetsByGuid; }
        }

        /// <summary>
        /// A list with all the unique 'Target names in this object manager
        /// </summary>
        public List<string> TargetNames
        {
            get { return _targetNames; }
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
        /// Dictionary with all the Robot Tools created from Euler data components used in this object manager. 
        /// The components are stored based on there unique GUID.
        /// </summary>
        public Dictionary<Guid, RobotToolFromDataEulerComponent> ToolsEulerByGuid
        {
            get { return _toolsEulerByGuid; }
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
        /// OBSOLETE: Used for old Work Object component.Will be removed in the future.
        /// </summary>
        public Dictionary<Guid, OldWorkObjectComponent> OldWorkObjectsByGuid 
        {
            get { return _oldWorkObjectsByGuid; }
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
        #endregion
    }
}
