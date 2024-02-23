﻿using System.Diagnostics;
using System.Text;

namespace Editor.Scripts.PackageManager.Utility
{

internal static class ProcessExtensions
{
    #region Public Methods

    public static int Run(
        this Process process,
        string application,
        string arguments,
        string workingDirectory,
        out string output,
        out string errors)
    {
        process.StartInfo = new ProcessStartInfo
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            FileName = application,
            Arguments = arguments,
            WorkingDirectory = workingDirectory
        };
        
        var outputBuilder = new StringBuilder();
        var errorsBuilder = new StringBuilder();
        process.OutputDataReceived += (_, args) => outputBuilder.AppendLine(args.Data);
        process.ErrorDataReceived += (_, args) => errorsBuilder.AppendLine(args.Data);
        
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        output = outputBuilder.ToString().TrimEnd();
        errors = errorsBuilder.ToString().TrimEnd();

        return process.ExitCode;
    }

    #endregion
}

}