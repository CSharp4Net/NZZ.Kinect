using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using NuiVector = Microsoft.Research.Kinect.Nui.Vector;

namespace NZZ.Kinect
{
    internal delegate void MoveForOrBackEventHandler(bool isActive, bool isForward);
    internal delegate void MoveLeftOrRightEventHandler(bool isActive, bool isLeft);

    internal delegate void LookUpOrDownEventHandler(bool isUp);
    internal delegate void LookLeftOrRightEventHandler(bool isLeft);

    internal delegate void ConvertCoordsToDictionaryReady(Dictionary<JointID, NuiVector> jointDictionary);
}
