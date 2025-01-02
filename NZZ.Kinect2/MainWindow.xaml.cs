using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MSKinect = Microsoft.Research.Kinect.Nui;
using System.Reflection;

namespace NZZ.Kinect2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SetzePunktFarben();
        }

        Objekte.Kinect Kinect { get; set; }

        Objekte.Spieler Spieler1 { get; set; }

        Objekte.Spieler Spieler2 { get; set; }

        Dictionary<MSKinect.JointID, Brush> PunktFarben { get; set; }

        private Polyline GetBodySegment(MSKinect.JointsCollection joints, Brush brush, params MSKinect.JointID[] ids)
        {
            var points = new PointCollection(ids.Length);
            foreach (MSKinect.JointID t in ids)
            {
                points.Add(GetDisplayPosition(joints[t]));
            }

            var polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = brush;
            polyline.StrokeThickness = 5;
            return polyline;
        }

        private Point GetDisplayPosition(MSKinect.Joint joint)
        {
            float depthX, depthY;
            Kinect.Runtime.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
            depthX = Math.Max(0, Math.Min(depthX * 320, 320)); // convert to 320, 240 space
            depthY = Math.Max(0, Math.Min(depthY * 240, 240)); // convert to 320, 240 space
            int colorX, colorY;
            var iv = new MSKinect.ImageViewArea();

            // Only ImageResolution.Resolution640x480 is supported at this point
            Kinect.Runtime.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(MSKinect.ImageResolution.Resolution640x480, iv, (int)depthX, (int)depthY, 0, out colorX, out colorY);

            // Map back to skeleton.Width & skeleton.Height
            return new Point((int)(CanvasSpieler1.Width * colorX / 640.0), (int)(CanvasSpieler1.Height * colorY / 480));
        }

        private void SetzePunktFarben()
        { 
            PunktFarben = new Dictionary<MSKinect.JointID,Brush>();
            PunktFarben.Add(MSKinect.JointID.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155)));
            PunktFarben.Add(MSKinect.JointID.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155)));
            PunktFarben.Add(MSKinect.JointID.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29)));
            PunktFarben.Add(MSKinect.JointID.Head, new SolidColorBrush(Color.FromRgb(200, 0, 0)));
            PunktFarben.Add(MSKinect.JointID.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79, 84, 33)));
            PunktFarben.Add(MSKinect.JointID.ElbowLeft, new SolidColorBrush(Color.FromRgb(84, 33, 42)));
            PunktFarben.Add(MSKinect.JointID.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0)));
            PunktFarben.Add(MSKinect.JointID.HandLeft, new SolidColorBrush(Color.FromRgb(215, 86, 0)));
            PunktFarben.Add(MSKinect.JointID.ShoulderRight, new SolidColorBrush(Color.FromRgb(33, 79,  84)));
            PunktFarben.Add(MSKinect.JointID.ElbowRight, new SolidColorBrush(Color.FromRgb(33, 33, 84)));
            PunktFarben.Add(MSKinect.JointID.WristRight, new SolidColorBrush(Color.FromRgb(77, 109, 243)));
            PunktFarben.Add(MSKinect.JointID.HandRight, new SolidColorBrush(Color.FromRgb(37,  69, 243)));
            PunktFarben.Add(MSKinect.JointID.HipLeft, new SolidColorBrush(Color.FromRgb(77, 109, 243)));
            PunktFarben.Add(MSKinect.JointID.KneeLeft, new SolidColorBrush(Color.FromRgb(69, 33, 84)));
            PunktFarben.Add(MSKinect.JointID.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122)));
            PunktFarben.Add(MSKinect.JointID.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0)));
            PunktFarben.Add(MSKinect.JointID.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213)));
            PunktFarben.Add(MSKinect.JointID.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222, 76)));
            PunktFarben.Add(MSKinect.JointID.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156)));
            PunktFarben.Add(MSKinect.JointID.FootRight, new SolidColorBrush(Color.FromRgb(77, 109, 243)));
        }

        private void ZeichneSpieler(MSKinect.SkeletonData skeletonData)
        {
            CanvasSpieler1.Children.Clear();

            // Draw bones
            Brush brush = Brushes.White;
            CanvasSpieler1.Children.Add(GetBodySegment(skeletonData.Joints, brush, MSKinect.JointID.HipCenter, MSKinect.JointID.Spine, MSKinect.JointID.ShoulderCenter, MSKinect.JointID.Head));
            CanvasSpieler1.Children.Add(GetBodySegment(skeletonData.Joints, brush, MSKinect.JointID.ShoulderCenter, MSKinect.JointID.ShoulderLeft, MSKinect.JointID.ElbowLeft, MSKinect.JointID.WristLeft, MSKinect.JointID.HandLeft));
            CanvasSpieler1.Children.Add(GetBodySegment(skeletonData.Joints, brush, MSKinect.JointID.ShoulderCenter, MSKinect.JointID.ShoulderRight, MSKinect.JointID.ElbowRight, MSKinect.JointID.WristRight, MSKinect.JointID.HandRight));
            CanvasSpieler1.Children.Add(GetBodySegment(skeletonData.Joints, brush, MSKinect.JointID.HipCenter, MSKinect.JointID.HipLeft, MSKinect.JointID.KneeLeft, MSKinect.JointID.AnkleLeft, MSKinect.JointID.FootLeft));
            CanvasSpieler1.Children.Add(GetBodySegment(skeletonData.Joints, brush, MSKinect.JointID.HipCenter, MSKinect.JointID.HipRight, MSKinect.JointID.KneeRight, MSKinect.JointID.AnkleRight, MSKinect.JointID.FootRight));

            // Draw joints
            foreach (MSKinect.Joint joint in skeletonData.Joints)
            {
                Point jointPos = GetDisplayPosition(joint);
                var jointLine = new Line();
                jointLine.X1 = jointPos.X - 3;
                jointLine.X2 = jointLine.X1 + 6;
                jointLine.Y1 = jointLine.Y2 = jointPos.Y;
                jointLine.Stroke = PunktFarben[joint.ID];
                jointLine.StrokeThickness = 6;
                CanvasSpieler1.Children.Add(jointLine);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Kinect = new Objekte.Kinect();
            Kinect.Initialize();
            Kinect.Runtime.SkeletonFrameReady += new EventHandler<MSKinect.SkeletonFrameReadyEventArgs>(Runtime_SkeletonFrameReady);
            Kinect.Runtime.VideoFrameReady += new EventHandler<MSKinect.ImageFrameReadyEventArgs>(Runtime_VideoFrameReady);
            Kinect.Runtime.DepthFrameReady += new EventHandler<MSKinect.ImageFrameReadyEventArgs>(Runtime_DepthFrameReady);

            Spieler1 = new Objekte.Spieler(0);
            Spieler2 = new Objekte.Spieler(1);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            Kinect.Runtime.SkeletonFrameReady -= new EventHandler<MSKinect.SkeletonFrameReadyEventArgs>(Runtime_SkeletonFrameReady);
            Kinect.Runtime.VideoFrameReady -= new EventHandler<MSKinect.ImageFrameReadyEventArgs>(Runtime_VideoFrameReady);
            Kinect.Runtime.DepthFrameReady -= new EventHandler<MSKinect.ImageFrameReadyEventArgs>(Runtime_DepthFrameReady);
            Kinect.Deinitialize();
            Kinect = null;

            Spieler1 = null;
            Spieler2 = null;
        }

        void Runtime_DepthFrameReady(object sender, MSKinect.ImageFrameReadyEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Statisch.ErrorHandler.Handle(MethodBase.GetCurrentMethod(), ex);
            }
        }

        void Runtime_VideoFrameReady(object sender, MSKinect.ImageFrameReadyEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Statisch.ErrorHandler.Handle(MethodBase.GetCurrentMethod(), ex);
            }
        }

        void Runtime_SkeletonFrameReady(object sender, MSKinect.SkeletonFrameReadyEventArgs e)
        {
            try
            {
                MSKinect.SkeletonFrame skeletonFrame = e.SkeletonFrame;

                foreach (MSKinect.SkeletonData skeletonData in skeletonFrame.Skeletons)
                {
                    if (skeletonData.TrackingState == MSKinect.SkeletonTrackingState.Tracked)
                    {
                        LblSpieler1.Foreground = Brushes.Black;
                        Spieler1.ÜbernehmeKörperpunkte(skeletonData);

                        ZeichneSpieler(skeletonData);
                    }
                    else
                    {
                        CanvasSpieler1.Children.Clear();
                        LblSpieler1.Foreground = Brushes.Red;
                    }                    
                }
            }
            catch (Exception ex)
            {
                Statisch.ErrorHandler.Handle(MethodBase.GetCurrentMethod(), ex);
            }
        }
    }
}
