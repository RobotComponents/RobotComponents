﻿// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
// RobotComponents Libs
using RobotComponents.ABB.Gh.Goos.Definitions;

namespace RobotComponents.ABB.Gh.Obsolete
{
    /// <summary>
    /// External Linear Axis parameter
    /// </summary>
    [Obsolete("This parameter is OBSOLETE and will be removed in the future. Use Param_ExternalLinearAxis instead.", false)]
    public class OldExternalLinearAxisParameter : GH_PersistentGeometryParam<GH_ExternalLinearAxis>, IGH_PreviewObject
    {
        /// <summary>
        /// Initializes a new instance of the ExternalLinearAxisParameter class
        /// </summary>
        public OldExternalLinearAxisParameter()
          : base(new GH_InstanceDescription("External Linear Axis", "ELA",
                "Contains the data of an External Linear Axis."
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
            return "External Linear Axis";
        }

        /// <summary>
        /// Gets or sets the name of the object. This field typically remains fixed during the lifetime of an object.
        /// </summary>
        public override string Name { get => "External Linear Axis"; set => base.Name = value; }

        /// <summary>
        /// Override this function to supply a custom icon (24x24 pixels). 
        /// The result of this property is cached, so don't worry if icon retrieval is not very fast.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.ExternalLinearAxis_Parameter_Icon; }
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
            get { return new Guid("11E61A1F-4673-4F22-8DA6-D0008DB13B26"); }
        }

        // We do not allow users to pick parameters, therefore the following 4 methods disable all this ui.
        #region disable pick parameters
        /// <summary>
        /// Disables picking of multiple values through the Grasshopper UI.
        /// Always returns GH_GetterResult.cancel to block user interaction.
        /// </summary>
        /// <param name="values"> The list intended to store picked values (unused). </param>
        /// <returns> GH_GetterResult.cancel to indicate the operation is canceled. </returns>
        protected override GH_GetterResult Prompt_Plural(ref List<GH_ExternalLinearAxis> values)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Disables picking of a single value through the Grasshopper UI.
        /// Always returns GH_GetterResult.cancel to block user interaction.
        /// </summary>
        /// <param name="value"> The variable intended to store the picked value (unused). </param>
        /// <returns> GH_GetterResult.cancel to indicate the operation is canceled. </returns>
        protected override GH_GetterResult Prompt_Singular(ref GH_ExternalLinearAxis value)
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

        #region preview methods
        /// <summary>
        /// Gets the clipping box for this data. The clipping box is typically the same as the boundingbox.
        /// </summary>
        public BoundingBox ClippingBox
        {
            get
            {
                return Preview_ComputeClippingBox();
            }
        }

        /// <summary>
        /// Implement this function to draw all shaded meshes. 
        /// If the viewport does not support shading, this function will not be called.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            Preview_DrawMeshes(args);
        }

        /// <summary>
        /// Implement this function to draw all wire and point previews.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            //Use a standard method to draw wires, you don't have to specifically implement this.
            Preview_DrawWires(args);
        }

        private bool m_hidden = false;

        /// <summary>
        /// Gets or sets the hidden flag for this component. Does not affect Hidden flags on parameters associated with this component.
        /// </summary>
        public bool Hidden
        {
            get { return m_hidden; }
            set { m_hidden = value; }
        }

        /// <summary>
        /// If a single parameter is PreviewCapable, so is the component. Override this property if you need special Preview flags.
        /// </summary>
        public bool IsPreviewCapable
        {
            get { return true; }
        }
        #endregion
    }
}
