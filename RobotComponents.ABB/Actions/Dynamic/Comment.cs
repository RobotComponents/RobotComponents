// This file is part of Robot Components. Robot Components is licensed under 
// the terms of GNU Lesser General Public License version 3.0 (LGPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// RobotComponents Libs
using RobotComponents.ABB.Definitions;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Actions.Interfaces;

namespace RobotComponents.ABB.Actions.Dynamic
{
    /// <summary>
    /// Represents a comment in RAPID Code.
    /// This action is only used to make the program easier to understand. 
    /// It has no effect on the execution of the program.
    /// </summary>
    [Serializable()]
    public class Comment : Action, IDynamic, ISerializable
    {
        #region fields
        private string _comment; // the comment as a string
        private CodeType _type; // the comment type as a CodeType enum
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected Comment(SerializationInfo info, StreamingContext context)
        {
            // int version = (int)info.GetValue("Version", typeof(int)); // <-- use this if the (de)serialization changes
            _comment = (string)info.GetValue("Comment", typeof(string));
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
            info.AddValue("Comment", _comment, typeof(string));
            info.AddValue("Code Type", _type, typeof(CodeType));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Comment class. 
        /// </summary>
        public Comment()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Comment class with the Code Type set as instruction.
        /// </summary>
        /// <param name="comment"> The comment. </param>
        public Comment(string comment)
        {
            _comment = comment;
            _type = CodeType.Instruction;
        }

        /// <summary>
        /// Initializes a new instance of the Comment class.
        /// </summary>
        /// <param name="comment"> the comment. </param>
        /// <param name="type"> The Code Type. </param>
        public Comment(string comment, CodeType type)
        {
            _comment = comment;
            _type = type;
        }

        /// <summary>
        /// Initializes a new instance of the Comment class by duplicating an existing Comment instance. 
        /// </summary>
        /// <param name="comment"> The Comment instance to duplicate. </param>
        public Comment(Comment comment)
        {
            _comment = comment.Com;
            _type = comment.Type;
        }

        /// <summary>
        /// Returns an exact duplicate of this Comment instance.
        /// </summary>
        /// <returns> A deep copy of the Comment instance. </returns>
        public Comment Duplicate()
        {
            return new Comment(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Comment instance as IDynamic. 
        /// </summary>
        /// <returns> A deep copy of the Comment instance as an IDynamic. </returns>
        public IDynamic DuplicateDynamic()
        {
            return new Comment(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Comment instance as an Action. 
        /// </summary>
        /// <returns> A deep copy of the Comment instance as an Action. </returns>
        public override Action DuplicateAction()
        {
            return new Comment(this);
        }
        #endregion

        #region method
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(_comment))
            {
                return "Empty Comment";
            }
            else if (!IsValid)
            {
                return "Invalid Comment";
            }
            else
            {
                return "Comment";
            }
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDDeclaration(Robot robot)
        {
            return _type == CodeType.Declaration ? $"! {_comment}" : "";
        }

        /// <summary>
        /// Returns the RAPID instruction code line of the this action. 
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> The RAPID code line. </returns>
        public override string ToRAPIDInstruction(Robot robot)
        {
            return _type == CodeType.Instruction ? $"! {_comment}" : "";
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
                if (_comment != "")
                {
                    RAPIDGenerator.ProgramDeclarationComments.Add("    " + $"! {_comment}");
                }
                else
                {
                    RAPIDGenerator.ProgramDeclarationComments.Add("    ");
                }
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
                if (_comment != "")
                {
                    RAPIDGenerator.ProgramInstructions.Add("    " + "    " + $"! {_comment}");
                }
                else
                {
                    RAPIDGenerator.ProgramInstructions.Add("    " + "    ");
                }
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
                if (_comment == null) { return false; }
                if (_comment == "") { return false; }
                return true; 
            }
        }

        /// <summary>
        /// Gets or sets the comment text.
        /// </summary>
        public string Com
        {
            get { return _comment; }
            set { _comment = value; }
        }

        /// <summary>
        /// Gets or sets the comment Code Type.
        /// </summary>
        public CodeType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        #endregion
    }

}