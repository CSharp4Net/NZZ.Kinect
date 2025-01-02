using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSKinect = Microsoft.Research.Kinect.Nui;
using System.Reflection;

namespace NZZ.Kinect2.Objekte
{
    public class Kinect
    {
        public MSKinect.Runtime Runtime = null;

        public void Initialize()
        {
            Runtime = new MSKinect.Runtime();
            Runtime.Initialize(MSKinect.RuntimeOptions.UseDepthAndPlayerIndex | MSKinect.RuntimeOptions.UseDepthAndPlayerIndex | MSKinect.RuntimeOptions.UseColor);

            Runtime.VideoStream.Open(MSKinect.ImageStreamType.Video, 2, MSKinect.ImageResolution.Resolution640x480, MSKinect.ImageType.Color);
            Runtime.DepthStream.Open(MSKinect.ImageStreamType.Depth, 2, MSKinect.ImageResolution.Resolution320x240, MSKinect.ImageType.DepthAndPlayerIndex);
        }

        public void Deinitialize()
        {
            if (Runtime != null)
            {
                Runtime.Uninitialize();
                Runtime = null;
            }
        }
    }
}
