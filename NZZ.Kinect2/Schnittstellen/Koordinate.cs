using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSKinect = Microsoft.Research.Kinect.Nui;

namespace NZZ.Kinect2.Schnittstellen
{
    interface Koordinate
    {
        MSKinect.JointID PunktTyp { get;  }

        MSKinect.Joint Punkt { get; set; }
    }
}
