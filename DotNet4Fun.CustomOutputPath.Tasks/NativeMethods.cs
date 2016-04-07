using System.Runtime.InteropServices;

namespace DotNet4Fun.CustomOutputPath.Tasks
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CreateSymbolicLink(
            [In] string symlinkFileName,
            [In] string targetFileName,
            SymlinkTargetType flags
            );
    }

    internal enum SymlinkTargetType
    {
        File,
        Directory
    }
}