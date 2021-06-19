using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows;

namespace controller
{

    class mouseController
    {

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        // public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr dwExtraInfo);
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;
        

       /* public static void sendMouseRightclick(int x, int y)
        {
            UIntPtr z = (UIntPtr)0;
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, (uint)x, (uint)y, 0, z);
        }*/
    }
}
