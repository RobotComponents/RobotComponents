using System;
using System.Collections.Generic;

using Rhino.Geometry;
using Grasshopper.Kernel;

using RobotComponents.Goos;

namespace RobotComponents.Parameters
{
    public class SetRobotToolParameter : GH_PersistentGeometryParam<SetRobotToolGoo>, IGH_PreviewObject
    {
        public SetRobotToolParameter()
          : base(new GH_InstanceDescription("SetRobotTool", "RT", "Maintains SetRobotTool data.", "RobotComponents", "Actions"))
        {
        }

        public override string ToString()
        {
            return "Set Robot Tool";
        }

        public override string Description { get => "Resulting Set Robot Tool"; set => base.Description = value; }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.ChangeTool_Parameter_Icon;
            }
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("0F58F521-09B0-442C-A7A4-65795B0A2D6E"); }
        }

        protected override GH_GetterResult Prompt_Plural(ref List<SetRobotToolGoo> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref SetRobotToolGoo value)
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
