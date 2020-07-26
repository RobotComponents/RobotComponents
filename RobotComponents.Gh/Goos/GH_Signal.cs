// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel.Types;
// Rhino Libs
using Rhino.Geometry;
// ABB Robotic Libs
using ABB.Robotics.Controllers.IOSystemDomain;

namespace RobotComponents.Gh.Goos
{
    /// <summary>
    /// Signal Goo wrapper class, makes sure Signal can be used in Grasshopper.
    /// </summary>
    public class GH_Signal : GH_GeometricGoo<DigitalSignal>
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Signal()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor from a signal value. 
        /// </summary>
        /// <param name="signal"> Signal Value to store inside this Goo instance. </param>
        public GH_Signal(DigitalSignal signal)
        {
            this.Value = signal;
        }

        /// <summary>
        /// Data constructor from other GH_Signal. This makes a shallow copy. 
        /// </summary>
        /// <param name="signalGoo"> SignalGoo to store inside this Goo instance. </param>
        public GH_Signal(GH_Signal signalGoo)
        {
            this.Value = signalGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the SignalGoo. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateSignalGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the SignalGoo. </returns>
        public override IGH_Goo Duplicate()
        {
            return DuplicateSignalGoo();
        }

        /// <summary>
        /// Make a complete duplicate of this geometry. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the SignalGoo. </returns>
        public GH_Signal DuplicateSignalGoo()
        {
            return new GH_Signal(Value);
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
                return true;
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
            {
                return "Null Signal";
            }
            else
            {
                return "Signal (" + Value.Name + "\\" + Value.Value + ")";
            }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Signal"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines an ABB Signal"; }
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
            return BoundingBox.Empty;
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
            //Cast to bool
            if (typeof(Q).IsAssignableFrom(typeof(bool)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)(Value.Value != 0);
                return true;
            }

            //Cast to GH_Boolean
            if (typeof(Q).IsAssignableFrom(typeof(GH_Boolean)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Boolean((Value.Value != 0));
                return true;
            }

            //Cast to integer
            if (typeof(Q).IsAssignableFrom(typeof(int)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Convert.ToInt32(Value.Value);
                return true;
            }

            //Cast to GH_Integer
            if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Integer(Convert.ToInt32(Value.Value));
                return true;
            }

            //Cast to double
            if (typeof(Q).IsAssignableFrom(typeof(double)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value.Value;
                return true;
            }

            //Cast to GH_Number
            if (typeof(Q).IsAssignableFrom(typeof(GH_Number)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Number(Value.Value);
                return true;
            }

            target = default(Q);
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
    }
}
