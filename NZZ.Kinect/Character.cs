using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using NuiVector = Microsoft.Research.Kinect.Nui.Vector;

namespace NZZ.Kinect
{
    internal class Character : IDisposable
    {
        internal Character()
        {
            LowerPartOfBody = new LowerBody(this);
            UpperPartOfBody = new UpperBody(this);
        }

        #region Members
        internal int ExactlyLevel = 1;
        internal static IntPtr HandleOfGame = IntPtr.Zero;

        internal float _minDistanceLeftToRightBig = 0;
        internal float _minDistanceLeftToRightSmall = 0;
        internal float _minDistanceUpToDown = 0;
        #endregion

        #region Properties
        internal LowerBody LowerPartOfBody { get; private set; }
        internal UpperBody UpperPartOfBody { get; private set; }

        Dictionary<JointID, NuiVector> JointDictionary { get; set; }
        #endregion

        #region Functions
        public void Dispose()
        {
            LowerPartOfBody.Dispose();
            UpperPartOfBody.Dispose();
        }

        internal void CheckUp(Dictionary<JointID, NuiVector> jointDictionary)
        {
            JointDictionary = jointDictionary;

            _minDistanceLeftToRightBig = JointDictionary[JointID.ShoulderLeft].X - JointDictionary[JointID.ShoulderRight].X;
            _minDistanceLeftToRightSmall = JointDictionary[JointID.ShoulderCenter].X - JointDictionary[JointID.ShoulderRight].X;
            _minDistanceUpToDown = JointDictionary[JointID.Head].Y - JointDictionary[JointID.ShoulderCenter].Y;

            if (_minDistanceLeftToRightBig < 0) _minDistanceLeftToRightBig *= -1;
            if (_minDistanceLeftToRightSmall < 0) _minDistanceLeftToRightSmall *= -1;
            if (_minDistanceUpToDown < 0) _minDistanceUpToDown *= -1;

            LowerPartOfBody.CheckUp();
            UpperPartOfBody.CheckUp();
        }

        internal bool IsOver(JointID jointOne, JointID jointTwo)
        {
            float distance = JointDictionary[jointOne].Y - JointDictionary[jointTwo].Y;

            if (distance < 0) distance *= -1;

            return (JointDictionary[jointOne].Y > JointDictionary[jointTwo].Y) && (distance > _minDistanceUpToDown);
            //return (Math.Round(JointDictionary[jointOne].Y, ExactlyLevel) > Math.Round(JointDictionary[jointTwo].Y, ExactlyLevel)) && (distance > _minDistanceUpToDown);
        }
        internal bool IsUnder(JointID jointOne, JointID jointTwo)
        {
            float distance = JointDictionary[jointOne].Y - JointDictionary[jointTwo].Y;

            if (distance < 0) distance *= -1;

            return (JointDictionary[jointOne].Y < JointDictionary[jointTwo].Y) && (distance > _minDistanceUpToDown);
            //return (Math.Round(JointDictionary[jointOne].Y, ExactlyLevel) < Math.Round(JointDictionary[jointTwo].Y, ExactlyLevel)) && (distance > _minDistanceUpToDown);
        }

        internal bool IsBefore(JointID jointOne, JointID jointTwo)
        {
            float distance = JointDictionary[jointOne].Z - JointDictionary[jointTwo].Z;

            if (distance < 0) distance *= -1;

            return (JointDictionary[jointOne].Z < JointDictionary[jointTwo].Z && (distance > _minDistanceLeftToRightSmall));
            //return (Math.Round(JointDictionary[jointOne].Z, ExactlyLevel) < Math.Round(JointDictionary[jointTwo].Z, ExactlyLevel) && (distance > _minDistanceLeftToRightBig));
        }
        internal bool IsAfter(JointID jointOne, JointID jointTwo)
        {
            float distance = JointDictionary[jointOne].Z - JointDictionary[jointTwo].Z;

            if (distance < 0) distance *= -1;

            return (JointDictionary[jointOne].Z > JointDictionary[jointTwo].Z && (distance > _minDistanceLeftToRightSmall));
            //return (Math.Round(JointDictionary[jointOne].Z, ExactlyLevel) > Math.Round(JointDictionary[jointTwo].Z, ExactlyLevel) && (distance > _minDistanceLeftToRightSmall));
        }

