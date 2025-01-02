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

namespace NZZ.Kinect
{
    /// <summary>
    /// Interaction logic for TextBoxWithJoint.xaml
    /// </summary>
    public partial class TextBoxWithJoint : UserControl
    {
        public TextBoxWithJoint()
        {
            InitializeComponent();
        }

        JointID _holdedJoint = JointID.Spine;
        public JointID HoldedJoint
        {
            get { return _holdedJoint; }
            set
            {
                LabelJointName.Content = value.ToString();
                _holdedJoint = value;
            }
        }

        public double TargetX { get; set; }

        public void DisplayJoint(Joint joint)
        {
            TextBoxJointX.Text = Math.Round(joint.Position.X, 2).ToString();
            TextBoxJointY.Text = Math.Round(joint.Position.Y, 2).ToString();
            TextBoxJointZ.Text = Math.Round(joint.Position.Z, 2).ToString();
            TextBoxJointW.Text = joint.Position.W.ToString();
        }
    }
}
