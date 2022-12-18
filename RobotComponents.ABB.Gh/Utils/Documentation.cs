// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Diagnostics;
using System.Collections.Generic;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Components.CodeGeneration;
using RobotComponents.ABB.Gh.Components.ControllerUtility;
using RobotComponents.ABB.Gh.Components.Deconstruct.CodeGeneration;
using RobotComponents.ABB.Gh.Components.Deconstruct.Definitions;
using RobotComponents.ABB.Gh.Components.Simulation;
using RobotComponents.ABB.Gh.Components.Definitions;
using RobotComponents.ABB.Gh.Components.Definitions.Presets;
using RobotComponents.ABB.Gh.Components.MultiMove;
using RobotComponents.ABB.Gh.Components.Utilities;
using RobotComponents.ABB.Gh.Parameters.Actions;
using RobotComponents.ABB.Gh.Parameters.Definitions;

namespace RobotComponents.ABB.Gh.Utils
{
    /// <summary>
    /// Static class that contains all the data (e.g. links to pages) that is relevant for our documentation.
    /// </summary>
    public static class Documentation
    {
        /// <summary>
        /// Dictionary that contains all the links to the documention of the different component types.
        /// </summary>
        public readonly static Dictionary<Type, string> ComponentWeblinks = new Dictionary<Type, string>()
        {
            { typeof(Documentation), "https://robotcomponents.github.io/RobotComponents-Documentation/" },

            #region Code generation
            // Declarations
            { typeof(JointTargetComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Declarative%20Actions/Joint%20Target/" },
            { typeof(ExternalJointPositionComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Declarative%20Actions/External%20Joint%20Position/" },
            { typeof(RobotJointPositionComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Declarative%20Actions/Robot%20Joint%20Position/" },
            { typeof(SpeedDataComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Declarative%20Actions/Speed%20Data/" },
            { typeof(RobotTargetComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Declarative%20Actions/Robot%20Target/" },
            { typeof(ZoneDataComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Declarative%20Actions/Zone%20Data/" },
            // Instructions
            { typeof(AnalogOutputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Set%20Analog%20Output/" },
            { typeof(JointConfigurationControlComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Set%20Joint%20Configuration%20Control/" },
            { typeof(LinearConfigurationControlComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Set%20Linear%20Configuration%20Control/" },
            { typeof(DigitalOutputComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Set%20Digital%20Output/" },
            { typeof(MoveComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Move/" },
            { typeof(OverrideRobotToolComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Override%20Robot%20Tool/" },
            { typeof(WaitTimeComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Wait%20for%20Time/" },
            { typeof(WaitAIComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Wait%20for%20Analog%20Input/" },
            { typeof(WaitDIComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Wait%20for%20Digital%20Input/" },
            // Dynamic
            { typeof(CodeLineComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Dynamic%20Actions/Code/" },
            { typeof(CommentComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Dynamic%20Actions/Comment/" },
            // Generators
            { typeof(RAPIDGeneratorComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/RAPID%20Generator/" },
            #endregion

            #region Controller utility
            { typeof(GetAnalogInputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(GetAnalogOutputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(GetControllerComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(GetDigitalInputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(GetDigitalOutputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(GetExternalAxisPlaneComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(GetExternalJointPositionComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(GetLogComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(GetRobotBaseFrameComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(GetRobotJointPositionComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(GetRobotToolPlaneComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(GetTaskNameComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(ReadConfigurationDomainComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(RunProgramComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(SetAnalogInputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(SetAnalogOutputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(SetDigitalInputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(SetDigitalOutputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            { typeof(UploadProgramComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" },
            #endregion

            #region Deconstruct
            // Actions
            { typeof(DeconstructMovementComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Actions/Deconstruct%20Move/" },
            { typeof(DeconstructSpeedDataComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Actions/Deconstruct%20Speed%20Data/" },
            { typeof(DeconstructRobotTargetComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Actions/Deconstruct%20Robot%20Target/" },
            { typeof(DeconstructZoneDataComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Actions/Deconstruct%20Zone%20Data/" },
            { typeof(DeconstructJointTargetComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Actions/Deconstruct%20Joint%20Target/" },
            { typeof(DeconstructExternalJointPositionComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Actions/Deconstruct%20External%20Joint%20Position/" },
            { typeof(DeconstructRobotJointPosistionComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Actions/Deconstruct%20Robot%20Joint%20Position/" },
            // Definitions
            { typeof(DeconstructExternalLinearAxisComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Definitions/Deconstruct%20External%20Linear%20Axis/" },
            { typeof(DeconstructExternalRotationalAxisComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Definitions/Deconstruct%20External%20Rotational%20Axis/" },
            { typeof(DeconstructRobotComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Definitions/Deconstruct%20Robot/" },
            { typeof(DeconstructRobotToolComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Definitions/Deconstruct%20Robot%20Tool/" },
            { typeof(DeconstructWorkObjectComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Definitions/Deconstruct%20Work%20Object/" },
            
            #endregion

            #region Definitions
            { typeof(ExternalLinearAxisComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/External%20Linear%20Axis/" },
            { typeof(ExternalRotationalAxisComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/External%20Rotational%20Axis/" },
            { typeof(RobotComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot/" },
            { typeof(RobotPresetComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(RobotToolComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Tool/" },
            { typeof(WorkObjectComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Work%20Object/" },
            #endregion

            #region Definitions: Robot presets
            { typeof(IRB1010_1_5_0_37_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB1100_4_0_475_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB1100_4_0_58_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB120_3_0_58_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB1200_5_90_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB1200_7_70_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB1300_10_1_15_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB1300_11_0_90_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB1300_7_1_40_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB140_6_0_81_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB1520ID_4_1_50_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB1600_X_1_20_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB1600_X_1_45_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB1660ID_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB2600_12_1_85_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB2600_X_1_65_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB2600ID_15_1_85_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB2600ID_8_2_00_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB4600_20_2_50_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB4600_40_2_55_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB4600_X_2_05_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6620_150_2_20_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6640_185_2_80_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6640_235_2_55_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6650_125_3_20_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6650_200_2_75_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6650S_125_3_50_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6650S_200_3_00_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6650S_90_3_90_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6700_150_3_20_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6700_155_2_85_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6700_175_3_05_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6700_200_2_60_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6700_205_2_80_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6700_235_2_65_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6700_245_3_00_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6700_300_2_70_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6790_235_2_65_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB6790_205_2_80_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB7600_150_3_50_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB7600_325_3_10_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB7600_340_2_80_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB7600_400_2_55_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(IRB7600_500_2_55_Component), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            #endregion

            #region Multi Move
            { typeof(TaskListComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Multi%20Move/Task%20List/"},
            { typeof(WaitSyncTaskComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Multi%20Move/Sync%20Move%20On/"},
            { typeof(SyncMoveOnComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Multi%20Move/Sync%20Move%20Off/"},
            { typeof(SyncMoveOffComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Multi%20Move/Wait%20Sync%20Task/"},
            #endregion

            #region Simulation
            { typeof(ForwardKinematicsComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Simulation/Forward%20Kinematics/" },
            { typeof(InverseKinematicsComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Simulation/Inverse%20Kinematics/" },
            { typeof(PathGeneratorComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Simulation/Path%20Generator/" },
            #endregion

            #region Utilities
            { typeof(FlipPlaneXComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Utility/Flip%20Plane%20X/" },
            { typeof(FlipPlaneYComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Utility/Flip%20Plane%20Y/" },
            { typeof(GetObjectManager), "https://robotcomponents.github.io/RobotComponents-Documentation/" }, 
            { typeof(PlaneToQuaternionComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Utility/Plane%20to%20Quarternion/" },
            { typeof(PlaneVisualizerComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Utility/Plane%20Visualizer/" },
            { typeof(QuaternionToPlaneComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Utility/Quaternion%20to%20Plane/" },
            { typeof(RobotToolCalibrationComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Utility/Robot%20Tool%20Calibration/" },
            { typeof(GroupActionsComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Utility/Group%20Actions/" },
            { typeof(UngroupActionsComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Utility/Ungroup%20Actions/" },
            { typeof(InfoComponent), "https://robotcomponents.github.io/RobotComponents-Documentation"},
            #endregion

            #region Parameters
            // Actions
            { typeof(Param_JointTarget), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Joint%20Target/" },
            { typeof(Param_RobotTarget), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Robot%20Target/" },
            { typeof(Param_RobotJointPosition), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Robot%20Joint%20Position/" },
            { typeof(Param_ExternalJointPosition), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/External%20Joint%20Position/" },
            { typeof(Param_Action), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Action/"},
            { typeof(Param_ActionGroup), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Action%20Group/"},
            { typeof(Param_AnalogOutput), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Set%20Analog%20Output/" }, 
            { typeof(Param_LinearConfigurationControl), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Set%20Linear%20Configuration%20Control/"},
            { typeof(Param_JointConfigurationControl), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Set%20Joint%20Configuration%20Control/"},
            { typeof(Param_CodeLine), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Code%20Line/"},
            { typeof(Param_Comment), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Comment/"},
            { typeof(Param_DigitalOutput), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Set%20Digital%20Output/"},
            { typeof(Param_Movement), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Move/"},
            { typeof(Param_OverrideRobotTool), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Override%20Robot%20Tool/"},
            { typeof(Param_SpeedData), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Speed%20Data/"},
            { typeof(Param_Target), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Target/"},
            { typeof(Param_WaitTime), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Wait%20for%20Time/"},
            { typeof(Param_WaitAI), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Wait%20for%20Analog%20Input/"},
            { typeof(Param_WaitDI), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Wait%20for%20Digital%20Input/"},
            { typeof(Param_ZoneData), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Zone%20Data/"},
            // Multi Move
            { typeof(Param_TaskList), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Task%20List/"},
            { typeof(Param_WaitSyncTask), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Wait%20Sync%20Task/"},
            { typeof(Param_SyncMoveOn), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Sync%20Move%20On/"},
            { typeof(Param_SyncMoveOff), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Sync%20Move%20Off/"},
            // Definitions
            { typeof(Param_ExternalAxis), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Definitions/External%20Axis/"},
            { typeof(Param_ExternalLinearAxis), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Definitions/External%20Linear%20Axis/"},
            { typeof(Param_ExternalRotationalAxis), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Definitions/External%20Rotational%20Axis/"},
            { typeof(Param_MechanicalUnit), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Definitions/Mechanical%20Unit/"},
            { typeof(Param_Robot), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Definitions/Robot/"},
            { typeof(Param_RobotTool), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Definitions/Robot%20Tool/"},
            { typeof(Param_WorkObject), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Definitions/Work%20Object/"},
            #endregion
        };

        /// <summary>
        /// Open an url in the default webbrowser
        /// </summary>
        /// <param name="url"> The url as a string. </param>
        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            { 
                // We do nothing if the browser could not be opened
            }
        }
    }
}
