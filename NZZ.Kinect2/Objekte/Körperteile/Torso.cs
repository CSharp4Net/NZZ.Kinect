using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZZ.Kinect2.Objekte.Körperteile
{
    class Torso : Schnittstellen.Geste
    {
        public Torso()
        {
            Kopf = new Einzelteile.Kopf();

            Hals = new Einzelteile.Hals();

            Bauch = new Einzelteile.Bauch();
        }

        public Einzelteile.Kopf Kopf { get; set; }

        public Einzelteile.Hals Hals { get; set; }

        public Einzelteile.Bauch Bauch { get; set; }
    }
}
