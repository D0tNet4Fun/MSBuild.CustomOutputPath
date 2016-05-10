using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CustomOutputPath.Tasks.Tasks
{
    public class ConvertToSymlink : Task
    {
        [Required]
        public string LinkName { get; set; }

        [Required]
        public string TargetName { get; set; }

        public string TargetType { get; set; }

        private SymlinkTargetType SymlinkTargetType
        {
            get
            {
                SymlinkTargetType targetType;
                if (!Enum.TryParse(TargetType, out targetType))
                {
                    throw new InvalidOperationException("Unknown target type ");
                }
                return targetType;
            }
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

            var exists = SymlinkTargetType == SymlinkTargetType.Directory ? Directory.Exists(LinkName) : File.Exists(LinkName);
            if (exists)
            {
                // if this is already a symlink, check if it points to the same target and if not then delete it
                var isSymlink = (File.GetAttributes(LinkName) & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
                if (isSymlink)
                {
                    var querySymlinkTask = new QuerySymlink { LinkName = LinkName, TargetName = TargetName, TargetType = TargetType, BuildEngine = BuildEngine };
                    if (!querySymlinkTask.Execute())
                    {
                        return false;
                    }
                    Log.LogMessage(MessageImportance.Low, $"{SymlinkTargetType} '{LinkName}' is a symlink which points to '{querySymlinkTask.TargetName}'");
                    if (string.Equals(querySymlinkTask.TargetName, TargetName.TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase))
                    {
                        // the symlink's target matches our target
                        return true;
                    }
                    Log.LogMessage(MessageImportance.Normal, $"Deleting symlink '{LinkName}' because it points to a different target '{querySymlinkTask.TargetName}'");
                }
                else
                {
                    Log.LogMessage(MessageImportance.Normal, $"{SymlinkTargetType} '{LinkName}' needs to be deleted before converting it to a symlink");
                }
                // if this is reached then delete is needed
                DeleteDirectoryOrFile();
            }

            var createSymlinkTask = new CreateSymlink { LinkName = LinkName, TargetName = TargetName, TargetType = TargetType, BuildEngine = BuildEngine };
            return createSymlinkTask.Execute();
        }

        private void DeleteDirectoryOrFile()
        {
            switch (SymlinkTargetType)
            {
                case SymlinkTargetType.Directory:
                    Directory.Delete(LinkName);
                    break;
                case SymlinkTargetType.File:
                    File.Delete(LinkName);
                    break;
            }
        }
    }
}