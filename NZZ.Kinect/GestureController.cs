using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using NuiVector = Microsoft.Research.Kinect.Nui.Vector;
using System.Windows;
using System.Diagnostics;
using System.Reflection;

namespace NZZ.Kinect
{
    internal static class GestureController
    {
        internal static event ConvertCoordsToDictionaryReady EventCoordToDictReady;

        internal static bool HandleFound { get; private set; }

        internal static void CheckSkeletonAndDoWork(JointsCollection jointsCollection)
        {
            Dictionary<JointID, NuiVector> jointDictionary = ConvertJointCollection(jointsCollection);

            EventCoordToDictReady(jointDictionary);
        }

        static Dictionary<JointID, NuiVector> ConvertJointCollection(JointsCollection collection)
        {
            Dictionary<JointID, NuiVector> dict = new Dictionary<JointID, NuiVector>();

            foreach (Joint joint in collection)
            {
                dict.Add(joint.ID, joint.Position);
            }

            return dict;
        }

        internal static bool ConnectToApply(string windowName)
        {
            try
            {
                foreach (Process p in Process.GetProcesses("."))
                {

                    if (p.MainWindowTitle.Length > 0)
                    {
                        if (p.ProcessName == windowName)
                        {
                            Character.HandleOfGame = p.MainWindowHandle;

                            HandleFound = true;

                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                NZZ.Framework.Logger.Writer.WriteError(MethodBase.GetCurrentMethod().Name, ex.ToString());
                return false;
            }
        }
    }
}
