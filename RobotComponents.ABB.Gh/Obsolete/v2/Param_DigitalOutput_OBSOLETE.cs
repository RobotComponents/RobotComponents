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
    /// Digital Output parameter
    /// </summary>
    [Obsolete("This class is obsolete and will be removed in v3. Use Param_SetDigitalOutput instead.", false)]
    public class Param_DigitalOutput : GH_PersistentParam<GH_SetDigitalOutput>
    {
        /// <summary>
        /// Initializes a new instance of the Param_DigitalOutput class
        /// </summary>
        public Param_DigitalOutput()
          : base(new GH_InstanceDescription("Digital Output Parameter", "DO",
                "Contains the data of a Set Digital Output instruction."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
                "Robot Components ABB", "Parameters"))
        {
        }

        /// <summary>
        /// Converts this structure to a human-readable string.
        /// </summary>
        /// <returns> A string representation of the parameter. </returns>
        public override string ToString()
        {
            return "Digital Output";
        }

        /// <summary>
        /// Gets or sets the name of the object. This field typically remains fixed during the lifetime of an object.
        /// </summary>
        public override string Name { get => "Digital Output"; set => base.Name = value; }

        /// <summary>
        /// Override this function to supply a custom icon (24x24 pixels). 
        /// The result of this property is cached, so don't worry if icon retrieval is not very fast.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DigitalOutput_Parameter_Icon; }
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.The default is to expose everywhere.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return true; }
        }

        /// <summary>
        /// Returns a consistent ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C137C7B6-C6C0-482F-8192-732D9B1EA651"); }
        }

        // We do not allow users to pick parameters, therefore the following 4 methods disable all this ui.
        #region disable pick parameters
        /// <summary>
        /// Disables picking of multiple values through the Grasshopper UI.
        /// Always returns GH_GetterResult.cancel to block user interaction.
        /// </summary>
        /// <param name="values"> The list intended to store picked values (unused). </param>
        /// <returns> GH_GetterResult.cancel to indicate the operation is canceled. </returns>
        protected override GH_GetterResult Prompt_Plural(ref List<GH_SetDigitalOutput> values)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Disables picking of a single value through the Grasshopper UI.
        /// Always returns GH_GetterResult.cancel to block user interaction.
        /// </summary>
        /// <param name="value"> The variable intended to store the picked value (unused). </param>
        /// <returns> GH_GetterResult.cancel to indicate the operation is canceled. </returns>
        protected override GH_GetterResult Prompt_Singular(ref GH_SetDigitalOutput value)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Overrides the context menu item for setting multiple values.
        /// Returns a hidden menu item labeled "Not available".
        /// </summary>
        /// <returns> A hidden ToolStripMenuItem instance. </returns>
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };

            return item;
        }

        /// <summary>
        /// Overrides the context menu item for setting multiple values.
        /// Returns a hidden menu item labeled "Not available".
        /// </summary>
        /// <returns> A hidden ToolStripMenuItem instance. </returns>
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
