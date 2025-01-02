using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSKinect = Microsoft.Research.Kinect.Nui;

namespace NZZ.Kinect2.Objekte.Körperteile.Einzelteile
{
    class Fuß : Schnittstellen.Koordinate
    {
        public Fuß(Typen.Seite seite)
        {
            switch (seite)
            {
                case Typen.Seite.Links:
                    PunktTyp = MSKinect.JointID.FootLeft;
                    break;
                case Typen.Seite.Rechts:
                    PunktTyp = MSKinect.JointID.FootRight;
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
