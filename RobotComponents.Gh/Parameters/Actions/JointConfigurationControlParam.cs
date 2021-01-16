// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.Gh.Goos.Actions;

namespace RobotComponents.Gh.Parameters.Actions
{
    /// <summary>
    /// Joint Configuration Control parameter
    /// </summary>
    public class JointConfigurationControlParameter : GH_PersistentParam<GH_JointConfigurationControl>
    {
        /// <summary>
        /// Initializes a new instance of the GH_PersistentParam<GH_JointConfigurationControl> class
        /// </summary>
        public JointConfigurationControlParameter()
          : base(new GH_InstanceDescription("Joint Configuration Control Parameter", "JCCP",
                "Contains the data of a Set Joint Configuration Control instruction."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.Utils.VersionNumbering.CurrentVersion,
                "RobotComponents", "Parameters"))
        {
        }

        /// <summary>
        /// Converts this structure to a human-readable string.
        /// </summary>
        /// <returns> A string representation of the parameter. </returns>
        public override string ToString()
        {
            return "Joint Configuration Control";
        }

        /// <summary>
        /// Gets or sets the name of the object. This field typically remains fixed during the lifetime of an object.
        /// </summary>
        public override string Name { get => "Joint Configuration Control"; set => base.Name = value; }

        /// <summary>
        /// Override this function to supply a custom icon (24x24 pixels). 
        /// The result of this property is cached, so don't worry if icon retrieval is not very fast.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.ConfJ_Parameter_Icon; }
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.The default is to expose everywhere.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Returns a consistent ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3CBD5E38-2C5D-4C06-A8CD-5938D7703D77"); }
        }

        // We do not allow users to pick parameters, therefore the following 4 methods disable all this ui.
        #region disable pick parameters
        protected override GH_GetterResult Prompt_Plural(ref List<GH_JointConfigurationControl> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_JointConfigurationControl value)
        {
            return GH_GetterResult.cancel;
        }

        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
            item.Text = "Not available";
            item.Visible = false;
            return item;
        }

        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
            item.Text = "Not available";
            item.Visible = false;
            return item;
        }
        #endregion
    }


}
