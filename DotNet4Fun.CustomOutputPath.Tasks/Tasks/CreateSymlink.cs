using System;
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DotNet4Fun.CustomOutputPath.Tasks.Tasks
{
    public class CreateSymlink : Task
    {
        private SymlinkTargetType _targetType;

        [Required]
        public string LinkName { get; set; }

        [Required]
        public string TargetName { get; set; }

        public string TargetType
        {
            get { return _targetType.ToString(); }
            set { _targetType = (SymlinkTargetType)Enum.Parse(typeof(SymlinkTargetType), value, true); }
        }

        public override bool Execute()
        {
            var result = NativeMethods.CreateSymbolicLink(LinkName.TrimEnd('\\'), TargetName.TrimEnd('\\'), _targetType);
            if (!result)
            {
                const uint requiredPrivilegeError = 0x80070522;
                if ((uint)Marshal.GetHRForLastWin32Error() == requiredPrivilegeError)
                {
                    Log.LogError($"Error creating symbolic link for {LinkName} <<====>> {TargetName}. Try running the build from an elevated process.");
                    return false;
                }

                var exception = Win32Utils.GetExceptionForLastError();
                Log.LogError($"Error creating symbolic link for {LinkName} <<====>> {TargetName}: {exception.Message}");
                return false;
            }
            Log.LogMessage(MessageImportance.High, $"Symbolic link created for {LinkName} <<====>> {TargetName}");
            return true;
        }
    }
}