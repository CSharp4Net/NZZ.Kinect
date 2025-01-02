using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Diagnostics;
using System.Windows.Input;

namespace NZZ.Kinect
{
    public class Communicator
    {
        [DllImport("user32.dll")]
        static extern bool BringWindowToTop(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void PressKey(IntPtr handle, char ch, bool holdPressed)
        {
            if (!BringWindowToTop(handle)) Console.Write("failed"); ;
            if (!SetForegroundWindow(handle)) Console.Write("failed");
            Thread.Sleep(250);

            byte vk = WindowsAPI.VkKeyScan(ch);
            ushort scanCode = (ushort)WindowsAPI.MapVirtualKey(vk, 0);

            if (holdPressed)
                KeyDown(scanCode);
            else
                KeyUp(scanCode);
        }

        public static void KeyDown(ushort scanCode)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.DeviceKeyboard;
            inputs[0].ki.dwFlags = 0;
            inputs[0].ki.wScan = (ushort)(scanCode & 0xff);

            uint intReturn = WindowsAPI.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key: " + scanCode);
            }
        }
        public static void KeyUp(ushort scanCode)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.DeviceKeyboard;
            inputs[0].ki.wScan = scanCode;
            inputs[0].ki.dwFlags = WindowsAPI.KEYEVENTF_KEYUP;
            uint intReturn = WindowsAPI.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key: " + scanCode);
            }
        }

        public static void MouseMoveForward()
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.DeviceMouse;
            inputs[0].ki.dwFlags = WindowsAPI.MOUSEEVENTF_RIGHTDOWN;
            uint intReturn = WindowsAPI.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send mouse click left.");
            }
        }

        public static void MouseMoveBack()
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.DeviceMouse;
            inputs[0].ki.dwFlags = WindowsAPI.MOUSEEVENTF_LEFTDOWN;
            uint intReturn = WindowsAPI.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send mouse click left.");
            }
        }

        public static void MouseLeftClick()
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.DeviceMouse;
            inputs[0].ki.dwFlags = WindowsAPI.MK_LBUTTON;
            uint intReturn = WindowsAPI.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send mouse click left.");
            }
        }
        public static void MouseRightClick()
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.DeviceMouse;
            inputs[0].ki.dwFlags = WindowsAPI.WM_LBUTTONDOWN;
            uint intReturn = WindowsAPI.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send mouse click right.");
            }
        }

        public class WindowsAPI
        {
            public const uint WM_KEYDOWN = 0x100;
            public const uint WM_KEYUP = 0x101;
            public const uint WM_LBUTTONDOWN = 0x201;
            public const uint WM_LBUTTONUP = 0x202;

            public const uint WM_CHAR = 0x102;

            public const int MK_LBUTTON = 0x01;

            public const int KeyReturn = 0x0d;
            public const int KeyEscape = 0x1b;
            public const int KeyAlt = 0x10;
            public const int KeyTabulator = 0x09;

            public const int VK_LEFT = 0x25;
            public const int VK_UP = 0x26;
            public const int VK_RIGHT = 0x27;
            public const int VK_DOWN = 0x28;

            public const int KeyF1 = 0x70;
            public const int KeyF2 = 0x71;
            public const int KeyF3 = 0x72;
            public const int KeyF4 = 0x73;
            public const int KeyF5 = 0x74;
            public const int KeyF6 = 0x75;
            public const int KeyF7 = 0x76;
            public const int KeyF8 = 0x77;
            public const int KeyF9 = 0x78;
            public const int KeyF10 = 0x79;
            public const int KeyF11 = 0x80;
            public const int KeyF12 = 0x81;

            public const int DeviceMouse = 0;
            public const int DeviceKeyboard = 1;
            public const int DeviceHardware = 2;

            public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
            public const uint KEYEVENTF_KEYUP = 0x0002;
            public const uint KEYEVENTF_UNICODE = 0x0004;
            public const uint KEYEVENTF_SCANCODE = 0x0008;
            public const uint XBUTTON1 = 0x0001;
            public const uint XBUTTON2 = 0x0002;
            public const uint MOUSEEVENTF_MOVE = 0x0001;
            public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
            public const uint MOUSEEVENTF_LEFTUP = 0x0004;
            public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
            public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
            public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
            public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
            public const uint MOUSEEVENTF_XDOWN = 0x0080;
            public const uint MOUSEEVENTF_XUP = 0x0100;
            public const uint MOUSEEVENTF_WHEEL = 0x0800;
            public const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
            public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

            public static void SwitchWindow(IntPtr windowHandle)
            {
                if (GetForegroundWindow() == windowHandle)
                    return;

                IntPtr foregroundWindowHandle = GetForegroundWindow();
                uint currentThreadId = GetCurrentThreadId();
                uint temp;
                uint foregroundThreadId = GetWindowThreadProcessId(foregroundWindowHandle, out temp);
                AttachThreadInput(currentThreadId, foregroundThreadId, true);
                SetForegroundWindow(windowHandle);
                AttachThreadInput(currentThreadId, foregroundThreadId, false);

                while (GetForegroundWindow() != windowHandle)
                {
                }
            }

            #region Extern functions
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();
            [DllImport("kernel32.dll")]
            public static extern uint GetCurrentThreadId();
            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
            [DllImport("user32.dll")]
            public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out()] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
            [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
            public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
            [DllImport("user32.dll")]
            public static extern byte VkKeyScan(char ch);
            [DllImport("user32.dll")]
            public static extern uint MapVirtualKey(uint uCode, uint uMapType);
            [DllImport("user32.dll")]
            public static extern IntPtr SetFocus(IntPtr hWnd);
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
            [DllImport("User32.dll")]
            public static extern uint SendInput(uint numberOfInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT[] input, int structSize);
            [DllImport("user32.dll")]
            public static extern IntPtr GetMessageExtraInfo();
            #endregion

            public static int MakeLong(int low, int high)
            {
                return (high << 16) | (low & 0xffff);
            }
        }

        #region Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            int dx;
            int dy;
            uint mouseData;
            uint dwFlags;
            uint time;
            IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            uint uMsg;
            ushort wParamL;
            ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)] //*
            public MOUSEINPUT mi;
            [FieldOffset(4)] //*
            public KEYBDINPUT ki;
            [FieldOffset(4)] //*
            public HARDWAREINPUT hi;
        }
        #endregion
    }
}
