using Rhino.Geometry;

namespace RobotComponents.BaseClasses
{
    /// <summary>
    /// Work Object class
    /// </summary>
    public class WorkObject
    {
        #region fields
        private string _name;
        private Plane _plane;
        private ExternalAxis _externalAxis; // coupled with external axis: e.g. rotational poistioner, but can also be combined with a linear axis that moves the work object. 
        private Quaternion _orientation;
        private bool _robotHold;
        private Plane _userFrame;

        #endregion

        #region constructors
        public WorkObject()
        {
            _name = "wobj0";
            _plane = Plane.WorldXY;
            _externalAxis = null; // To do: shoud implement that an external axis can be null
            _robotHold = false;
            _userFrame = Plane.WorldXY;
            Initilize();
        }

        public WorkObject(string name, Plane plane)
        {
            _name = name;
            _plane = plane;
            _externalAxis = null;  // To do: shoud implement that an external axis can be null
            _robotHold = false;
            _userFrame = Plane.WorldXY;
            Initilize();
        }

        public WorkObject(string name, Plane plane, ExternalAxis externalAxis)
        {
            _name = name;
            _plane = plane;
            _externalAxis = externalAxis;
            _robotHold = false;
            _userFrame = Plane.WorldXY;
            Initilize();
        }

        public WorkObject Duplicate()
        {
            WorkObject dup = new WorkObject(Name, Plane, ExternalAxis);
            return dup;
        }
        #endregion

        #region methods
        public bool IsValid
        {
            get
            {
                if (Plane == null) { return false; }
                return true;
            }
        }

        public void GetOrientation()
        {
            Orientation = Quaternion.Rotation(Plane.WorldXY, _plane); // Todo: needs to be checked, is this correct?

            // From target class: as example
            // Plane refPlane = new Plane(Plane.WorldXY);
            // Quaternion quat = Quaternion.Rotation(refPlane, _plane);
            // double[] quaternion = new double[] { quat.A, quat.B, quat.C, quat.D };
            // return quat;
        }

        public void Initilize()
        {
            GetOrientation();
        }

        public void ReInitilize()
        {
            Initilize();
        }
        public string GetWorkObjData()
        {
            string result = "";
            result = CreateWorkObjString();
            return result;
        }

        private string CreateWorkObjString()
        {
            string result = " ";


            // Add robot hold < robhold of bool >
            if (RobotHold)
            {
                result = "[TRUE, ";
            }
            else
            {
                result = "[FALSE, ";
            }

            // Add User frame < ufprog of bool >
            // Todo..
            // result +=

            // Add mechanical unit (an external axis or robot) < ufmec of string >
            // Todo..
            // result +=

            // Add user frame coordinate < uframe of pose > < trans of pos >
            // Todo..
            // result +=

            // Add user frame orientation < uframe of pose > < rot of orient >
            // Todo..
            // result +=

            // Add object frame coordinate < oframe of pose > < trans of pos >
            // Todo..
            // result +=

            // Add object frame orientation < oframe of pose > < rot of orient >
            // Todo..
            // result +=

            return result;
        }

        #endregion

        #region properties
        public string Name 
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool RobotHold
        {
            get { return _robotHold; }
            set { _robotHold = value; }
        }

        public Plane UserFrame 
        { 
            get {return _userFrame; }
            set { _userFrame = value; }
        }

        public Plane Plane
        {
            get { return _plane; }
            set { _plane = value; }
        }

        public ExternalAxis ExternalAxis 
        {
            get { return _externalAxis; } 
            set { _externalAxis = value; } 
        }

        public Quaternion Orientation 
        {
            get { return _orientation; }
            set { _orientation = value; }
        }
        #endregion
    }
}
