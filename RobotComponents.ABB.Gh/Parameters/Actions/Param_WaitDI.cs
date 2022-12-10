// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Goos.Actions;

namespace RobotComponents.ABB.Gh.Parameters.Actions
{
    /// <summary>
    /// Wait for Digital Input parameter
    /// </summary>
    public class Param_WaitDI : GH_PersistentParam<GH_WaitDI>
    {
        /// <summary>
        /// Initializes a new instance of the GH_PersistentParam<GH_WaitDI> class
        /// </summary>
        public Param_WaitDI()
          : base(new GH_InstanceDescription("Wait for Digital Input", "WDI",
                "Contains the data of a Wait for Digital Input instruction."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.ABB.Utils.VersionNumbering.CurrentVersion,
                "Robot Components ABB", "Parameters"))
        {
        }

        /// <summary>
        /// Converts this structure to a human-readable string.
        /// </summary>
        /// <returns> A string representation of the parameter. </returns>
        public override string ToString()
        {
            return "Wait for Digital Input";
        }

        /// <summary>
        /// Gets or sets the name of the object. This field typically remains fixed during the lifetime of an object.
        /// </summary>
        public override string Name { get => "Wait for Digital Input"; set => base.Name = value; }

        /// <summary>
        /// Override this function to supply a custom icon (24x24 pixels). 
        /// The result of this property is cached, so don't worry if icon retrieval is not very fast.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.WaitDI_Parameter_Icon; }
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.The default is to expose everywhere.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Returns a consistent ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6A8D1013-52BF-4373-8CEB-9EFA8B96A9F0"); }
        }

        // We do not allow users to pick parameters, therefore the following 4 methods disable all this ui.
        #region disable pick parameters
        protected override GH_GetterResult Prompt_Plural(ref List<GH_WaitDI> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_WaitDI value)
        {
            return GH_GetterResult.cancel;
        }

        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };

            return item;
        }

        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };

            return item;
        }
        #endregion
    }
}
