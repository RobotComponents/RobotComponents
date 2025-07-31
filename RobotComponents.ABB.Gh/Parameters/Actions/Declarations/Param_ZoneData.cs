// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotcComponents Libs
using RobotComponents.ABB.Gh.Goos.Actions.Declarations;

namespace RobotComponents.ABB.Gh.Parameters.Actions.Declarations
{
    /// <summary>
    /// Zone Data parameter
    /// </summary>
    public class Param_ZoneData : GH_RobotParam<GH_ZoneData>
    {
        /// <summary>
        /// Initializes a new instance of the GH_RobotParam<GH_ZoneData> class
        /// </summary>
        public Param_ZoneData() : base("Zone Data Parameter", "ZD", "Parameters",
                "Contains the data of a Zone Data declaration.")
        {
        }

        /// <summary>
        /// Converts this structure to a human-readable string.
        /// </summary>
        /// <returns> A string representation of the parameter. </returns>
        public override string ToString()
        {
            return "Zone Data";
        }

        /// <summary>
        /// Gets or sets the name of the object. This field typically remains fixed during the lifetime of an object.
        /// </summary>
        public override string Name { get => "Zone Data"; set => base.Name = value; }

        /// <summary>
        /// Override this function to supply a custom icon (24x24 pixels). 
        /// The result of this property is cached, so don't worry if icon retrieval is not very fast.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.ZoneData_Parameter_Icon; }
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.The default is to expose everywhere.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Returns a consistent ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1DEA0335-6574-4A08-9616-8D49B72D4162"); }
        }
    }
}
