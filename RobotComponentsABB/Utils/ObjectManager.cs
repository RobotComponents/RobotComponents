using System;
using System.Collections.Generic;

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

        // global ToolString for Code Generation
        private string _currentTool; //tool0 = defaulTool
        #endregion

        #region constructors
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

            _currentTool = "tool0"; //tool0 = defaulTool
        }
        #endregion

        #region Properties
        public Dictionary<Guid, OldTargetComponent> OldTargetsByGuid
        {
            get { return _oldTargetsByGuid; }
        }

        public Dictionary<Guid, TargetComponent> TargetsByGuid
        {
            get { return _targetsByGuid; }
        }

        public List<string> TargetNames
        {
            get { return _targetNames; }
        }

        public Dictionary<Guid, SpeedDataComponent> SpeedDatasByGuid
        {
            get { return _speedDatasByGuid; }
        }

        public List<string> SpeedDataNames
        {
            get { return _speedDataNames; }
        }

        public Dictionary<Guid, RobotToolFromDataEulerComponent> ToolsEulerByGuid
        {
            get { return _toolsEulerByGuid; }
        }

        public Dictionary<Guid, RobotToolFromPlanesComponent> ToolsPlanesByGuid
        {
            get { return _toolsPlanesByGuid; }
        }

        public List<string> ToolNames
        {
            get { return _toolNames; }
        }

        public string CurrentTool
        {
            get { return _currentTool; }
            set { _currentTool = value; }
        }

        public Dictionary<Guid, WorkObjectComponent> WorkObjectsByGuid { get => _workObjectsByGuid; }
        public List<string> WorkObjectNames { get => _workObjectNames; }
        #endregion
    }
}
