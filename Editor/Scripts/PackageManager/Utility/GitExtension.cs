using System;
using System.Diagnostics;

namespace Editor.Scripts.PackageManager.Utility
{

internal static class GitExtension
{
    #region Public Methods

    public static string LatestGitTag(string repository)
    {
        return RunGit(@"git describe --tags --abbrev=0", repository);
    }

    public static string LatestGitCommit(string repository, string branch)
    {
        return RunGit($@"git rev-parse {branch}", repository);
    }

    #endregion

    #region Private Methods

    private static string RunGit(string arguments, string path)
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

    #endregion
}

}
