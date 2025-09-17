﻿// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components

// System Libs
using System;
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;
// Robot Components Libs
using RobotComponents.ABB.Gh.Goos.Actions.Instructions;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// Auto Axis Configuration parameter
    /// </summary>
    [Obsolete("This parameter is OBSOLETE and will be removed in the future.", false)]
    public class OldAutoAxisConfigParameter : GH_PersistentParam<GH_LinearConfigurationControl>
    {
        /// <summary>
        /// Initializes a new instance of the AutoAxisConfigParameter class
        /// </summary>
        public OldAutoAxisConfigParameter()
          : base(new GH_InstanceDescription("Auto Axis Configurator Parameter", "AACP",
                "Contains the data of an Set Auto Axis Configuration instruction."
                + System.Environment.NewLine + System.Environment.NewLine +
                "Robot Components: v" + RobotComponents.VersionNumbering.CurrentVersion,
                "RobotComponents", "Parameters"))
        {
        }

        /// <summary>
        /// Converts this structure to a human-readable string.
        /// </summary>
        /// <returns> A string representation of the parameter. </returns>
        public override string ToString()
        {
            return "Auto Axis Configuration";
        }

        /// <summary>
        /// Gets or sets the name of the object. This field typically remains fixed during the lifetime of an object.
        /// </summary>
        public override string Name { get => "Auto Axis Configuration"; set => base.Name = value; }

        /// <summary>
        /// Override this function to supply a custom icon (24x24 pixels). 
        /// The result of this property is cached, so don't worry if icon retrieval is not very fast.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return null; }
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
            get { return new Guid("F413CBF2-C0C8-4FA4-A8A2-CC60D1F10343"); }
        }

        // We do not allow users to pick parameters, therefore the following 4 methods disable all this ui.
        #region disable pick parameters
        /// <summary>
        /// Disables picking of multiple values through the Grasshopper UI.
        /// Always returns GH_GetterResult.cancel to block user interaction.
        /// </summary>
        /// <param name="values"> The list intended to store picked values (unused). </param>
        /// <returns> GH_GetterResult.cancel to indicate the operation is canceled. </returns>
        protected override GH_GetterResult Prompt_Plural(ref List<GH_LinearConfigurationControl> values)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Disables picking of a single value through the Grasshopper UI.
        /// Always returns GH_GetterResult.cancel to block user interaction.
        /// </summary>
        /// <param name="value"> The variable intended to store the picked value (unused). </param>
        /// <returns> GH_GetterResult.cancel to indicate the operation is canceled. </returns>
        protected override GH_GetterResult Prompt_Singular(ref GH_LinearConfigurationControl value)
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
