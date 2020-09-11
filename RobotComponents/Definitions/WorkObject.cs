// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/RobotComponents/RobotComponents>.

// System lib
using System;
// Rhino Libs
using Rhino.Geometry;
// Robot Components Libs
using RobotComponents.Enumerations;

namespace RobotComponents.Definitions
{
    /// <summary>
    /// The WorkObject class creates the work object data for the RAPID base code.
    /// Work object data is used to describe the work object that the robot welds, processes, moves within, etc.
    /// The work object is typically combined with a robot movement to defined the global coordinate of the robot target. 
    /// </summary>
    public class WorkObject
    {
        #region fields
        private ReferenceType _referenceType; // reference type
        private string _name; // The work object name
        private Plane _plane; // The work object coordinate system
        private Quaternion _orientation; // The orientation of the work object coordinate system
        private ExternalAxis _externalAxis; // The coupled mechanical unit
        private bool _robotHold; // Bool that indicates if the robot holds the work object
        private bool _fixedFrame; // Bool that indicates if the workobject is fixed (true) or movable (false)
        private Plane _userFrame; // The user frame coordinate system
        private Quaternion _userFrameOrientation; // the orienation of the user frame coordinate system
        private Plane _globalPlane; // global work object plane
        #endregion

        #region constructors
        /// <summary>
        /// An empty constructr that creates the the work object data wobj0 in such a way 
        /// that the object coordinate system coincides with the world coordinate system. 
        /// The robot does not hold the work object. 
        /// </summary>
        public WorkObject()
        {
            _referenceType = ReferenceType.PERS;
            _name = "wobj0";
            _plane = Plane.WorldXY;
            _externalAxis = null;
            _robotHold = false;
            _userFrame = Plane.WorldXY;

            Initialize();
        }

        /// <summary>
        /// The constructor to create a fixed user defined work object coordinate system. 
        /// </summary>
        /// <param name="name"> The work object name. </param>
        /// <param name="plane"> The work object coorindate system as a Plane. </param>
        public WorkObject(string name, Plane plane)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _plane = plane;
            _externalAxis = null;
            _robotHold = false;
            _userFrame = Plane.WorldXY;

            Initialize();
        }

        /// <summary>
        /// The constructor to create a movable usre definied work object coordinate system.
        /// </summary>
        /// <param name="name"> The work object name. </param>
        /// <param name="plane"> The work object coorindate system as a Plane. </param>
        /// <param name="externalAxis"> The coupled external axis (mechanical unit) that moves the work object. </param>
        public WorkObject(string name, Plane plane, ExternalAxis externalAxis)
        {
            _referenceType = ReferenceType.PERS;
            _name = name;
            _plane = plane;
            _externalAxis = externalAxis;
            _robotHold = false;
            _userFrame = Plane.WorldXY;

            Initialize();
        }

        /// <summary>
        /// Creates a new work object by duplicating an existing movement.
        /// This creates a deep copy of the existing work object.
        /// </summary>
        /// <param name="workObject"> The work object that should be duplicated. </param>
        /// <param name="duplicateMesh"> A boolean that indicates if the meshes should be duplicated. </param>
        public WorkObject(WorkObject workObject, bool duplicateMesh = true)
        {
            _referenceType = workObject.ReferenceType;
            _name = workObject.Name;
            _plane = new Plane(workObject.Plane);
            _userFrame = new Plane(workObject.UserFrame);
            _globalPlane = new Plane(workObject.GlobalWorkObjectPlane);
            _userFrameOrientation = workObject.UserFrameOrientation;
            _orientation = workObject.Orientation;
            _robotHold = workObject.RobotHold;
            _fixedFrame = workObject.FixedFrame;

            if (workObject.ExternalAxis == null) { _externalAxis = null; }
            else if (duplicateMesh == true) { _externalAxis = workObject.ExternalAxis.DuplicateExternalAxis(); }
            else { _externalAxis = workObject.ExternalAxis.DuplicateExternalAxisWithoutMesh(); }          
        }

        /// <summary>
        /// A method to duplicate the WorkObject object. 
        /// </summary>
        /// <returns> Returns a deep copy of the WorkObject object. </returns>
        public WorkObject Duplicate()
        {
            return new WorkObject(this);
        }

