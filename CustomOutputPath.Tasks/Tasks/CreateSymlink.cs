using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CustomOutputPath.Tasks.Tasks
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
            if (string.IsNullOrEmpty(LinkName))
            {
                throw new InvalidOperationException("LinkName was not set");
            }
            if (string.IsNullOrEmpty(TargetName))
            {
                throw new InvalidOperationException("TargetName was not set");
            }

            var linkParentDirectory = Path.GetDirectoryName(LinkName.TrimEnd('\\'));
            try
            {
                if (linkParentDirectory != null && !Directory.Exists(linkParentDirectory))
                {
                    Directory.CreateDirectory(linkParentDirectory);
                }
            }
            catch (Exception ex)
            {
                Log.LogError($"Error creating symbolic link for {LinkName} <<====>> {TargetName}. Could not create parent directory {linkParentDirectory}: {ex.Message}");
                return false;
            }

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