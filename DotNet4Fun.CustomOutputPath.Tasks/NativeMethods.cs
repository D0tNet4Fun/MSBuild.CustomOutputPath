using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

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

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] FileAccessEx fileAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            FileOptions flags,
            IntPtr template
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            int dwIoControlCode,
            [MarshalAs(UnmanagedType.AsAny)] [In] object inBuffer,
            int nInBufferSize,
            [MarshalAs(UnmanagedType.AsAny)] [Out] object outBuffer,
            int nOutBufferSize,
            ref int pBytesReturned,
            IntPtr lpOverlapped
            );

        [StructLayout(LayoutKind.Sequential)]
        public class ReparseDataBuffer
        {
            public uint ReparseTag;
            public ushort ReparseDataLength;
            public ushort Reserved;
            public ushort SubstituteNameOffset;
            public ushort SubstituteNameLength;
            public ushort PrintNameOffset;
            public ushort PrintNameLength;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32767)]
            public byte[] PathBuffer;

            public string GetTargetName()
            {
                return Encoding.Unicode.GetString(PathBuffer, PrintNameOffset, PrintNameLength);
            }
        }

        [Flags]
        public enum FileAccessEx : uint
        {
            None = 0x0,
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericReadWrite = GenericRead | GenericWrite
        }
    }

    internal enum SymlinkTargetType
    {
        File,
        Directory
    }
}