using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using RobotComponents.Goos;

namespace RobotComponents.Parameters
{
    public class DigitalOutputParameter : GH_PersistentGeometryParam<DigitalOutputGoo>, IGH_PreviewObject
    {
        public DigitalOutputParameter()
          : base(new GH_InstanceDescription("DigitalOutput", "DO", "Maintains Digital Output data.", "RobotComponents", "Actions"))
        {
        }

        public override string ToString()
        {
            return "Digital Output";
        }

        public override string Description { get => "Resulting Digital Output"; set => base.Description = value; }

        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.DigitalOutput_Parameter_Icon; }
        }
        public override GH_Exposure Exposure
        {
            get {return GH_Exposure.hidden; }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("C137C7B6-C6C0-482F-8192-732D9B1EA651"); }
        }

        protected override GH_GetterResult Prompt_Plural(ref List<DigitalOutputGoo> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref DigitalOutputGoo value)
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
