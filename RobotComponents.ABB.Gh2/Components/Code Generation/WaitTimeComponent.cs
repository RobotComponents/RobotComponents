// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Grasshopper Libs
using Grasshopper.Components;
using Grasshopper.UI;
using GrasshopperIO;
// Robot Components Libs
using RobotComponents.ABB.Actions;

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