        /// <summary>
        /// A method to duplicate the WorkObject object without meshes. 
        /// </summary>
        /// <returns> Returns a deep copy of the WorkObject object with an external axis with empty meshes. </returns>
        public WorkObject DuplicateWithoutMesh()
        {
            return new WorkObject(this, false);
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            if (!this.IsValid)
            {
                return "Invalid Work Object";
            }
            else if (this.FixedFrame == false)
            {
                return "Movable Work Object (" + this.Name + ")";
            }
            else
            {
                return "Work Object (" + this.Name + ")";
            }
        }

        /// <summary>
        /// Method that calculates the quaternion orientation of the work object coordinate system. 
        /// </summary>
        /// <returns> Returns the quaternion orientation of the work object. </returns>
        public Quaternion GetOrientation()
        {
            _orientation = Quaternion.Rotation(Plane.WorldXY, _plane);
            return _orientation;
        }

        /// <summary>
        /// Method that calculates the quaternion orientation of the user frame coordinate system. 
        /// </summary>
        /// <returns> Returns the quaternion orientation of the user frame. </returns>
        public Quaternion GetUserFrameOrientation()
        {
            _userFrameOrientation = Quaternion.Rotation(Plane.WorldXY, _userFrame);
            return _userFrameOrientation;
        }

        /// <summary>
        /// Calculates the global work object plane since the work object coordinate system and 
        /// the user frame coordinate system can both be un equal to the worldc coordinate system. 
        /// </summary>
        /// <returns> Returns the global work object plane. </returns>
        public Plane GetGlobalWorkObjectPlane()
        {
            // Create a deep copy of the work object plane
            _globalPlane = new Plane(_plane);

            // Re-orient the plane
            Transform orient1 = Transform.PlaneToPlane(Plane.WorldXY, _userFrame);
            _globalPlane.Transform(orient1);

            // Re-orient again if an external axis is used
            if (_externalAxis != null)
            {
                if (_externalAxis is ExternalRotationalAxis)
                {
                    // For a external rotational axis the coordinate system of the work object plane
                    // is definied in the coordinate system of teh external rotational axis (the axis plane)
                    Transform orient2 = Transform.PlaneToPlane(Plane.WorldXY, _externalAxis.AxisPlane);
                    _globalPlane.Transform(orient2);
                }
                else if (_externalAxis is ExternalLinearAxis)
                {
                    //TODO...
                }
            }

            return _globalPlane;
        }

        /// <summary>
        /// A method that calls all the other methods that are needed to initialize the data that is needed to construct a valid work object. 
        /// </summary>
        private void Initialize()
        {
            GetOrientation();
            GetUserFrameOrientation();
            GetGlobalWorkObjectPlane();

            // Set to a movable frame if an exernal axes is coupled
            if (_externalAxis != null)
                _fixedFrame = false;
            else
                _fixedFrame = true;
        }

        /// <summary>
        /// A method that can be called to reinitialize all the data that is needed to construct a valid work object. 
        /// </summary>
        public void ReInitialize()
        {
            Initialize();
        }

        /// <summary>
        /// Method for creating the work object data for the system BASE code. 
        /// </summary>
        /// <returns> Returns the work object BASE code as a string. </returns>
        public string GetWorkObjData()
        {
            string result = CreateWorkObjString();
            return result;
        }