        internal bool IsRight(JointID jointOne, JointID jointTwo)
        {
            float distance = JointDictionary[jointOne].X - JointDictionary[jointTwo].X;

            if (distance < 0) distance *= -1;

            return (JointDictionary[jointOne].X > JointDictionary[jointTwo].X && (distance > _minDistanceLeftToRightBig));
            //return (Math.Round(JointDictionary[jointOne].X, ExactlyLevel) > Math.Round(JointDictionary[jointTwo].X, ExactlyLevel) && (distance > _minDistanceLeftToRightSmall));
        }
        internal bool IsLeft(JointID jointOne, JointID jointTwo)
        {
            float distance = JointDictionary[jointOne].X - JointDictionary[jointTwo].X;

            if (distance < 0) distance *= -1;

            return (JointDictionary[jointOne].X < JointDictionary[jointTwo].X && (distance > _minDistanceLeftToRightBig));
            //return (Math.Round(JointDictionary[jointOne].X, ExactlyLevel) < Math.Round(JointDictionary[jointTwo].X, ExactlyLevel) && (distance > _minDistanceLeftToRightSmall));
        }
        #endregion

        internal class LowerBody : IDisposable
        {
            internal LowerBody(Character parent)
            {
                _myParent = parent;
            }

            #region Members
            Character _myParent = null;

            bool _moveForward = false;
            bool _moveBack = false;
            bool _moveLeft = false;
            bool _moveRight = false;

            bool _doDuck = false;
            #endregion

            #region Events
            internal event MoveForOrBackEventHandler EventMoveForOrBack;
            internal event MoveLeftOrRightEventHandler EventMoveLeftOrRight;
            #endregion

            #region Properties
            internal bool MoveForward
            {
                get
                {
                    return _moveForward;
                }
                set
                {
                    if (_moveForward != value)
                    {
                        DoMove_Forward(value);
                        FireEvent_MoveForward(value);
                        _moveForward = value;
                    }
                }
            }
            internal bool MoveBack
            {
                get
                {
                    return _moveBack;
                }
                set
                {
                    if (_moveBack != value)
                    {
                        DoMove_Back(value);
                        FireEvent_MoveBack(value);
                        _moveBack = value;
                    }
                }
            }
            internal bool MoveLeft
            {
                get
                {
                    return _moveLeft;
                }
                set
                {
                    if (_moveLeft != value)
                    {
                        DoMove_Left(value);
                        FireEvent_MoveLeft(value);
                        _moveLeft = value;
                    }
                }
            }
            internal bool MoveRight
            {
                get
                {
                    return _moveRight;
                }
                set
                {
                    if (_moveRight != value)
                    {
                        DoMove_Right(value);
                        FireEvent_MoveRight(value);
                        _moveRight = value;
                    }
                }
            }

            internal bool DoDuck
            {
                get
                {
                    return _doDuck;
                }
                set
                {
                    if (_doDuck != value) Communicator.PressKey(HandleOfGame, 's', value);
                    _doDuck = value;
                }
            }
            #endregion

            #region Functions
            public void Dispose()
            {

            }

            internal void CheckUp()
            {
                CheckFor_MoveForward();
                CheckFor_MoveBack();
                CheckFor_MoveLeft();
                CheckFor_MoveRight();
            }

