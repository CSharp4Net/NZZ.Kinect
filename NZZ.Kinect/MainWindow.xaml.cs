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
using Microsoft.Research.Kinect.Nui;
using System.Collections;
using System.Diagnostics;

namespace NZZ.Kinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors
        public MainWindow()
        {
            NZZ.Framework.StartUp.Initialize(_AddOnNamespace);

            InitializeComponent();

            InitializeMembers();
        }
        #endregion

        const string _AddOnNamespace = "NZZ.KINECT";

        Character Charakter1 { get; set; }

        #region Members
        Runtime myRuntime = null;

        bool readyToWork = false;
        #endregion

        #region Functions
        void InitializeMembers()
        {

        }
        void InitializeDevice()
        {
            readyToWork = true;

            AddEntryToLogBox("Loading ... ");
            myRuntime = new Runtime();
            AddEntryToLogBox("OK", false);

            try
            {
                AddEntryToLogBox("Initializing ... ");
                myRuntime.Initialize(
                    RuntimeOptions.UseDepthAndPlayerIndex |
                    RuntimeOptions.UseSkeletalTracking |
                    RuntimeOptions.UseColor);
                AddEntryToLogBox("OK", false);
            }
            catch (InvalidOperationException)
            {
                System.Windows.MessageBox.Show("Runtime initialization failed. Please make sure Kinect device is plugged in.");
                readyToWork = false;
            }

            try
            {
                AddEntryToLogBox("Get streams ... ");
                myRuntime.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
                myRuntime.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);

                AddEntryToLogBox("OK", false);
            }
            catch (InvalidOperationException)
            {
                System.Windows.MessageBox.Show("Failed to open stream. Please make sure to specify a supported image type and resolution.");
                readyToWork = false;
            }
        }
        void InitializeGUI()
        {
            if (readyToWork)
            {
                ButtonConnect.IsEnabled = true;
                ButtonDisconnect.IsEnabled = true;
            }
        }

        void AddEntryToLogBox(string message, bool newLine = true)
        {
            TextBoxLog.Text += (newLine ? Environment.NewLine : string.Empty) + message;
        }

        void ShowCoordinatesInGUI(JointsCollection jointsCollection)
        {
            // für jedes körperteil des skeletts
            foreach (Joint joint in jointsCollection)
            {
                bool elementFound = false;
                foreach (UIElement element in GridWithLeftJoints.Children)
                {
                    if (element is TextBoxWithJoint)
                    {
                        if ((element as TextBoxWithJoint).HoldedJoint == joint.ID)
                        {
                            (element as TextBoxWithJoint).DisplayJoint(joint);
                            elementFound = true;
                        }
                    }
                    if (elementFound) break;
                }
                if (!elementFound)
                    foreach (UIElement element in GridWithRightJoints.Children)
                    {
                        if (element is TextBoxWithJoint)
                        {
                            if ((element as TextBoxWithJoint).HoldedJoint == joint.ID)
                            {
                                (element as TextBoxWithJoint).DisplayJoint(joint);
                                elementFound = true;
                            }
                        }
                        if (elementFound) break;
                    }

                //switch (joint.ID)
                //{
                //    case JointID.Head:
                //        TbJ
                //        break;

                //    case JointID.HipRight:
                //        TbJointHipRight.DisplayJoint(joint);
                //        break;
                //    case JointID.ShoulderRight:
                //        TbJointShoulderRight.DisplayJoint(joint);
                //        break;
                //    case JointID.ElbowRight:
                //        TbJointElbowRight.DisplayJoint(joint);
                //        break;
                //    case JointID.WristRight:
                //        TbJointWristRight.DisplayJoint(joint);
                //        break;
                //    case JointID.HandRight:
                //        TbJointHandRight.DisplayJoint(joint);
                //        break;

                //    case JointID.HipLeft:
                //        TbJointHipLeft.DisplayJoint(joint);
                //        break;
                //    case JointID.ShoulderLeft:
                //        TbJointShoulderLeft.DisplayJoint(joint);
                //        break;
                //    case JointID.ElbowLeft:
                //        TbJointElbowLeft.DisplayJoint(joint);
                //        break;
                //    case JointID.WristLeft:
                //        TbJointWristLeft.DisplayJoint(joint);
                //        break;
                //    case JointID.HandLeft:
                //        TbJointHandLeft.DisplayJoint(joint);
                //        break;
                //}
            }
        }
        #endregion

        #region Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeDevice();

                InitializeGUI();

                Charakter1 = new Character();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                Charakter1.Dispose();

                myRuntime.Uninitialize();

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddEntryToLogBox("Registrate ... ");

                Charakter1.LowerPartOfBody.EventMoveForOrBack += new MoveForOrBackEventHandler(LowerPartOfBody_EventMoveForOrBack);
                Charakter1.LowerPartOfBody.EventMoveLeftOrRight += new MoveLeftOrRightEventHandler(LowerPartOfBody_EventMoveLeftOrRight);
                Charakter1.UpperPartOfBody.EventLookUpOrDown += new LookUpOrDownEventHandler(UpperPartOfBody_EventLookUpOrDown);
                Charakter1.UpperPartOfBody.EventLookLeftOrRight += new LookLeftOrRightEventHandler(UpperPartOfBody_EventLookLeftOrRight);

                GestureController.EventCoordToDictReady += new ConvertCoordsToDictionaryReady(GestureController_EventCoordToDictReady);
                
                myRuntime.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(MyRuntimeSays_SkeletonFrameReady);
                myRuntime.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(MyRuntimeSays_SkeletonFrameReady_Color);

                AddEntryToLogBox("OK", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddEntryToLogBox("Deregistrate ... ");

                myRuntime.SkeletonFrameReady -= new EventHandler<SkeletonFrameReadyEventArgs>(MyRuntimeSays_SkeletonFrameReady);
                myRuntime.SkeletonFrameReady -= new EventHandler<SkeletonFrameReadyEventArgs>(MyRuntimeSays_SkeletonFrameReady_Color);
                
                GestureController.EventCoordToDictReady += new ConvertCoordsToDictionaryReady(GestureController_EventCoordToDictReady);

                Charakter1.LowerPartOfBody.EventMoveForOrBack -= new MoveForOrBackEventHandler(LowerPartOfBody_EventMoveForOrBack);
                Charakter1.LowerPartOfBody.EventMoveLeftOrRight -= new MoveLeftOrRightEventHandler(LowerPartOfBody_EventMoveLeftOrRight);
                Charakter1.UpperPartOfBody.EventLookUpOrDown -= new LookUpOrDownEventHandler(UpperPartOfBody_EventLookUpOrDown);
                Charakter1.UpperPartOfBody.EventLookLeftOrRight -= new LookLeftOrRightEventHandler(UpperPartOfBody_EventLookLeftOrRight);

                AddEntryToLogBox("OK", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void UpperPartOfBody_EventLookLeftOrRight(bool isLeft)
        {
            if (isLeft)
            {
                TextBoxLookleft.Background = new SolidColorBrush(Colors.LightGreen);
                TextBoxLookRight.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                TextBoxLookleft.Background = new SolidColorBrush(Colors.White);
                TextBoxLookRight.Background = new SolidColorBrush(Colors.LightGreen);
            }
        }
        void UpperPartOfBody_EventLookUpOrDown(bool isUp)
        {
            if (isUp)
            {
                TextBoxLookUp.Background = new SolidColorBrush(Colors.LightGreen);
                TextBoxLookDown.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                TextBoxLookUp.Background = new SolidColorBrush(Colors.White);
                TextBoxLookDown.Background = new SolidColorBrush(Colors.LightGreen);
            }
        }

        void LowerPartOfBody_EventMoveLeftOrRight(bool isActive, bool isLeft)
        {
            if (isActive)
            {
                if (isLeft)
                {
                    TextBoxMoveLeft.Background = new SolidColorBrush(Colors.LightGreen);
                    TextBoxMoveRight.Background = new SolidColorBrush(Colors.White);
                }
                else
                {
                    TextBoxMoveLeft.Background = new SolidColorBrush(Colors.White);
                    TextBoxMoveRight.Background = new SolidColorBrush(Colors.LightGreen);

                }
            }
            else
            {
                TextBoxMoveLeft.Background = new SolidColorBrush(Colors.White);
                TextBoxMoveRight.Background = new SolidColorBrush(Colors.White);
            }
        }
        void LowerPartOfBody_EventMoveForOrBack(bool isActive, bool isForward)
        {
            if (isActive)
            {
                if (isForward)
                {
                    TextBoxMoveForward.Background = new SolidColorBrush(Colors.LightGreen);
                    TextBoxMoveBack.Background = new SolidColorBrush(Colors.White);
                }
                else
                {
                    TextBoxMoveForward.Background = new SolidColorBrush(Colors.White);
                    TextBoxMoveBack.Background = new SolidColorBrush(Colors.LightGreen);
                }
            }
            else
            {
                TextBoxMoveForward.Background = new SolidColorBrush(Colors.White);
                TextBoxMoveBack.Background = new SolidColorBrush(Colors.White);
            }
        }
        
        private void MyRuntimeSays_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            try
            {
                SkeletonFrame skeletonFrame = e.SkeletonFrame;

                foreach (SkeletonData skeleton in skeletonFrame.Skeletons)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        ShowCoordinatesInGUI(skeleton.Joints);

                        GestureController.CheckSkeletonAndDoWork(skeleton.Joints);

                        if (GestureController.HandleFound)
                            GestureController.CheckSkeletonAndDoWork(skeleton.Joints);
                        else
                            GestureController.ConnectToApply("TESV");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void GestureController_EventCoordToDictReady(Dictionary<JointID, Microsoft.Research.Kinect.Nui.Vector> jointDictionary)
        {
            Charakter1.CheckUp(jointDictionary);
        }
        #endregion

        #region SekeltonAnimation
        private void MyRuntimeSays_SkeletonFrameReady_Color(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skeletonFrame = e.SkeletonFrame;
            int iSkeleton = 0;
            var brushes = new Brush[6];
            brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
            brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
            brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
            brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

            skeletonCanvas.Children.Clear();
            foreach (SkeletonData data in skeletonFrame.Skeletons)
            {
                if (SkeletonTrackingState.Tracked == data.TrackingState)
                {
                    // Draw bones
                    Brush brush = brushes[iSkeleton % brushes.Length];
                    skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointID.HipCenter, JointID.Spine, JointID.ShoulderCenter, JointID.Head));
                    skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ElbowLeft, JointID.WristLeft, JointID.HandLeft));
                    skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderRight, JointID.ElbowRight, JointID.WristRight, JointID.HandRight));
                    skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipLeft, JointID.KneeLeft, JointID.AnkleLeft, JointID.FootLeft));
                    skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipRight, JointID.KneeRight, JointID.AnkleRight, JointID.FootRight));

                    // Draw joints
                    foreach (Joint joint in data.Joints)
                    {
                        Point jointPos = GetDisplayPosition(joint);
                        var jointLine = new Line();
                        jointLine.X1 = jointPos.X - 3;
                        jointLine.X2 = jointLine.X1 + 6;
                        jointLine.Y1 = jointLine.Y2 = jointPos.Y;
                        jointLine.Stroke = _jointColors[joint.ID];
                        jointLine.StrokeThickness = 6;
                        skeletonCanvas.Children.Add(jointLine);
                    }
                }

                iSkeleton++;
            } // for each skeleton
        }

        private Point GetDisplayPosition(Joint joint)
        {
            float depthX, depthY;
            myRuntime.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
            depthX = Math.Max(0, Math.Min(depthX * 320, 320)); // convert to 320, 240 space
            depthY = Math.Max(0, Math.Min(depthY * 240, 240)); // convert to 320, 240 space
            int colorX, colorY;
            var iv = new ImageViewArea();

            // Only ImageResolution.Resolution640x480 is supported at this point
            myRuntime.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, iv, (int)depthX, (int)depthY, 0, out colorX, out colorY);

            // Map back to skeleton.Width & skeleton.Height
            return new Point((int)(skeletonCanvas.Width * colorX / 640.0), (int)(skeletonCanvas.Height * colorY / 480));
        }

        private Polyline GetBodySegment(JointsCollection joints, Brush brush, params JointID[] ids)
        {
            var points = new PointCollection(ids.Length);
            foreach (JointID t in ids)
            {
                points.Add(GetDisplayPosition(joints[t]));
            }

            var polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = brush;
            polyline.StrokeThickness = 5;
            return polyline;
        }

        private readonly Dictionary<JointID, Brush> _jointColors = new Dictionary<JointID, Brush>
        { 
            {JointID.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointID.Head, new SolidColorBrush(Color.FromRgb(200, 0, 0))},
            {JointID.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79, 84, 33))},
            {JointID.ElbowLeft, new SolidColorBrush(Color.FromRgb(84, 33, 42))},
            {JointID.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HandLeft, new SolidColorBrush(Color.FromRgb(215, 86, 0))},
            {JointID.ShoulderRight, new SolidColorBrush(Color.FromRgb(33, 79,  84))},
            {JointID.ElbowRight, new SolidColorBrush(Color.FromRgb(33, 33, 84))},
            {JointID.WristRight, new SolidColorBrush(Color.FromRgb(77, 109, 243))},
            {JointID.HandRight, new SolidColorBrush(Color.FromRgb(37,  69, 243))},
            {JointID.HipLeft, new SolidColorBrush(Color.FromRgb(77, 109, 243))},
            {JointID.KneeLeft, new SolidColorBrush(Color.FromRgb(69, 33, 84))},
            {JointID.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointID.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointID.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222, 76))},
            {JointID.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointID.FootRight, new SolidColorBrush(Color.FromRgb(77, 109, 243))}
        };
        #endregion
    }
}
