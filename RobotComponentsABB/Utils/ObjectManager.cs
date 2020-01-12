using System;
using System.Linq;
using System.Collections.Generic;

using RobotComponents.BaseClasses;
using RobotComponentsABB.Components;

namespace RobotComponentsABB
{
    /// <summary>
    /// The ObjectManager keeps track of different variables to enable global funcionalities
    /// </summary>

    public class ObjectManager
    {
        #region fields
        // contains information on all targets in file to notify user about duplicates
        private Dictionary<Guid, OldTargetComponent> _oldTargetsByGuid;
        private Dictionary<Guid, TargetComponent> _targetsByGuid;
        private List<string> _targetNames;

        // contains information on all speedDatas in file to notify the user about duplicates
        private Dictionary<Guid, SpeedDataComponent> _speedDatasByGuid;
        private List<string> _speedDataNames;

        // contains information on all robot tools in file for code generation
        private Dictionary<Guid, RobotToolFromDataEulerComponent> _toolsEulerByGuid;
        private Dictionary<Guid, RobotToolFromPlanesComponent> _toolsPlanesByGuid;
        private List<string> _toolNames;

        // contains information on all work objects in file for code generation
        private Dictionary<Guid, WorkObjectComponent> _workObjectsByGuid;
        private List<string> _workObjectNames;
        #endregion

        #region constructors
        /// <summary>
        /// Creates an empty object manager. 
        /// </summary>
        public ObjectManager()
        {
            _oldTargetsByGuid = new Dictionary<Guid, OldTargetComponent>();
            _targetsByGuid = new Dictionary<Guid, TargetComponent>();
            _targetNames = new List<string>();

            _speedDatasByGuid = new Dictionary<Guid, SpeedDataComponent>();
            _speedDataNames = new List<string>();

            _toolsEulerByGuid = new Dictionary<Guid, RobotToolFromDataEulerComponent>();
            _toolsPlanesByGuid = new Dictionary<Guid, RobotToolFromPlanesComponent>();
            _toolNames = new List<string>() { "tool0" };

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
                robotTools.Add(entry.Value.robotTool);
            }

            // Add robot tools that are created from Planes
            foreach (KeyValuePair<Guid, RobotToolFromPlanesComponent> entry in _toolsPlanesByGuid)
            {
                robotTools.Add(entry.Value.robotTool);
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

            // Add all the work objects
            foreach (KeyValuePair<Guid, WorkObjectComponent> entry in _workObjectsByGuid)
            {
                workObjects.Add(entry.Value.WorkObject.Duplicate());
            }

            // Sort based on name
            workObjects = workObjects.OrderBy(x => x.Name).ToList();

            // Return
            return workObjects;
        }
        #endregion

        #region Properties
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
        #endregion
    }
}
