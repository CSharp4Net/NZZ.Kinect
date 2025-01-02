using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSKinect = Microsoft.Research.Kinect.Nui;

namespace NZZ.Kinect2.Objekte
{
    public class Spieler
    {
        public Spieler(int spielerNummer)
        {
            SpielerNummer = spielerNummer;

            Torso = new Körperteile.Torso();

            LinkerArm = new Körperteile.Arm(Typen.Seite.Links);

            RechterArm = new Körperteile.Arm(Typen.Seite.Rechts);

            LinkesBein = new Körperteile.Bein(Typen.Seite.Links);

            RechtesBein = new Körperteile.Bein(Typen.Seite.Rechts);
        }

        public int SpielerNummer { get; private set; }

        Körperteile.Torso Torso { get; set; }

        Körperteile.Arm LinkerArm { get; set; }

        Körperteile.Arm RechterArm { get; set; }

        Körperteile.Bein LinkesBein { get; set; }

        Körperteile.Bein RechtesBein { get; set; }

        public void ÜbernehmeKörperpunkte(MSKinect.SkeletonData skeletonData)
        {
            Torso.Kopf.Punkt = skeletonData.Joints[Torso.Kopf.PunktTyp];
            Torso.Hals.Punkt = skeletonData.Joints[Torso.Hals.PunktTyp];
            Torso.Bauch.Punkt = skeletonData.Joints[Torso.Bauch.PunktTyp];

            LinkerArm.Schulter.Punkt = skeletonData.Joints[LinkerArm.Schulter.PunktTyp];
            LinkerArm.Ellenbogen.Punkt = skeletonData.Joints[LinkerArm.Ellenbogen.PunktTyp];
            LinkerArm.Handgelenk.Punkt = skeletonData.Joints[LinkerArm.Handgelenk.PunktTyp];
            LinkerArm.Hand.Punkt = skeletonData.Joints[LinkerArm.Hand.PunktTyp];

            RechterArm.Schulter.Punkt = skeletonData.Joints[RechterArm.Schulter.PunktTyp];
            RechterArm.Ellenbogen.Punkt = skeletonData.Joints[RechterArm.Ellenbogen.PunktTyp];
            RechterArm.Handgelenk.Punkt = skeletonData.Joints[RechterArm.Handgelenk.PunktTyp];
            RechterArm.Hand.Punkt = skeletonData.Joints[RechterArm.Hand.PunktTyp];

            LinkesBein.Hüfte.Punkt = skeletonData.Joints[LinkesBein.Hüfte.PunktTyp];
            LinkesBein.Knie.Punkt = skeletonData.Joints[LinkesBein.Knie.PunktTyp];
            LinkesBein.Fußknöchel.Punkt = skeletonData.Joints[LinkesBein.Fußknöchel.PunktTyp];
            LinkesBein.Fuß.Punkt = skeletonData.Joints[LinkesBein.Fuß.PunktTyp];

            RechtesBein.Hüfte.Punkt = skeletonData.Joints[RechtesBein.Hüfte.PunktTyp];
            RechtesBein.Knie.Punkt = skeletonData.Joints[RechtesBein.Knie.PunktTyp];
            RechtesBein.Fußknöchel.Punkt = skeletonData.Joints[RechtesBein.Fußknöchel.PunktTyp];
            RechtesBein.Fuß.Punkt = skeletonData.Joints[RechtesBein.Fuß.PunktTyp];
        }
    }
}
