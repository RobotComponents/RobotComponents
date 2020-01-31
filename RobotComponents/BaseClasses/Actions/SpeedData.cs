using System;

using RobotComponents.BaseClasses.Definitions;

namespace RobotComponents.BaseClasses.Actions
{
    /// <summary>
    /// SpeedData class. SpeedData is used to specify the velocity at which both the robot and the external axes move.
    /// Speed data defines the velocity at which the tool center point moves, the reorientation speed of the tool, and 
    /// at which linear or rotating external axes move. When several different types of movement are combined, one of 
    /// the velocities often limits all movements.The velocity of the other movements will be reduced in such a way 
    /// that all movements will finish executing at the same time.
    /// </summary>
    public class SpeedData : Action
    {
        #region fields
        private string _name; // SpeeData variable name
        private double _v_tcp; // Tool center point speed
        private double _v_ori; // Re-orientation speed
        private double _v_leax; // External linear axis speed
        private double _v_reax; // External rotational axis speed
        private bool _predefined; // ABB predefinied data (e.g. v5, v10, v20, v30)?
        #endregion

        #region constructors
        /// <summary>
        /// Defines an empty SpeedData object. 
        /// </summary>
        public SpeedData()
        {
        }

        /// <summary>
        /// Constructor for creating a predefined SpeedData. ABB defined already a number of speed data in the system module.
        /// </summary>
        /// <param name="v_tcp">  The velocity of the tool center point (TCP) in mm/s. </param>
        public SpeedData(double v_tcp)
        {
            double tcp = Math.Round(v_tcp, 0);

            _name = "v" + tcp.ToString();
            _v_tcp = tcp;
            _v_ori = 500;
            _v_leax = 5000;
            _v_reax = 1000;
            _predefined = true;
        }

        /// <summary>
        /// Constructor for creating a predefined SpeedData. ABB defined already a number of speed data in the system module.
        /// </summary>
        /// <param name="v_tcp">  The velocity of the tool center point (TCP) in mm/s. </param>
        public SpeedData(int v_tcp)
        {
            double tcp = Convert.ToDouble(v_tcp);
            tcp = Math.Round(tcp, 0);

            _name = "v" + tcp.ToString();
            _v_tcp = tcp;
            _v_ori = 500;
            _v_leax = 5000;
            _v_reax = 1000;
            _predefined = true;
        }

        /// <summary>
        /// Constructor for creating an custom SpeedData. 
        /// </summary>
        /// <param name="name"> The SpeedData variable name. </param>
        /// <param name="v_tcp"> The velocity of the tool center point (TCP) in mm/s. </param>
        /// <param name="v_ori"> The reorientation velocity of the TCP expressed in degrees/s. </param>
        /// <param name="v_leax"> The velocity of linear external axes in mm/s. </param>
        /// <param name="v_reax"> The velocity of rotating external axes in degrees/s. </param>
        public SpeedData(string name, double v_tcp, double v_ori, double v_leax, double v_reax)
        {
            _name = name;
            _v_tcp = v_tcp;
            _v_ori = v_ori;
            _v_leax = v_leax;
            _v_reax = v_reax;
            _predefined = false;
        }

        /// <summary>
        /// Constructor used to duplicate the current SpeedData object. 
        /// </summary>
        /// <param name="name"> The SpeedData variable name. </param>
        /// <param name="v_tcp"> The velocity of the tool center point (TCP) in mm/s. </param>
        /// <param name="v_ori"> The reorientation velocity of the TCP expressed in degrees/s. </param>
        /// <param name="v_leax"> The velocity of linear external axes in mm/s. </param>
        /// <param name="v_reax"> The velocity of rotating external axes in degrees/s. </param>
        /// <param name="predefined"> A boolean that indicates if the speeddata is predefined by ABB. </param>
        private SpeedData(string name, double v_tcp, double v_ori, double v_leax, double v_reax, bool predefined)
        {
            _name = name;
            _v_tcp = v_tcp;
            _v_ori = v_ori;
            _v_leax = v_leax;
            _v_reax = v_reax;
            _predefined = predefined;
        }

        /// <summary>
        /// Duplicates a SpeedData object
        /// </summary>
        /// <returns> Returns a deep copy of the SpeedData object. </returns>
        public SpeedData Duplicate()
        {
            SpeedData dup = new SpeedData(Name, V_TCP, V_ORI, V_LEAX, V_REAX, PreDefinied);
            return dup;
        }
        #endregion

        #region method
        /// <summary>
        /// Used to create variable definitions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="robotInfo">Defines the RobotInfo for the action.</param>
        /// <param name="RAPIDcode">Defines the RAPID Code the variable entries are added to.</param>
        /// <returns>Return the RAPID variable code.</returns>
        public override string InitRAPIDVar(RAPIDGenerator RAPIDGenerator)
        {
            if (_predefined == false) 
            {
                return ("@" + "\t" + "VAR speeddata " + _name + ":= [" + _v_tcp + ", " + _v_ori + ", " + _v_leax + ", " + _v_reax + "];");
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Used to create action instructions in the RAPID Code. It is typically called inside the CreateRAPIDCode() method of the RAPIDGenerator class.
        /// </summary>
        /// <param name="robotToolName">Defines the robot rool name.</param>
        /// <returns>Returns the RAPID main code.</returns>
        public override string ToRAPIDFunction(string robotToolName)
        {
            return "";
        }
        #endregion

        #region properties
        /// <summary>
        /// A boolean that indicates if the SpeedData is valid. 
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (Name == "") { return false; }
                if (V_TCP <= 0) { return false; }
                if (V_ORI <= 0) { return false; }
                if (V_LEAX <= 0) { return false; }
                if (V_REAX <= 0) { return false; }
                return true;
            }
        }

        /// <summary>
        /// The SpeedData variable name. 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The velocity of the tool center point (TCP) in mm/s.
        /// If a stationary tool or coordinated external axes are used, the velocity is specified relative to the work object.
        /// </summary>
        public double V_TCP
        {
            get { return _v_tcp; }
            set { _v_tcp = value; }
        }

        /// <summary>
        /// The reorientation velocity of the TCP expressed in degrees/s. 
        /// If a stationary tool or coordinated external axes are used, the velocity is specified relative to the work object.
        /// </summary>
        public double V_ORI
        {
            get { return _v_ori; }
            set { _v_ori = value; }
        }

        /// <summary>
        /// The velocity of linear external axes in mm/s.
        /// </summary>
        public double V_LEAX
        {
            get { return _v_leax; }
            set { _v_leax = value; }
        }

        /// <summary>
        /// The velocity of rotating external axes in degrees/s.
        /// </summary>
        public double V_REAX
        {
            get { return _v_reax; }
            set { _v_reax = value; }
        }

        /// <summary>
        /// A boolean that indicates if the speeddata is predefined by ABB. 
        /// </summary>
        public bool PreDefinied
        {
            get { return _predefined; }
            set { _predefined = value; }
        }
        #endregion

    }
}
