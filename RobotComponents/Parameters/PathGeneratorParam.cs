using System;
using System.Collections.Generic;

using Rhino.Geometry;
using Grasshopper.Kernel;

using RobotComponents.Goos;

namespace RobotComponents.Parameters
{

    public class PathGeneratorParameter : GH_PersistentGeometryParam<PathGeneratorGoo>, IGH_PreviewObject
    {
        public PathGeneratorParameter()
          : base(new GH_InstanceDescription("Path Generator", "PG", "Generates Path from Actions.", "RobotComponents", "Simulation"))
        {
        }

        public override string ToString()
        {
            return "Path Generator";
        }

        public override string Description { get => "Resulting Path Generator"; set => base.Description = value; }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.PathGen_Parameter_Icon;
            }
        }
        public override GH_Exposure Exposure
        {
            get
            {
                // If you want to provide this parameter on the toolbars, use something other than hidden.
                return GH_Exposure.hidden;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("9922B6D3-11A8-4670-AE6A-DA0F7D4D97DC"); }
        }

        //We do not allow users to pick PathGenerator, 
        //therefore the following 4 methods disable all this ui.
        protected override GH_GetterResult Prompt_Plural(ref List<PathGeneratorGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref PathGeneratorGoo value)
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

        #region preview methods
        public BoundingBox ClippingBox
        {
            get
            {
                return Preview_ComputeClippingBox();
            }
        }
        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            Preview_DrawMeshes(args);
        }
        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            //Use a standard method to draw wires, you don't have to specifically implement this.
            Preview_DrawWires(args);
        }

        private bool m_hidden = false;
        public bool Hidden
        {
            get { return m_hidden; }
            set { m_hidden = value; }
        }
        public bool IsPreviewCapable
        {
            get { return true; }
        }
        #endregion
    }
}
