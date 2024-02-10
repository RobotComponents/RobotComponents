// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Rhino Libs
using Rhino.Geometry;
// RobotComponents Libs
using RobotComponents.ABB.Actions;
using RobotComponents.ABB.Actions.Interfaces;
using RobotComponents.ABB.Enumerations;
using RobotComponents.ABB.Utils;

namespace RobotComponents.ABB.Definitions
{
    /// <summary>
    /// Represents a Robot Tool.
    /// </summary>
    [Serializable()]
    public class RobotTool : ISerializable, IDeclaration
    {
        #region fields
        private Scope _scope;
        private VariableType _variableType;
        private static readonly string _datatype = "tooldata";
        private string _name;
        private Mesh _mesh;
        private Plane _attachmentPlane;
        private Plane _toolPlane;
        private bool _robotHold;
        private Point3d _position;
        private Quaternion _orientation;
        private LoadData _loadData;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected RobotTool(SerializationInfo info, StreamingContext context)
        {
            //Version version = (Version)info.GetValue("Version", typeof(Version)); // <-- use this if the (de)serialization changes
            _scope = (Scope)info.GetValue("Scope", typeof(Scope));
            _variableType = (VariableType)info.GetValue("Variable Type", typeof(VariableType));
            _name = (string)info.GetValue("Name", typeof(string));
            _mesh = (Mesh)info.GetValue("Mesh", typeof(Mesh));
            _attachmentPlane = (Plane)info.GetValue("Attachment Plane", typeof(Plane));
            _toolPlane = (Plane)info.GetValue("Tool Plane", typeof(Plane));
            _robotHold = (bool)info.GetValue("Robot Hold", typeof(bool));
            _loadData = (LoadData)info.GetValue("Load Data", typeof(LoadData));

            Initialize();
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VersionNumbering.Version, typeof(Version));
            info.AddValue("Scope", _scope, typeof(Scope));
            info.AddValue("Variable Type", _variableType, typeof(VariableType));
            info.AddValue("Name", _name, typeof(string));
            info.AddValue("Mesh", _mesh, typeof(Mesh));
            info.AddValue("Attachment Plane", _attachmentPlane, typeof(Plane));
            info.AddValue("Tool Plane", _toolPlane, typeof(Plane));
            info.AddValue("Robot Hold", _robotHold, typeof(bool));
            info.AddValue("Load Data", _loadData, typeof(LoadData));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the Robot Tool class with the default tool tool0.
        /// </summary>
        public RobotTool()
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
            _name = "tool0";
            _mesh = new Mesh();
            _attachmentPlane = Plane.WorldXY;
            _toolPlane = Plane.WorldXY;
            _robotHold = true;

            _loadData = new LoadData
            {
                Name = ""
            };

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class from planes.
        /// </summary>
        /// <remarks>
        /// Sets the loaddata as load0.
        /// </remarks>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="mesh"> The tool mesh. </param>
        /// <param name="attachmentPlane"> The attachement plane. </param>
        /// <param name="toolPlane"> The tool center point and tool orientation as a plane. </param>
        public RobotTool(string name, Mesh mesh, Plane attachmentPlane, Plane toolPlane)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
            _name = name;
            _mesh = mesh;
            _attachmentPlane = attachmentPlane;
            _toolPlane = toolPlane;
            _robotHold = true;

            _loadData = new LoadData
            {
                Name = ""
            };

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class from planes.
        /// </summary>
        /// <remarks>
        /// Sets the loaddata as load0.
        /// </remarks>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="meshes"> The tool mesh as The list with robot meshes. </param>
        /// <param name="attachmentPlane"> The attachement plane. </param>
        /// <param name="toolPlane"> The tool center point and tool orientation as a plane. </param>
        public RobotTool(string name, IList<Mesh> meshes, Plane attachmentPlane, Plane toolPlane)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
            _name = name;
            _mesh = new Mesh();

            for (int i = 0; i < meshes.Count; i++)
            {
                _mesh.Append(meshes[i]);
            }

            _attachmentPlane = attachmentPlane;
            _toolPlane = toolPlane;
            _robotHold = true;

