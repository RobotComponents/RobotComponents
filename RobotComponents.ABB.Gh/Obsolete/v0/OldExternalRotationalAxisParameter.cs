﻿// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

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
    /// External Rotational Axis parameter
    /// </summary>
    [Obsolete("This parameter is OBSOLETE and will be removed in the future. Use Param_ExternalRotationalAxis instead.", false)]
    public class ExternalRotationalAxisParameter : GH_PersistentGeometryParam<GH_ExternalRotationalAxis>, IGH_PreviewObject
    {
        /// <summary>
        /// Initializes a new instance of the GH_PersistentGeometryParam<GH_ExternalRotationalAxis> class
        /// </summary>
        public ExternalRotationalAxisParameter()
          : base(new GH_InstanceDescription("External Rotational Axis", "ERA",
                "Contains the data of an External Rotational Axis."
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
            return "External Rotational Axis";
        }

        /// <summary>
        /// Gets or sets the name of the object. This field typically remains fixed during the lifetime of an object.
        /// </summary>
        public override string Name { get => "External Rotational Axis"; set => base.Name = value; }

        /// <summary>
        /// Override this function to supply a custom icon (24x24 pixels). 
        /// The result of this property is cached, so don't worry if icon retrieval is not very fast.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.ExternalRotationalAxis_Parameter_Icon; }
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.The default is to expose everywhere.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            {
                // If you want to provide this parameter on the toolbars, use something other than hidden.
                return GH_Exposure.hidden;
            }
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
            get { return new Guid("5AE7B921-18A0-4197-B6E8-8D8A6ABC32F4"); }
        }

        // We do not allow users to pick parameters, therefore the following 4 methods disable all this ui.
        #region disable pick parameters
        protected override GH_GetterResult Prompt_Plural(ref List<GH_ExternalRotationalAxis> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_ExternalRotationalAxis value)
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
