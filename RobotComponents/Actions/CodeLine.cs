// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.Definitions;
using RobotComponents.Enumerations;
using RobotComponents.Utils;

namespace RobotComponents.Actions
{
    /// <summary>
    /// Represents a custom (user definied) RAPID Code Line.
    /// </summary>
    [Serializable()]
    public class CodeLine : Action, IDynamic, ISerializable
    {
        #region fields
        private string _code; // the code line as a string
        private CodeType _type;  // the code line type as a CodeType enum
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected CodeLine(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _code = (string)info.GetValue("Code", typeof(string));
            _type = (CodeType)info.GetValue("Code Type", typeof(CodeType));
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VersionNumbering.CurrentVersionAsInt, typeof(int));
            info.AddValue("Code", _code, typeof(string));
            info.AddValue("Code Type", _type, typeof(CodeType));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a empty instance of the Code Line class.
        /// </summary>
        public CodeLine()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Code Line class with the Code Type set as instruction.
        /// </summary>
        /// <param name="code"> The custom RAPID code line. </param>
        public CodeLine(string code)
        {
            _code = code;
            _type = CodeType.Instruction;
        }

        /// <summary>
        /// Initializes a new instance of the Code Line class
        /// </summary>
        /// <param name="code"> The custom RAPID code line. </param>
        /// <param name="type">The Code Type. </param>
        public CodeLine(string code, CodeType type)
        {
            _code = code;
            _type = type;
        }

        /// <summary>
        /// Initializes a new instance of the Code Line class by duplicating an existing Code Line instance. 
        /// </summary>
        /// <param name="codeLine"> The Code Line instance to duplicate. </param>
        public CodeLine(CodeLine codeLine)
        {
            _code = codeLine.Code;
            _type = codeLine.Type;
        }

        /// <summary>
        /// Returns an exact duplicate of this Code Line instance.
        /// </summary>
        /// <returns> A deep copy of the Code Line instance. </returns>
        public CodeLine Duplicate()
        {
            return new CodeLine(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Code Line instance as IDynamic. 
        /// </summary>
        /// <returns> A deep copy of the Code Line instance as an IDynamic. </returns>
        public IDynamic DuplicateDynamic()
        {
            return new CodeLine(this) as IDynamic;
        }

        /// <summary>
        /// Returns an exact duplicate of this Code Line instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Code Line instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new CodeLine(this) as Action;
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid Code Line";
            }
            else
            {
                return "Code Line";
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            if (_type == CodeType.Declaration)
            {
                return _code;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            if (_type == CodeType.Instruction)
            {
                return _code;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Creates declarations in the RAPID program module inside the RAPID Generator. 
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDDeclaration(RAPIDGenerator RAPIDGenerator)
        {
            if (_type == CodeType.Declaration)
            {
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + _code);
            }
        }

        /// <summary>
        /// Creates instructions in the RAPID program module inside the RAPID Generator.
        /// This method is called inside the RAPID generator.
        /// </summary>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public override void ToRAPIDInstruction(RAPIDGenerator RAPIDGenerator)
        {
            if (_type == CodeType.Instruction)
            {
                RAPIDGenerator.StringBuilder.Append(Environment.NewLine + "\t\t" + _code);
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public override bool IsValid
        {
            get 
            { 
                if (Code == null) { return false; }
                if (Code == "") { return false; }
                return true; 
            }
        }

        /// <summary>
        /// Gets or sets the custom RAPID Code Line text.
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// Gets or sets the Code Type.
        /// </summary>
        public CodeType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        #endregion
    }

}
