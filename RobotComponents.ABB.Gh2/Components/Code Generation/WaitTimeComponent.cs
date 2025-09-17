// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// Copyright (c) 2022-2025 Arjen Deetman
//
// Authors:
//   - Arjen Deetman (2022-2025)
//
// For license details, see the LICENSE file in the project root.

// Grasshopper Libs
using Grasshopper2.Components;
using Grasshopper2.UI;
using GrasshopperIO;
// Robot Components Libs
using RobotComponents.ABB.Actions.Instructions;

namespace RobotComponents.ABB.Gh2.Components.Code_Generation
{
    [IoId("222C39F4-A09B-4AB5-8133-FFD258AE8DF5")]
    public sealed class WaitTimeComponent : Component
    {
        public WaitTimeComponent()
            : base(new Nomen("Wait for Time",
                "Defines an instruction to wait a given amount of time between two other RAPID instructions.",
                "Robot Components",
                "Code Generation"))
        {
            Threading = ThreadingState.UiSingleThreaded;
        }

        protected override void AddInputs(InputAdder inputs)
        {
            inputs.AddNumber("Duration", "D", "Duration in seconds as a number").Set(1.0);
        }

        protected override void AddOutputs(OutputAdder outputs)
        {
            outputs.AddGeneric("Wait Time", "WT", "Resulting Wait for Time instruction");
        }

        protected override void Process(IDataAccess access)
        {
            // Get inputs
            access.GetItem(0, out double time);

            // Set outputs
            access.SetItem(0, new WaitTime(time));
        }
    }
}