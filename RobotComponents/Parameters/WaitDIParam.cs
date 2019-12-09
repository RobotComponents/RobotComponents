using System;
using System.Collections.Generic;

using Rhino.Geometry;
using Grasshopper.Kernel;

using RobotComponents.Goos;

namespace RobotComponents.Parameters
{
    public class WaitDIParameter : GH_PersistentGeometryParam<WaitDIGoo>, IGH_PreviewObject
    {
        public WaitDIParameter()
          : base(new GH_InstanceDescription("WaitDI", "WDI", "Contains WaitDI Data.", "RobotComponents", "Actions"))
        {
        }

        public override string ToString()
        {
            return "WaitDI";
        }

        public override string Description { get => "Resulting WaitDI"; set => base.Description = value; }

        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.WaitDI_Parameter_Icon; }
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("6A8D1013-52BF-4373-8CEB-9EFA8B96A9F0"); }
        }

        //We do not allow users to pick Targets, 
        //therefore the following 4 methods disable all this ui.
        protected override GH_GetterResult Prompt_Plural(ref List<WaitDIGoo> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref WaitDIGoo value)
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
