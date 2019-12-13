using System;
using System.Collections.Generic;

using RobotComponents.Components;

namespace RobotComponents
{
    /// <summary>
    /// The ObjectManager keeps track of different variables to enable global funcionalities
    /// </summary>

    public class ObjectManager
    {
        #region fields
        // contains information on all targets in file to notify user about duplicates
        private Dictionary<Guid, AdvancedTargetComponent> _advancedTargetsByGuid;
        private Dictionary<Guid, TargetComponent> _targetsByGuid;
        private List<string> _targetNames;

        // contains information on all speedDatas in file to notify the user about duplicates
        private Dictionary<Guid, SpeedDataComponent> _speedDatasByGuid;
        private List<string> _speedDataNames;

        // contains information on all tools in file for code generation
        private Dictionary<Guid, RobotToolFromDataEulerComponent> _toolsEulerByGuid;
        private Dictionary<Guid, RobotToolFromPlanesComponent> _toolsPlanesByGuid;
        private List<string> _toolNames;

        // global ToolString for Code Generation
        private string _currentTool; //tool0 = defaulTool
        #endregion

        #region constructors
        public ObjectManager()
        {
            _advancedTargetsByGuid = new Dictionary<Guid, AdvancedTargetComponent>();
            _targetsByGuid = new Dictionary<Guid, TargetComponent>();
            _targetNames = new List<string>();

            _speedDatasByGuid = new Dictionary<Guid, SpeedDataComponent>();
            _speedDataNames = new List<string>();

            _toolsEulerByGuid = new Dictionary<Guid, RobotToolFromDataEulerComponent>();
            _toolsPlanesByGuid = new Dictionary<Guid, RobotToolFromPlanesComponent>();
            _toolNames = new List<string>() { "tool0" };

            _currentTool = "tool0"; //tool0 = defaulTool
        }
        #endregion

        #region Properties
        public Dictionary<Guid, AdvancedTargetComponent> AdvancedTargetsByGuid { get => _advancedTargetsByGuid; set => _advancedTargetsByGuid = value; }
        public Dictionary<Guid, TargetComponent> TargetsByGuid { get => _targetsByGuid; set => _targetsByGuid = value; }
        public List<string> TargetNames { get => _targetNames; set => _targetNames = value; }
        public Dictionary<Guid, SpeedDataComponent> SpeedDatasByGuid { get => _speedDatasByGuid; set => _speedDatasByGuid = value; }
        public List<string> SpeedDataNames { get => _speedDataNames; set => _speedDataNames = value; }
        public Dictionary<Guid, RobotToolFromDataEulerComponent> ToolsEulerByGuid { get => _toolsEulerByGuid; set => _toolsEulerByGuid = value; }
        public Dictionary<Guid, RobotToolFromPlanesComponent> ToolsPlanesByGuid { get => _toolsPlanesByGuid; set => _toolsPlanesByGuid = value; }
        public List<string> ToolNames { get => _toolNames; set => _toolNames = value; }
        public string CurrentTool { get => _currentTool; set => _currentTool = value; }
        #endregion
    }
}
