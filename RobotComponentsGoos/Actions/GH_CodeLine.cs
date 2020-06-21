// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
// RobotComponentsLibs
using RobotComponents.BaseClasses.Actions;

namespace RobotComponentsGoos.Actions
{
    /// <summary>
    /// Code Line Goo wrapper class, makes sure the Code Line class can be used in Grasshopper.
    /// </summary>
    public class GH_CodeLine : GH_Goo<CodeLine>
    {
        #region constructors
        /// <summary>
        /// Blank constructor
        /// </summary>
        public GH_CodeLine()
        {
            this.Value = null;
        }

        /// <summary>
        /// Data constructor: Create a Code Line Goo instance from a Code Line instance.
        /// </summary>
        /// <param name="codeLine"> Code Line Value to store inside this Goo instance. </param>
        public GH_CodeLine(CodeLine codeLine)
        {
            this.Value = codeLine;
        }

        /// <summary>
        /// Data constructor: Creates a Code Line Goo instance from another Code Line Goo instance.
        /// This creates a shallow copy of the passed Code Line Goo instance. 
        /// </summary>
        /// <param name="codeLineGoo"> Code Line Goo instance to copy. </param>
        public GH_CodeLine(GH_CodeLine codeLineGoo)
        {
            if (codeLineGoo == null)
            {
                codeLineGoo = new GH_CodeLine();
            }

            this.Value = codeLineGoo.Value;
        }

        /// <summary>
        /// Make a complete duplicate of this Goo instance. No shallow copies.
        /// </summary>
        /// <returns> A duplicate of the CodeLineGoo. </returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_CodeLine(Value == null ? new CodeLine() : Value.Duplicate());
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
                if (Value == null) { return "No internal Code Line instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Code Line instance: Did you define a Text?";
            }
        }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null) { return "Null Code Line"; }
            else { return Value.ToString(); }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Code Line"; }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Code Line."; }
        }
        #endregion

        #region casting methods
        /// <summary>
        /// Attempt a cast to type Q.
        /// </summary>
        /// <typeparam name="Q"> Type to cast to.  </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(ref Q target)
        {
            //Cast to Code Line
            if (typeof(Q).IsAssignableFrom(typeof(CodeLine)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)Value; }
                return true;
            }

            //Cast to Code Line Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_CodeLine)))
            {
                if (Value == null) { target = default(Q); }
                else { target = (Q)(object)new GH_CodeLine(Value); }
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

            //Cast from Code Line
            if (typeof(CodeLine).IsAssignableFrom(source.GetType()))
            {
                Value = source as CodeLine;
                return true;
            }

            //Cast from Action
            if (typeof(Action).IsAssignableFrom(source.GetType()))
            {
                if (source is CodeLine action)
                {
                    Value = action;
                    return true;
                }
            }

            //Cast from Action Goo
            if (typeof(GH_Action).IsAssignableFrom(source.GetType()))
            {
                GH_Action actionGoo = source as GH_Action;
                if (actionGoo.Value is CodeLine action)
                {
                    Value = action;
                    return true;
                }
            }

            // Cast from Text
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                GH_String ghString = (GH_String)source;
                Value = new CodeLine(ghString.Value);
                return true;
            }

            return false;
        }
        #endregion
    }
}
