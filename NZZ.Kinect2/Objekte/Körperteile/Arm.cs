using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZZ.Kinect2.Objekte.Körperteile
{
    class Arm : Schnittstellen.Geste
    {
        public Arm(Typen.Seite seite)
        {
            Seite = seite;

            Schulter = new Einzelteile.Schulter(seite);

            Ellenbogen = new Einzelteile.Ellenbogen(seite);

            Handgelenk = new Einzelteile.Handgelenk(seite);

            Hand = new Einzelteile.Hand(seite);
        }

        public Typen.Seite Seite { get; private set; }

        public Einzelteile.Schulter Schulter { get; private set; }

        public Einzelteile.Ellenbogen Ellenbogen { get; private set; }

        public Einzelteile.Handgelenk Handgelenk { get; private set; }

        public Einzelteile.Hand Hand { get; private set; }
    }
}
