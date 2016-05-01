using System;
using System.Runtime.InteropServices;

namespace CustomOutputPath.Tasks
{
    internal static class Win32Utils
    {
        public static Exception GetExceptionForLastError()
        {
            int lastErrorHr = Marshal.GetHRForLastWin32Error();
            return Marshal.GetExceptionForHR(lastErrorHr);
        }
    }
}