using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSKinect = Microsoft.Research.Kinect.Nui;

namespace NZZ.Kinect2.Objekte.Körperteile.Einzelteile
{
    class Handgelenk : Schnittstellen.Koordinate
    {
        public Handgelenk(Typen.Seite seite)
        {
            switch (seite)
            {
                case Typen.Seite.Links:
                    PunktTyp = MSKinect.JointID.WristLeft;
                    break;
                case Typen.Seite.Rechts:
                    PunktTyp = MSKinect.JointID.WristRight;
                    break;
            }
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