            void CheckFor_MoveForward()
            {
                if (_myParent.IsBefore(JointID.FootRight, JointID.FootLeft) &&
                    _myParent.IsBefore(JointID.FootRight, JointID.HipCenter))
                {
                    MoveBack = false;
                    MoveForward = true;
                }
                else if (_myParent.IsBefore(JointID.FootLeft, JointID.FootRight) &&
                        _myParent.IsBefore(JointID.FootLeft, JointID.HipCenter))
                {
                    MoveBack = false;
                    MoveForward = true;
                }
                else
                {
                    MoveForward = false;
                }
            }
            void CheckFor_MoveBack()
            {
                if (_myParent.IsAfter(JointID.FootLeft, JointID.FootRight) &&
                    _myParent.IsAfter(JointID.FootLeft, JointID.HipCenter))
                {
                    MoveForward = false;
                    MoveBack = true;
                }
                else if (_myParent.IsAfter(JointID.FootRight, JointID.FootLeft) &&
                        _myParent.IsAfter(JointID.FootRight, JointID.HipCenter))
                {
                    MoveForward = false;
                    MoveBack = true;
                }
                else
                {
                    MoveBack = false;
                }
            }
            void CheckFor_MoveLeft()
            {
                if (_myParent.IsRight(JointID.FootRight, JointID.HipRight) &&
                    !_myParent.IsRight(JointID.FootLeft, JointID.HipLeft) &&
                    !_myParent.IsLeft(JointID.FootLeft, JointID.HipLeft))
                {
                    MoveRight = false;
                    MoveLeft = true;
                }
                else
                {
                    MoveLeft = false;
                }
            }
            void CheckFor_MoveRight()
            {
                if (_myParent.IsLeft(JointID.FootLeft, JointID.HipLeft) &&
                    !_myParent.IsLeft(JointID.FootRight, JointID.HipRight) &&
                    !_myParent.IsRight(JointID.FootRight, JointID.HipRight))
                {
                    MoveLeft = false;
                    MoveRight = true;
                }
                else
                {
                    MoveRight = false;
                }
            }

            void DoMove_Forward(bool value)
            {
                if (HandleOfGame != IntPtr.Zero)
                    Communicator.PressKey(HandleOfGame, 'w', value);
            }
            void DoMove_Back(bool value)
            {
                if (HandleOfGame != IntPtr.Zero)
                    Communicator.PressKey(HandleOfGame, 's', value);
            }
            void DoMove_Left(bool value)
            {
                if (HandleOfGame != IntPtr.Zero)
                    Communicator.PressKey(HandleOfGame, 'a', value);
            }
            void DoMove_Right(bool value)
            {
                if (HandleOfGame != IntPtr.Zero)
                    Communicator.PressKey(HandleOfGame, 'd', value);
            }

            void FireEvent_MoveForward(bool value)
            {
                if (EventMoveForOrBack != null)
                {
                    EventMoveForOrBack(value, true);
                }
                else
                {
                    Console.Write(string.Format("EventMoveForOrBack IsForward:{0}", true));
                }
            }
            void FireEvent_MoveBack(bool value)
            {
                if (EventMoveForOrBack != null)
                {
                    EventMoveForOrBack(value, false);
                }
                else
                {
                    Console.Write(string.Format("EventMoveForOrBack IsForward:{0}", false));
                }
            }
            void FireEvent_MoveLeft(bool value)
            {
                if (EventMoveLeftOrRight != null)
                {
                    EventMoveLeftOrRight(value, true);
                }
                else
                {
                    Console.Write(string.Format("EventMoveLeftOrRight IsLeft:{0}", true));
                }
            }
            void FireEvent_MoveRight(bool value)
            {
                if (EventMoveLeftOrRight != null)
                {
                    EventMoveLeftOrRight(value, false);
                }
                else
                {
                    Console.Write(string.Format("EventMoveLeftOrRight IsLeft:{0}", false));
                }
            }
            #endregion
        }

        internal class UpperBody : IDisposable
        {
            internal UpperBody(Character parent)
            {
                _myParent = parent;
            }

            #region Events
            internal event LookUpOrDownEventHandler EventLookUpOrDown;
            internal event LookLeftOrRightEventHandler EventLookLeftOrRight;
            #endregion

            #region Members
            Character _myParent = null;

            bool _lookUp = false;
            bool _lookDown = false;
            bool _lookLeft = false;
            bool _lookRight = false;

            bool _holdWeapons = false;
            #endregion

