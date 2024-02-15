using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Editor.Scripts.PackageManager
{

internal static class GitExtension
{
    #region Public Methods

    public static string RunGit(string arguments, string path)
    {
        using var process = new Process();

        var exitCode = process.Run(
            "git",
            arguments,
            path,
            out var output,
            out var errors);

        if (exitCode == 0)
            return output;

        throw new Exception($"Git Exit Code: {exitCode} - {errors}");
    }

    public static string LatestGitTag(string repository)
    {
        var tag = RunGit(@"git describe --tags --abbrev=0", repository);
        Debug.Log(tag);

        return tag;
    }

    #endregion
}

}
