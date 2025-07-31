// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Goos.Actions.Instructions;

namespace RobotComponents.ABB.Gh.Parameters.Actions.Instructions
{
    /// <summary>
    /// Wait Sync Task parameter
    /// </summary>
    public class Param_WaitSyncTask : GH_RobotParam<GH_WaitSyncTask>
    {
        /// <summary>
        /// Initializes a new instance of the Param_WaitSyncTask class
        /// </summary>
        public Param_WaitSyncTask() : base("Wait Sync Task Parameter", "WST", "Parameters",
                "Contains the data of a Wait Sync Task synchronization point.")
        {
        }

        /// <summary>
        /// Converts this structure to a human-readable string.
        /// </summary>
        /// <returns> A string representation of the parameter. </returns>
        public override string ToString()
        {
            return "Wait Sync Task";
        }

        /// <summary>
        /// Gets or sets the name of the object. This field typically remains fixed during the lifetime of an object.
        /// </summary>
        public override string Name { get => "Wait Sync Task"; set => base.Name = value; }

        /// <summary>
        /// Override this function to supply a custom icon (24x24 pixels). 
        /// The result of this property is cached, so don't worry if icon retrieval is not very fast.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.WaitSyncTask_Parameter_Icon; }
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.The default is to expose everywhere.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
        }

        /// <summary>
        /// Returns a consistent ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B3C80E2B-E1F9-41C9-A122-2FC165CDEC4F"); }
        }
    }
}
