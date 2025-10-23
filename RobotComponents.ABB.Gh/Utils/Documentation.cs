// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2020 EDEK Uni Kassel
// Copyright (c) 2020-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2020-2025)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Components.CodeGeneration;
using RobotComponents.ABB.Gh.Components.ControllerUtility;
using RobotComponents.ABB.Gh.Components.Deconstruct.CodeGeneration;
using RobotComponents.ABB.Gh.Components.Deconstruct.Definitions;
using RobotComponents.ABB.Gh.Components.Simulation;
using RobotComponents.ABB.Gh.Components.Definitions;
using RobotComponents.ABB.Gh.Components.MultiMove;
using RobotComponents.ABB.Gh.Components.Utilities;
using RobotComponents.ABB.Gh.Parameters.Actions;
using RobotComponents.ABB.Gh.Parameters.Actions.Declarations;
using RobotComponents.ABB.Gh.Parameters.Actions.Dynamic;
using RobotComponents.ABB.Gh.Parameters.Actions.Instructions;
using RobotComponents.ABB.Gh.Parameters.Definitions;

namespace RobotComponents.ABB.Gh.Utils
{
    /// <summary>
    /// Static class that contains all the data (e.g. links to pages) that is relevant for our documentation.
    /// </summary>
    internal static class Documentation
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
            { typeof(ConfigurationDataComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Declarative%20Actions/" }, // TODO
            // Instructions
            { typeof(Components.CodeGeneration.SetAnalogOutputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Set%20Analog%20Output/" },
            { typeof(JointConfigurationControlComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Set%20Joint%20Configuration%20Control/" },
            { typeof(LinearConfigurationControlComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Set%20Linear%20Configuration%20Control/" },
            { typeof(Components.CodeGeneration.SetDigitalOutputComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Set%20Digital%20Output/" },
            { typeof(MoveComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Move/" },
            { typeof(OverrideRobotToolComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Override%20Robot%20Tool/" },
            { typeof(WaitTimeComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Wait%20for%20Time/" },
            { typeof(WaitAIComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Wait%20for%20Analog%20Input/" },
            { typeof(WaitDIComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/Wait%20for%20Digital%20Input/" },
            { typeof(CirclePathModeComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/" }, // TODO
            { typeof(PulseDigitalOutputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/" }, // TODO
            { typeof(PathAccelerationLimitationComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/" }, // TODO
            { typeof(VelocitySetComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/" }, // TODO
            { typeof(AccelerationSetComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Instructive%20Actions/" }, // TODO
            // Dynamic
            { typeof(CodeLineComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Dynamic%20Actions/Code/" },
            { typeof(CommentComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/Dynamic%20Actions/Comment/" },
            // Generators
            { typeof(RAPIDGeneratorComponent) , "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Code%20Generation/RAPID%20Generator/" },
            #endregion

            #region Controller utility
            { typeof(GetAnalogInputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetAnalogOutputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetControllerComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetDigitalInputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetDigitalOutputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetExternalAxisPlanesComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetExternalJointPositionsComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetLogComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetRobotBaseFramesComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetRobotJointPositionsComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetRobotToolPlanesComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetTaskNamesComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(ReadConfigurationDomainComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(RunProgramComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(SetAnalogInputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(Components.ControllerUtility.SetAnalogOutputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(SetDigitalInputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(Components.ControllerUtility.SetDigitalOutputComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(UploadProgramComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetRobotTargetsComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(GetJointTargetsComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
            { typeof(ReadRapidDomainComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Controller%20Utility/" }, // TODO
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
            { typeof(DeconstructConfigurationDataComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Actions/" }, // TODO
            { typeof(DeconstructPathAccelerationLimitationComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Actions/" }, // TODO
            // Definitions
            { typeof(DeconstructExternalLinearAxisComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Definitions/Deconstruct%20External%20Linear%20Axis/" },
            { typeof(DeconstructExternalRotationalAxisComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Definitions/Deconstruct%20External%20Rotational%20Axis/" },
            { typeof(DeconstructRobotComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Definitions/Deconstruct%20Robot/" },
            { typeof(DeconstructRobotToolComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Definitions/Deconstruct%20Robot%20Tool/" },
            { typeof(DeconstructWorkObjectComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Definitions/Deconstruct%20Work%20Object/" },
            { typeof(DeconstructLoadDataComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Deconstruct/Definitions/" },
            { typeof(DeconstructRobotKinematicParametersComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/" }, // TODO
            #endregion

            #region Definitions
            { typeof(ExternalLinearAxisComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/External%20Linear%20Axis/" },
            { typeof(ExternalRotationalAxisComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/External%20Rotational%20Axis/" },
            { typeof(RobotComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot/" },
            { typeof(RobotPresetComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Presets/" },
            { typeof(RobotToolComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Robot%20Tool/" },
            { typeof(WorkObjectComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/Work%20Object/" },
            { typeof(LoadDataComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/" }, // TODO
            { typeof(RobotKinematicParametersComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/" }, // TODO
            { typeof(GetAxisPlanesFromKinematicsParametersComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Definitions/" }, // TODO
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
            { typeof(GetObjectManagerComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/" }, // TODO
            { typeof(CheckVariableNamesComponent), "https://robotcomponents.github.io/RobotComponents-Documentation/" }, // TODO
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
            { typeof(Param_SetAnalogOutput), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Set%20Analog%20Output/" },
            { typeof(Param_LinearConfigurationControl), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Set%20Linear%20Configuration%20Control/"},
            { typeof(Param_JointConfigurationControl), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Set%20Joint%20Configuration%20Control/"},
            { typeof(Param_CodeLine), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Code%20Line/"},
            { typeof(Param_Comment), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Comment/"},
            { typeof(Param_SetDigitalOutput), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Set%20Digital%20Output/"},
            { typeof(Param_Movement), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Move/"},
            { typeof(Param_OverrideRobotTool), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Override%20Robot%20Tool/"},
            { typeof(Param_SpeedData), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Speed%20Data/"},
            { typeof(Param_Target), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Target/"},
            { typeof(Param_WaitTime), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Wait%20for%20Time/"},
            { typeof(Param_WaitAI), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Wait%20for%20Analog%20Input/"},
            { typeof(Param_WaitDI), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Wait%20for%20Digital%20Input/"},
            { typeof(Param_ZoneData), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/Zone%20Data/"},
            { typeof(Param_CirclePathMode), "https://robotcomponents.github.io/RobotComponents-Documentation/" }, // TODO
            { typeof(Param_ConfigurationData), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/"}, // TODO
            { typeof(Param_PathAccelerationLimitation), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/"}, // TODO
            { typeof(Param_PulseDigitalOutput), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/"}, // TODO
            { typeof(Param_VelocitySet), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/"}, // TODO
            { typeof(Param_AccelerationSet), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Actions/"}, // TODO
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
            { typeof(Param_LoadData), "https://robotcomponents.github.io/RobotComponents-Documentation/docs/Parameters/Definitions/"}, // TODO
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
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
            }
            catch
            {
                // We do nothing if the browser could not be opened
            }
        }
    }
}