            #region Properties
            internal bool LookUp
            {
                get
                {
                    return _lookUp;
                }
                set
                {
                    if (value)
                    {
                        DoLook_Up();
                        FireEvent_LookUp();     
                    }
                    _lookUp = value;
                }
            }
            internal bool LookDown
            {
                get
                {
                    return _lookDown;
                }
                set
                {
                    if (true)
                    {
                        DoLook_Down();
                        FireEvent_LookBack();  
                    }
                    _lookDown = value;
                }
            }
            internal bool LookLeft
            {
                get
                {
                    return _lookLeft;
                }
                set
                {
                    if (_lookLeft != value)
                    {
                        DoLook_Left();
                        FireEvent_DoLeft();
                        _lookLeft = value;
                    }
                }
            }
            internal bool LookRight
            {
                get
                {
                    return _lookRight;
                }
                set
                {
                    if (_lookRight != value)
                    {
                        DoLook_Right();
                        FireEvent_DoRight();
                        _lookRight = value;
                    }
                }
            }

            internal bool HoldWeapons
            {
                get
                {
                    return _holdWeapons;
                }
                set
                {
                    if (_holdWeapons != value) Communicator.PressKey(HandleOfGame, 'r', value);
                    _holdWeapons = value;
                }
            }
            #endregion

            #region Functions
            public void Dispose()
            {

            }

            internal void CheckUp()
            {
                CheckFor_LookUp();
                CheckFor_LookDown();
                CheckFor_LookLeft();
                CheckFor_LookRight();
            }

            void CheckFor_LookUp()
            {
                if (_myParent.IsAfter(JointID.Head, JointID.HipCenter))
                {
                    LookDown = false;
                    LookUp = true;
                }
                else
                {
                    LookUp = false;
                }
            }
            void CheckFor_LookDown()
            {
                if (_myParent.IsBefore(JointID.Head, JointID.HipCenter))
                {
                    LookUp = false;
                    LookDown = true;
                }
                else
                {
                    LookDown = false;
                }
            }
            void CheckFor_LookLeft()
            {
                if (_myParent.IsLeft(JointID.ShoulderLeft, JointID.HipLeft))
                {
                    LookRight = false;
                    LookLeft = true;
                }
                else
                {
                    LookLeft = false;
                }
            }
            void CheckFor_LookRight()
            {
                if (_myParent.IsLeft(JointID.ShoulderRight, JointID.HipRight))
                {
                    LookLeft = false;
                    LookRight = true;
                }
                else
                {
                    LookRight = false;
                }
            }

            void DoLook_Up()
            {
                if (HandleOfGame != IntPtr.Zero)
                    Communicator.MouseMoveBack();
            }
            void DoLook_Down()
            {
                if (HandleOfGame != IntPtr.Zero)
                    Communicator.MouseMoveForward();
            }
            void DoLook_Left()
            {
                //if (HandleOfGame != IntPtr.Zero)
                //    Communicator.PressKey(HandleOfGame, 'a', value);
            }
            void DoLook_Right()
            {
                //if (HandleOfGame != IntPtr.Zero)
                //    Communicator.PressKey(HandleOfGame, 'd', value);
            }

            void DoAttack_LeftHand()
            {
                if (HandleOfGame != IntPtr.Zero)
                    Communicator.MouseLeftClick();
            }
            void DoAttack_RightHand()
            {
                if (HandleOfGame != IntPtr.Zero)
                    Communicator.MouseRightClick();
            }

            void FireEvent_LookUp()
            {
                if (EventLookUpOrDown != null)
                {
                    EventLookUpOrDown(true);
                }
                else
                {
                    Console.Write(string.Format("EventLookUpOrDown IsUp: {0}", true));
                }
            }
            void FireEvent_LookBack()
            {
                if (EventLookUpOrDown != null)
                {
                    EventLookUpOrDown(false);
                }
                else
                {
                    Console.Write(string.Format("EventLookUpOrDown IsUp: {0}", false));
                }
            }
            void FireEvent_DoLeft()
            {
                if (EventLookLeftOrRight != null)
                {
                    EventLookLeftOrRight(true);
                }
                else
                {
                    Console.Write(string.Format("EventLookLeftOrRight IsLeft: {0}", true));
                }
            }
            void FireEvent_DoRight()
            {
                if (EventLookLeftOrRight != null)
                {
                    EventLookLeftOrRight(false);
                }
                else
                {
                    Console.Write(string.Format("EventLookLeftOrRight IsLeft: {0}", false));
                }
            }
            #endregion
        }
    }
}