            _loadData = new LoadData
            {
                Name = ""
            };

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class from planes.
        /// </summary>
        /// <remarks>
        /// Sets the loaddata as load0.
        /// </remarks>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="mesh"> The tool mesh. </param>
        /// <param name="attachmentPlane"> The attachement plane. </param>
        /// <param name="toolPlane"> The tool center point and tool orientation as a plane. </param>
        /// <param name="loadData"> The tool loaddata as load data. </param>
        public RobotTool(string name, Mesh mesh, Plane attachmentPlane, Plane toolPlane, LoadData loadData)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
            _name = name;
            _mesh = mesh;
            _attachmentPlane = attachmentPlane;
            _toolPlane = toolPlane;
            _robotHold = true;
            _loadData = loadData.Duplicate();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class from planes.
        /// </summary>
        /// <remarks>
        /// Sets the loaddata as load0.
        /// </remarks>
        /// <param name="name"> The tool name, must be unique. </param>
        /// <param name="meshes"> The tool mesh as The list with robot meshes. </param>
        /// <param name="attachmentPlane"> The attachement plane. </param>
        /// <param name="toolPlane"> The tool center point and tool orientation as a plane. </param>
        /// <param name="loadData"> The tool loaddata as load data. </param>
        public RobotTool(string name, IList<Mesh> meshes, Plane attachmentPlane, Plane toolPlane, LoadData loadData)
        {
            _scope = Scope.GLOBAL;
            _variableType = VariableType.PERS;
            _name = name;
            _mesh = new Mesh();

            for (int i = 0; i < meshes.Count; i++)
            {
                _mesh.Append(meshes[i]);
            }

            _attachmentPlane = attachmentPlane;
            _toolPlane = toolPlane;
            _robotHold = true;
            _loadData = loadData.Duplicate();

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Robot Tool class by duplicating an existing Robot Tool instance. 
        /// </summary>
        /// <param name="robotTool"> The Robot Tool instance to duplicate. </param>
        /// <param name="duplicateMesh"> Specifies whether the meshes should be duplicated. </param>
        public RobotTool(RobotTool robotTool, bool duplicateMesh = true)
        {
            _scope = robotTool.Scope;
            _variableType = robotTool.VariableType;
            _name = robotTool.Name;
            _attachmentPlane = new Plane(robotTool.AttachmentPlane);
            _toolPlane = new Plane(robotTool.ToolPlane);
            _robotHold = robotTool.RobotHold;
            _loadData = robotTool.LoadData.Duplicate();
            _position = new Point3d(robotTool.Position);
            _orientation = robotTool.Orientation;

            if (duplicateMesh == true) { _mesh = robotTool.Mesh.DuplicateMesh(); }
            else { } //_mesh = new Mesh(); }
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot Tool instance.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Robot Tool instance. 
        /// </returns>
        public RobotTool Duplicate()
        {
            return new RobotTool(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot Tool instance without meshes.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Robot Tool instance without meshes. 
        /// </returns>
        public RobotTool DuplicateWithoutMesh()
        {
            return new RobotTool(this, false);
        }

        /// <summary>
        /// Returns an exact duplicate of this Robot Tool instance as an IDeclaration.
        /// </summary>
        /// <returns> 
        /// A deep copy of the Robot Tool instance as an IDeclaration. 
        /// </returns>
        public IDeclaration DuplicateDeclaration()
        {
            return new RobotTool(this);
        }

        /// <summary>
        /// Returns an empty Robot Tool instance.
        /// </summary>
        /// <returns> 
        /// The empty Robot Tool.
        /// </returns>
        public static RobotTool GetEmptyRobotTool()
        {
            RobotTool robotTool = new RobotTool();
            robotTool.Clear();

            return robotTool;
        }
        #endregion

        #region parse
        /// <summary>
        /// Initializes a new instance of the Robot Tool class from a rapid data string.
        /// </summary>
        /// <remarks>
        /// Only used for the Parse and TryParse methods. Therefore, this constructor is private. 
        /// </remarks>
        /// <param name="rapidData"> The RAPID data string. </param>
        private RobotTool(string rapidData)
        {
            this.SetRapidDataFromString(rapidData, out string[] values);

            if (values.Length == 19)
            {
                _robotHold = values[0] == "TRUE";

                _position = new Point3d
                {
                    X = double.Parse(values[1]),
                    Y = double.Parse(values[2]),
                    Z = double.Parse(values[3])
                };

                _orientation = new Quaternion
                {
                    A = double.Parse(values[4]),
                    B = double.Parse(values[5]),
                    C = double.Parse(values[6]),
                    D = double.Parse(values[7])
                };

                _loadData = new LoadData
                {
                    Name = "",
                    Mass = double.Parse(values[8])
                };

                _loadData.CenterOfGravity = new Point3d
                {
                    X = double.Parse(values[9]),
                    Y = double.Parse(values[10]),
                    Z = double.Parse(values[11])
                };

                _loadData.AxesOfMoment = new Quaternion
                {
                    A = double.Parse(values[12]),
                    B = double.Parse(values[13]),
                    C = double.Parse(values[14]),
                    D = double.Parse(values[15])
                };

                _loadData.InertialMoments = new Vector3d
                {
                    X = double.Parse(values[16]),
                    Y = double.Parse(values[17]),
                    Z = double.Parse(values[18])
                };
            }
            else
            {
                throw new InvalidCastException("Invalid RAPID data string: The number of values does not match.");
            }

            _mesh = new Mesh();
            _attachmentPlane = Plane.WorldXY;
            _toolPlane = HelperMethods.QuaternionToPlane(_attachmentPlane, _position, _orientation);
        }

        /// <summary>
        /// Returns a Robot Tool instance constructed from a RAPID data string. 
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. s</param>
        public static RobotTool Parse(string rapidData)
        {
            return new RobotTool(rapidData);
        }

        /// <summary>
        /// Attempts to parse a RAPID data string into a Robot Tool instance.  
        /// </summary>
        /// <param name="rapidData"> The RAPID data string. </param>
        /// <param name="robotTool"> The Robot Tool intance. </param>
        /// <returns> 
        /// True on success, false on failure. 
        /// </returns>
        public static bool TryParse(string rapidData, out RobotTool robotTool)
        {
            try
            {
                robotTool = new RobotTool(rapidData);
                return true;
            }
            catch
            {
                robotTool = new RobotTool();
                return false;
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> 
        /// A string that represents the current object. 
        /// </returns>
        public override string ToString()
        {
            if (!IsValid)
            {
                return "Invalid Robot Tool";
            }
            else
            {
                return $"Robot Tool ({_name})";
            }
        }

        /// <summary>
        /// Initializes the fields and properties to construct a valid Robot Tool istance.
        /// </summary>
        private void Initialize()
        {
            CalculateToolPosition();
            CalculateToolOrientation();
        }

        /// <summary>
        /// Reinitializes the fields and properties to construct a valid Robot Tool instance. 
        /// </summary>
        public void ReInitialize()
        {
            Initialize();
        }

        /// <summary>
        /// Calculates and returns the tool center point relative to the defined attachment plane. 
        /// </summary>
        /// <returns> 
        /// The tool center point. 
        /// </returns>
        public Point3d CalculateToolPosition()
        {
            Plane toolPlane = new Plane(_toolPlane);
            Transform orient = Rhino.Geometry.Transform.PlaneToPlane(_attachmentPlane, Plane.WorldXY);
            toolPlane.Transform(orient);

            _position = new Point3d(toolPlane.Origin);

            return _position;
        }

        /// <summary>
        /// Calculates and returns the tool center orientation relative to the defined attachment plane. 
        /// </summary>
        /// <returns> 
        /// The quaternion orientation of the tool center plane. 
        /// </returns>
        public Quaternion CalculateToolOrientation()
        {
            _orientation = HelperMethods.PlaneToQuaternion(_attachmentPlane, _toolPlane);

            return _orientation;
        }

        /// <summary>
        /// Returns the tooldata in RAPID code format, e.g. 
        /// </summary>
        /// <remarks>
        /// Example outputs are 
        /// "[TRUE, [[215.448, 3.171, 102.332], [0.5, 0.5, 0.5, 0.5]], [0.001, [0, 0, 0.001], [1, 0, 0, 0], 0, 0, 0]] and 
        /// "[TRUE, [[215.448, 3.171, 102.332], [0.5, 0.5, 0.5, 0.5]], load1]
        /// </remarks>
        /// <returns> 
        /// The string with tooldata values. 
        /// </returns>
        public string ToRAPID()
        {
            // Add robot hold < robhold of bool >
            string result = _robotHold ? "[TRUE, [[" : "[FALSE, [[";

            // Add coordinate of toolframe < tframe of pose > < trans of pos >
            result += $"{_position.X:0.###}, {_position.Y:0.###}, {_position.Z:0.###}], [";

            // Add orientation of tool frame < tframe of pose > < rot of orient >
            result += $"{_orientation.A:0.######}, {_orientation.B:0.######}, " +
                $"{_orientation.C:0.######}, {_orientation.D:0.######}]], ";

            // Add tool load < tload of loaddata >
            result += _loadData.ToRAPID();

            // Close
            result += "]";

            return result;
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this Robot Tool.
        /// </summary>
        /// <returns> 
        /// The RAPID code line. 
        /// </returns>
        public string ToRAPIDDeclaration()
        {
            string result = _scope == Scope.GLOBAL ? "" : $"{Enum.GetName(typeof(Scope), _scope)} ";
            result += $"{Enum.GetName(typeof(VariableType), _variableType)} {_datatype} {_name} := {ToRAPID()};";

            return result;
        }

        /// <summary>
        /// Returns the RAPID declaration code line of the this action.
        /// </summary>
        /// <param name="robot"> The Robot were the code is generated for. </param>
        /// <returns> 
        /// The RAPID code line in case a variable name is defined. 
        /// </returns>
        public string ToRAPIDDeclaration(Robot robot)
        {
            if (_name != "" && _name != "tool0")
            {
                return ToRAPIDDeclaration();
            }

            return string.Empty;
        }

        /// <summary>
        /// Creates declarations and instructions in the RAPID program module inside the RAPID Generator.
        /// </summary>
        /// <remarks>
        /// This method is called inside the RAPID generator.
        /// </remarks>
        /// <param name="RAPIDGenerator"> The RAPID Generator. </param>
        public void ToRAPIDGenerator(RAPIDGenerator RAPIDGenerator)
        {
            _loadData.ToRAPIDGenerator(RAPIDGenerator);

            if (_name != "" && _name != "tool0")
            {
                if (!RAPIDGenerator.RobotTools.ContainsKey(_name))
                {
                    RAPIDGenerator.RobotTools.Add(_name, this);
                    RAPIDGenerator.ProgramDeclarationsToolData.Add("    " + ToRAPIDDeclaration(RAPIDGenerator.Robot));
                }
            }
        }

        /// <summary>
        /// Clears all the fields and properties of the current instance.
        /// </summary>
        /// <remarks>
        /// Typically used for defining an empty Robot Tool instance 
        /// since the empty constructor creates the default Robot Tool tool0.
        /// </remarks>
        private void Clear()
        {
            _name = "";
            _mesh = new Mesh();
            _attachmentPlane = Plane.Unset;
            _toolPlane = Plane.Unset;
            _robotHold = false;
            _position = Point3d.Unset;
            _orientation = Quaternion.Zero;

            _loadData = new LoadData
            {
                Name = ""
            };
        }

        /// <summary>
        /// Transforms the Robot Tool spatial properties (planes and meshes). 
        /// </summary>
        /// <param name="xform"> The spatial deform. </param>
        public void Transform(Transform xform)
        {
            _mesh.Transform(xform);
            _attachmentPlane.Transform(xform);
            _toolPlane.Transform(xform);

            ReInitialize();
        }

        /// <summary>
        /// Returns the Bounding Box of the object.
        /// </summary>
        /// <param name="accurate"> If true, a physically accurate bounding box will be computed. If not, a bounding box estimate will be computed. </param>
        /// <returns> 
        /// The Bounding Box. 
        /// </returns>
        public BoundingBox GetBoundingBox(bool accurate)
        {
            BoundingBox boundingBox = BoundingBox.Empty;

            if (_mesh != null)
            {
                boundingBox.Union(_mesh.GetBoundingBox(accurate));
            }

            return boundingBox;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (_name == null) { return false; }
                if (_name == "") { return false; }
                if (_attachmentPlane == null) { return false; }
                if (_attachmentPlane == Plane.Unset) { return false; }
                if (_toolPlane == null) { return false; }
                if (_toolPlane == Plane.Unset) { return false; }
                if (_loadData.IsValid == false) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the scope. 
        /// </summary>
        public Scope Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        /// <summary>
        /// Gets or sets the variable type. 
        /// </summary>
        public VariableType VariableType
        {
            get { return _variableType; }
            set { _variableType = value; }
        }

        /// <summary>
        /// Gets the RAPID datatype. 
        /// </summary>
        public string Datatype
        {
            get { return _datatype; }
        }

        /// <summary>
        /// Gets or sets the tool name.
        /// </summary>
        /// <remarks>
        /// Each name has to be unique.
        /// </remarks>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the mesh.
        /// </summary>
        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        /// <summary>
        /// Gets or sets the tool attachment plane.
        /// </summary>
        public Plane AttachmentPlane
        {
            get
            {
                return _attachmentPlane;
            }
            set
            {
                _attachmentPlane = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets or sets the position and orientaton of the tool center point. 
        /// </summary>
        public Plane ToolPlane
        {
            get
            {
                return _toolPlane;
            }
            set
            {
                _toolPlane = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the robot is holding the tool.
        /// </summary>
        /// <remarks>
        /// Use true if the robot is holding the tool. 
        /// Use false if the robot is not holding the tool (e.g. stationary tool).
        /// </remarks>
        public bool RobotHold
        {
            get { return _robotHold; }
            set { _robotHold = value; }
        }

        /// <summary>
        /// Gets the position of the the tool center point which is the offset between the tool center plane and the attachment plane.
        /// </summary>
        public Point3d Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Gets the orientation of the tool center point as a Quaternion.
        /// </summary>
        public Quaternion Orientation
        {
            get { return _orientation; }
        }

        /// <summary>
        /// Gets or sets the load data.
        /// </summary>
        public LoadData LoadData
        {
            get { return _loadData; }
            set { _loadData = value; }
        }
        #endregion
    }
}