using System;
using System.Collections.Generic;

using Rhino.Geometry;
using Grasshopper.Kernel;

using RobotComponents.Goos;

namespace RobotComponents.Parameters
{

    public class CodeLineParameter : GH_PersistentGeometryParam<CodeLineGo>, IGH_PreviewObject
    {
        public CodeLineParameter()
          : base(new GH_InstanceDescription("Code Line Parameter", "CLP", "Defines a single CodeLine.", "RobotComponents", "Actions"))
        {
        }

        public override string ToString()
        {
            return "Code Line";
        }

        public override string Description { get => "Resulting Code Line"; set => base.Description = value; }

        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.CodeLine_Parameter_Icon; }
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("2154A09B-BC1F-40B5-BD5B-58ABEC37B2E2"); }
        }

        protected override GH_GetterResult Prompt_Plural(ref List<CodeLineGo> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref CodeLineGo value)
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
