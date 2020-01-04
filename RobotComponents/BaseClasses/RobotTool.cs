using Rhino.Geometry;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// RobotTool class, defines the basic properties and methods for any RobotTool.
    /// </summary>
    public class RobotTool
    {
        #region fields
        private string _name;
        private Mesh _mesh;
        private Plane _attachmentPlane;
        private Plane _toolPlane;

        // specific Tool Data for ABB ToolData RoboWare
        private bool _robotHold;
        private Vector3d _offset;
        private Quaternion _orientation;
        private double _mass;
        private Vector3d _cog; // center of gravity
        private Quaternion _cogOrientation;
        private Vector3d _inertia;
        #endregion

        #region constructors
        public RobotTool()
        {
            this._name = "tool0";
            this._mesh = new Mesh();
            this._attachmentPlane = Plane.WorldXY;
            this._toolPlane = Plane.WorldXY;

            this._robotHold = true;
            this._offset = new Vector3d(_toolPlane.Origin - _attachmentPlane.Origin);
            this._orientation = Quaternion.Rotation(_attachmentPlane, _toolPlane);
            this._mass = 1;
            this._cog = new Vector3d(0, 0, 0);
            this._cogOrientation = new Quaternion(1, 0, 0, 0);
            this._inertia = new Vector3d(0, 0, 0);
        }

        public RobotTool(string name, Mesh mesh, Plane attachmentPlane, Plane toolPlane)
        {
            this._name = name;
            this._mesh = mesh;
            this._attachmentPlane = attachmentPlane;
            this._toolPlane = toolPlane;

            this._robotHold = true;
            this._offset = new Vector3d(_toolPlane.Origin - _attachmentPlane.Origin);
            this._orientation = Quaternion.Rotation(_attachmentPlane, _toolPlane);
            this._mass = 1;
            this._cog = new Vector3d(0, 0, 0);
            this._cogOrientation = new Quaternion(1, 0, 0, 0);
            this._inertia = new Vector3d(0, 0, 0);
        }

        public RobotTool(string name, Mesh mesh, Vector3d offset, Quaternion orientation, 
            bool robotHold = true, double mass = 1, Vector3d cog = new Vector3d(), Quaternion cogOrientation = new Quaternion(), Vector3d inertia = new Vector3d())
        {
            this._name = name;
            this._mesh = mesh;

            this._robotHold = true;
            this._offset = offset;
            this._orientation = orientation;

            this._attachmentPlane = Plane.WorldXY;
            this._toolPlane = Plane.WorldXY;
            _toolPlane.Transform(orientation.MatrixForm());

            this._mass = mass;
            this._cog = cog;
            this._cogOrientation = cogOrientation;
            this._inertia = inertia;
        }

        public RobotTool Duplicate()
        {
            RobotTool dup = new RobotTool(Name, Mesh, AttachmentPlane, ToolPlane);
            return dup;
        }
        #endregion

        #region methods
        public string GetRSToolData()
        {
            string result = "";
            result = CreateToolDataString(this._offset, this._orientation, this._robotHold, this._mass, this._cog, this._cogOrientation, this._inertia);
            return result;
        }

        private string CreateToolDataString(Vector3d offset, Quaternion orientation, bool robotHold = true, double mass = 1, 
            Vector3d cog = new Vector3d(), Quaternion cogOrientation = new Quaternion(), Vector3d inertia = new Vector3d())
        {
            string result = "";

            // Add robot hold < robhold of bool >
            if (robotHold)
            {
                result = "[TRUE,[[";
            }
            else
            {
                result = "[FALSE,[[";
            }

            // Add coordinate of toolframe < tframe of pose > < trans of pos >
            result += offset.X.ToString("0.######") 
                + "," + offset.Y.ToString("0.######") 
                + "," + offset.Z.ToString("0.######") + "],[";

            // Add orientation of tool frame < tframe of pose > < rot of orient >
            result += orientation.A.ToString("0.######") 
                + "," + orientation.B.ToString("0.######") 
                + "," + orientation.C.ToString("0.######") 
                + "," + orientation.D.ToString("0.######") + "]],[";

            // Add tool load < tload of loaddata >
            result += mass.ToString("0.######") + ",[" 

                + cog.X.ToString("0.######") + "," 
                + cog.Y.ToString("0.######") + "," 
                + cog.Z.ToString("0.######") + "],["

                + cogOrientation.A.ToString("0.######") + "," 
                + cogOrientation.B.ToString("0.######") + "," 
                + cogOrientation.C.ToString("0.######") + "," 
                + cogOrientation.D.ToString("0.######") + ",]," 

                + inertia.X.ToString("0.######") + "," 
                + inertia.Y.ToString("0.######") + "," 
                + inertia.Z.ToString("0.######") + "]]";

            return result;
        }

        /// <summary>
        /// Clear / unset all the fields and properties of the object.
        /// Used for constructing an empty Robot Tool object. 
        /// Since our empty constructor creates the default Robot Tool tool0.
        /// </summary>
        public void Clear()
        {
            _name = "";
            _mesh = new Mesh();
            _attachmentPlane = Plane.Unset;
            _toolPlane = Plane.Unset;
            _robotHold = false;
            _offset = Vector3d.Unset;
            _orientation = Quaternion.Zero;
            _mass = 0; 
            _cog = Vector3d.Unset;
            _cogOrientation = Quaternion.Zero;
            _inertia = Vector3d.Unset;
    }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (AttachmentPlane == null) { return false; }
                if (ToolPlane == null) { return false; }

                return true;
            }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        public Plane AttachmentPlane
        {
            get { return _attachmentPlane; }
            set { _attachmentPlane = value; }
        }

        public Plane ToolPlane
        {
            get { return _toolPlane; }
            set { _toolPlane = value; }
        }

        public bool RobotHold
        {
            get { return _robotHold; }
            set { _robotHold = value; }
        }

        public Vector3d Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public Quaternion Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        public double Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }

        public Vector3d Cog
        {
            get { return _cog; }
            set { _cog = value; }
        }

        public Quaternion CogOrientation
        {
            get { return _cogOrientation; }
            set { _cogOrientation = value; }
        }

        public Vector3d Inertia
        {
            get { return _inertia; }
            set { _inertia = value; }
        }
        #endregion
    }

}