        /// <summary>
        /// Private method for creating the work object data for the system BASE code. 
        /// </summary>
        /// <returns> Returns the work object BASE code as a string. </returns>
        private string CreateWorkObjString()
        {
            string result = "";

            // Adds variable type
            result += Enum.GetName(typeof(ReferenceType), _referenceType);
            result += " wobjdata ";

            // Adds work object name
            result += $"{_name} := ";

            // Add robot hold < robhold of bool >
            if (_robotHold)
            {
                result += "[TRUE, ";
            }
            else
            {
                result += "[FALSE, ";
            }

            // Add User frame type < ufprog of bool >
            if (_fixedFrame)
            {
                result += "TRUE, ";
            }
            else
            {
                result += "FALSE, ";
            }

            // Add mechanical unit (an external axis or robot) < ufmec of string >            
            if (_externalAxis != null)
            {
                result += $"\"{_externalAxis.Name}\", ";
            }
            else
            {
                result += "\"\", ";
            }
            
            // Add user frame coordinate < uframe of pose > < trans of pos >
            result += $"[[{_userFrame.Origin.X:0.####}, {_userFrame.Origin.Y:0.####}, {_userFrame.Origin.Z:0.####}], ";

            // Add user frame orientation < uframe of pose > < rot of orient >
            result += $"[{_userFrameOrientation.A:0.#######}, {_userFrameOrientation.B:0.#######}, " +
                $"{_userFrameOrientation.C:0.#######}, {_userFrameOrientation.D:0.#######}]], ";

            // Add object frame coordinate < oframe of pose > < trans of pos >
            result += $"[[{_plane.Origin.X:0.####}, {_plane.Origin.Y:0.####}, {_plane.Origin.Z:0.####}], ";

            // Add object frame orientation < oframe of pose > < rot of orient >
            result += $"[{_orientation.A:0.#######}, {_orientation.B:0.#######}, " +
                $"{_orientation.C:0.#######}, {_orientation.D:0.#######}]]];";

            return result;
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the WorkObject object is valid. 
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (Name == null) { return false;  }
                if (Name == "") { return false; }
                if (Plane == null) { return false; }
                if (Plane == Plane.Unset) { return false; }
                if (UserFrame == null) {return false; }
                if (UserFrame == Plane.Unset) { return false;  }
                return true;
            }
        }

        /// <summary>
        /// Defines the reference type (PERS, VAR or CONST)
        /// </summary>
        public ReferenceType ReferenceType
        {
            get { return _referenceType; }
            set { _referenceType = value; }
        }

        /// <summary>
        /// The name of the work object.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Defines whether or not the robot in the actual program task is holding the work object. 
        /// </summary>
        public bool RobotHold
        {
            get { return _robotHold; }
            set { _robotHold = value; }
        }

        /// <summary>
        /// Returns the global work object plane since the work object coordinate system and 
        /// the user frame coordinate system can both be unequal to the world coordinate system.
        /// </summary>
        public Plane GlobalWorkObjectPlane
        {
            get { return _globalPlane; }
        }

        /// <summary>
        /// The user coordinate system, i.e. the position of the current work surface or fixture.
        /// If the robot is holding the tool, the user coordinate system is defined in the world 
        /// coordinate system (in the wrist coordinate system if a stationary tool is used). For 
        /// movable user frame (FixedFrame = false), the user frame is continuously defined by 
        /// the system.
        /// </summary>
        public Plane UserFrame
        {
            get 
            { 
                return _userFrame; 
            }
            set 
            { 
                _userFrame = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// The object coordinate system as a plane (e.g. the position of the current work object).
        /// The object coordinate system is defined in the user coordinate system.
        /// </summary>
        public Plane Plane
        {
            get 
            { 
                return _plane; 
            }
            set 
            { 
                _plane = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// The external axis (mechanical unit) with which the robot movements are coordinated. 
        /// Only specified in the case of movable user coordinate systems
        /// </summary>
        public ExternalAxis ExternalAxis
        {
            get 
            { 
                return _externalAxis; 
            }
            set 
            { 
                _externalAxis = value;
                ReInitialize();
            }
        }

        /// <summary>
        /// The Quaternion orientation of the work object coordinate system.
        /// </summary>
        public Quaternion Orientation
        {
            get { return _userFrameOrientation; }
        }

        /// <summary>
        /// The Quaternion orientation of the user frame coordinate system.
        /// </summary>
        public Quaternion UserFrameOrientation
        {
            get { return _userFrameOrientation; }
        }

        /// <summary>
        /// Defines whether or not a fixed user coordinate system is used.
        /// True indicates that the user frame is fixed. 
        /// False indicates that the user coordinate system is movable (e.g. coordinated external axes).
        /// </summary>
        public bool FixedFrame
        {
            get { return _fixedFrame; }
            set { _fixedFrame = value; }
        }
        #endregion
    }
}
