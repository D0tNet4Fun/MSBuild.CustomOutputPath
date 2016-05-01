using System.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CustomOutputPath.Tasks.Tasks
{
    public class LookUpVisualStudioHostingProcess : Task
    {
        [Required]
        public string TargetName { get; set; }

        [Output]
        public bool IsRunning { get; private set; }

        public override bool Execute()
        {
            var processes = GetVisualStudioHostingProcesses();
            try
            {
                IsRunning = processes.Length > 0;
                if (IsRunning)
                {
                    Log.LogMessage(MessageImportance.Normal, $"Found {processes.Length} VS hosting processes for project {TargetName}.");
                }
            }
            finally
            {
                foreach (var process in processes)
                {
                    process.Dispose();
                }
            }
            return true;
        }

        private Process[] GetVisualStudioHostingProcesses()
        {
            var processName = $"{TargetName}.vshost";

            Log.LogMessage(MessageImportance.Normal, $"Finding Visual Studio hosting process for {TargetName} ({processName})");
            var processes = Process.GetProcessesByName(processName);
            return processes;
        }
    }
}