using System;
using System.Collections.Generic;

using Rhino.Geometry;
using Grasshopper.Kernel;

using RobotComponents.Goos;

namespace RobotComponents.Parameters
{
    public class RobotInfoParameter : GH_PersistentGeometryParam<RobotInfoGoo>, IGH_PreviewObject
    {
        public RobotInfoParameter()
          : base(new GH_InstanceDescription("Robot Info", "Robot Info", "Maintains the Robot data.", "RobotComponents", "Definitions"))
        {
        }

        public override string ToString()
        {
            return "Robot Info";
        }

        public override string Description { get => "Resulting Robot Info"; set => base.Description = value; }

        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.RobotInfo_Parameter_Icon; }
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("DCCF6CCB-7463-4845-96C3-EB494170337C"); }
        }

        protected override GH_GetterResult Prompt_Plural(ref List<RobotInfoGoo> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref RobotInfoGoo value)
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
