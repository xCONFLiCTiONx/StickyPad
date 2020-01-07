using System;
using System.Diagnostics;

namespace StickyPad
{
    public class SingleInstance
    {
        internal static void Check()
        {
            foreach (Process proc in Process.GetProcessesByName("StickyPad"))
            {
                IntPtr hWnd = WindowHelper.FindWindow(null, "Sticky Pad");

                WindowHelper.GetWindowThreadProcessId(hWnd, out uint id);

                if (id != 0)
                {
                    if (id != Process.GetCurrentProcess().Id)
                    {
                        WindowHelper.SetWindowPos(hWnd, WindowHelper.HWND_TOPMOST, 0, 0, 0, 0, WindowHelper.SWP_NOMOVE | WindowHelper.SWP_NOSIZE | WindowHelper.SWP_SHOWWINDOW);


                        WindowHelper.SetWindowPos(hWnd, WindowHelper.HWND_NO_TOPMOST, 0, 0, 0, 0, WindowHelper.SWP_NOMOVE | WindowHelper.SWP_NOSIZE | WindowHelper.SWP_SHOWWINDOW);

                        WindowHelper.SetForegroundWindow(hWnd);

                        Environment.Exit(0);
                    }
                }
            }
        }
    }
}
