using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DotNet4Fun.CustomOutputPath.Tasks.Tasks
{
    public class QuerySymlink : Task
    {
        [Required]
        public string LinkName { get; set; }

        [Output]
        public string TargetName { get; set; }

        [Output]
        public string TargetType { get; set; }

        public override bool Execute()
        {
            var reparseDataBuffer = GetReparsePointInfo();
            if (reparseDataBuffer != null)
            {
                TargetName = reparseDataBuffer.GetTargetName();
            }
            if (TargetName != null)
            {
                try
                {
                    var attributes = File.GetAttributes(TargetName);
                    var targetType = (attributes & FileAttributes.Directory) == FileAttributes.Directory ? SymlinkTargetType.Directory : SymlinkTargetType.File;
                    TargetType = targetType.ToString();
                }
                catch (FileNotFoundException)
                {
                    // ignore
                }
                catch (DirectoryNotFoundException)
                {
                    // ignore
                }
            }
            return true;
        }

        private NativeMethods.ReparseDataBuffer GetReparsePointInfo()
        {
            const uint fileOpenReparsePoint = 0x00200000;
            const uint fileFlagBackupSemantics = 0x02000000;
            const FileOptions flags = (FileOptions)(fileOpenReparsePoint | fileFlagBackupSemantics);
            using (var handle = NativeMethods.CreateFile(LinkName, NativeMethods.FileAccessEx.None, FileShare.Read, IntPtr.Zero, FileMode.Open, flags, IntPtr.Zero))
            {
                if (handle.IsInvalid)
                {
                    throw Win32Utils.GetExceptionForLastError();
                }

                var reparseDataBuffer = new NativeMethods.ReparseDataBuffer();

                const int fsctlGetReparsePoint = 0x000900A8;
                var bytesReturned = 0;
                var success = NativeMethods.DeviceIoControl(handle, fsctlGetReparsePoint,
                    IntPtr.Zero, 0,
                    reparseDataBuffer, Marshal.SizeOf(typeof(NativeMethods.ReparseDataBuffer)),
                    ref bytesReturned, IntPtr.Zero);

                if (!success)
                {
                    const uint pathNotAReparsePointError = 0x80071126;
                    if ((uint)Marshal.GetHRForLastWin32Error() == pathNotAReparsePointError)
                    {
                        return null;
                    }
                    throw Win32Utils.GetExceptionForLastError();
                }
                return reparseDataBuffer;
            }
        }
    }
}