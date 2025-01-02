using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSKinect = Microsoft.Research.Kinect.Nui;

namespace NZZ.Kinect2.Objekte.Körperteile.Einzelteile
{
    class Tailie
    {
        public Tailie()
        {
            PunktTyp = MSKinect.JointID.HipCenter;
        }

        public MSKinect.JointID PunktTyp
        {
            get;
            private set;
        }

        public MSKinect.Joint Punkt
        {
            get;
            set;
        }
    }
}
