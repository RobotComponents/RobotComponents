// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
// RobotComponents Libs
using RobotComponents.BaseClasses.Actions;

namespace RobotComponentsGoos.Actions
{
    /// <summary>
    /// Comment Goo wrapper class, makes sure the Comment class can be used in Grasshopper.
    /// </summary>
    public class GH_Comment : GH_Goo<Comment>
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_Comment()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Creates a Comment Goo instance from a Comment instance.
        /// </summary>
        /// <param name="comment"> Comment Value to store inside this Goo instance. </param>
        public GH_Comment(Comment comment)
        {
            this.Value = comment;
        }

        /// <summary>
        /// Data constructor: Creates a Comment Goo instance from another Comment Goo instance.
        /// This creates a shallow copy of the passed Comment Goo instance. 
        /// </summary>
        /// <param name="commentGoo"> Comment Goo instance to copy. </param>
        public GH_Comment(GH_Comment commentGoo)
        {
            if (commentGoo == null)
            {
                commentGoo = new GH_Comment();
            }

            this.Value = commentGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the Comment Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_Comment(Value == null ? new Comment() : Value.Duplicate());
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
        /// Gets a string describing the state of "invalidness". 
        /// If the instance is valid, then this property should return Nothing or String.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal Comment instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Comment instance: Did you define a Text?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Comment"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Comment"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Comment."; }
        }
        #endregion

        #region casting methods
        /// <summary>
        /// Attempt a cast to type Q.
        /// </summary>
        /// <typeparam name="Q"> Type to cast to.  </typeparam>
        /// <param name="comment"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(ref Q target)
        {
            //Cast to Comment
            if (typeof(Q).IsAssignableFrom(typeof(Comment)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Comment Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Comment)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Comment(Value); }
                return true;
            }

            //Cast to Action
            if (typeof(Q).IsAssignableFrom(typeof(Action)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Action Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Action)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_Action(Value); }
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

            //Cast from Comment
            if (typeof(Comment).IsAssignableFrom(source.GetType()))
            {
                Value = source as Comment;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is Comment action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is Comment action)
                {
                    Value = action;
                    return true;
                }
            }

            // Cast from Text
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                GH_String ghString = (GH_String)source;
                Value = new Comment(ghString.Value);
                return true;
            }

            return false;
        }
        #endregion
    }
}
