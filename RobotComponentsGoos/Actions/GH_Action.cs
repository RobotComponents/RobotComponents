using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using RobotComponents.BaseClasses.Actions;

namespace RobotComponentsGoos.Actions
{
    /// <summary>
    /// Action Goo wrapper class, makes sure Actions can be used in Grasshopper.
    /// </summary>
    public class GH_Action : GH_GeometricGoo<Action>, IGH_PreviewData
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Action()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor from from an Action
        /// </summary>
        /// <param name="action"> Action Value to store inside this Goo instance. </param>
        public GH_Action(Action action)
        {
            if (action == null)
            {
                action = null;
            }

            this.Value = action;
        }

        /// <summary>
        /// Data constructor from ActionGoo
        /// </summary>
        /// <param name="actionGoo"> ActionGoo to store inside this Goo instance. </param>
        public GH_Action(GH_Action actionGoo)
        {
            if (actionGoo == null)
            {
                actionGoo = new GH_Action();
            }

            this.Value = actionGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the ActionGoo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateActionGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the ActionGoo. </returns>
        public GH_Action DuplicateActionGoo()
        {
            if (Value == null) 
                return null; 

            else if (Value is Action) 
                return new GH_Action(Value.DuplicateAction()); 

            else 
                return null; 
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the current value is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return Value.IsValid;
            }
        }

        /// <summary>
        /// ets a string describing the state of "invalidness". 
        /// If the instance is valid, then this property should return Nothing or String.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No inter Action instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Action instance: ?"; //Todo: beef this up to be more informative.
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
                return "Null Action";
            else if (Value is AbsoluteJointMovement)
                return "Absolute Joint Movement";
            else if (Value is AutoAxisConfig)
                return "Auto Axis Configuration";
            else if (Value is CodeLine)
                return "Code Line";
            else if (Value is Comment)
                return "Comment";
            else if (Value is DigitalOutput)
                return "Digital Output";
            else if (Value is Movement)
                return "Movement";
            else if (Value is OverrideRobotTool)
                return "Override Robot Tool";
            else if (Value is SpeedData)
                return "Speed Data";
            else if (Value is Target)
                return "Target";
            else if (Value is Timer)
                return "Timer";
            else if (Value is WaitDI)
                return "Wait for Digital Input";
            else
                return "Action";
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return ("Action"); }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return ("Defines an Action."); }
        }

        /// <summary>
        /// Gets the boundingbox for this geometry.
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get { return BoundingBox.Empty; }
        }

        /// <summary>
        /// Compute an aligned boundingbox.
        /// </summary>
        /// <param name="xform"> Transformation to apply to geometry for BoundingBox computation. </param>
        /// <returns> The world aligned boundingbox of the transformed geometry. </returns>
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            return Boundingbox;
        }
        #endregion

        #region casting methods
        /// <summary>
        /// Attempt a cast to type Q.
        /// </summary>
        /// <typeparam name="Q"> Type to cast to. </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(out Q target)
        {
            //Cast to Action.
            if (typeof(Q).IsAssignableFrom(typeof(Action)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            //Cast to ActionGoo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Action(Value);
                return true;
            }

            target = default(Q);
            return false;
        }

        /// <summary>
        /// Attempt a cast from generic object.
        /// </summary>
        /// <param name="source"> Reference to source of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                Value = source as Action;
                return true;
            }

            //Cast from ActionGoo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                Value = actionGoo.Value as Action;
                return true;
            }

            return false;
        }
        #endregion

        #region transformation methods
        /// <summary>
        /// Transforms the object or a deformable representation of the object.
        /// </summary>
        /// <param name="xform"> Transformation matrix. </param>
        /// <returns> Transformed geometry. If the local geometry can be transformed accurately, 
        /// then the returned instance equals this instance. Not all geometry types can be accurately 
        /// transformed under all circumstances though, if this is the case, this function will 
        /// return an instance of another IGH_GeometricGoo derived type which can be transformed.</returns>
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            return null;
        }

        /// <summary>
        /// Morph the object or a deformable representation of the object.
        /// </summary>
        /// <param name="xmorph"> Spatial deform. </param>
        /// <returns> Deformed geometry. If the local geometry can be deformed accurately, then the returned 
        /// instance equals this instance. Not all geometry types can be accurately deformed though, if 
        /// this is the case, this function will return an instance of another IGH_GeometricGoo derived 
        /// type which can be deformed.</returns>
        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            return null;
        }
        #endregion

        #region drawing methods
        /// <summary>
        /// Gets the clipping box for this data. The clipping box is typically the same as the boundingbox.
        /// </summary>
        public BoundingBox ClippingBox
        {
            get { return Boundingbox; }
        }

        /// <summary>
        /// Implement this function to draw all shaded meshes. 
        /// If the viewport does not support shading, this function will not be called.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {

        }

        /// <summary>
        /// Implement this function to draw all wire and point previews.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {

        }
        #endregion
    }
}
