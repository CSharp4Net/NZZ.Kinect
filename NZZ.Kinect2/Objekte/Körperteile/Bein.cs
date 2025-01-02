using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZZ.Kinect2.Objekte.Körperteile
{
    class Bein : Schnittstellen.Geste
    {
        public Bein(Typen.Seite seite)
        {
            Seite = seite;

            Hüfte = new Einzelteile.Hüfte(seite);

            Knie = new Einzelteile.Knie(seite);

            Fußknöchel = new Einzelteile.Fußknöchel(seite);

            Fuß = new Einzelteile.Fuß(seite);
        }

        public Typen.Seite Seite { get; private set; }

        public Einzelteile.Hüfte Hüfte { get; private set; }

        public Einzelteile.Knie Knie { get; private set; }

        public Einzelteile.Fußknöchel Fußknöchel { get; private set; }

        public Einzelteile.Fuß Fuß { get; private set; }
    }
}
