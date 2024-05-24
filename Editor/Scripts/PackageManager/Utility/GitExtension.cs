#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MegaPint.Editor.Scripts.PackageManager.Utility
{

/// <summary> Class to get information via git methods </summary>
internal static class GitExtension
{
    #region Public Methods

    /// <summary> Get the latest public tag of a specified repository </summary>
    /// <param name="repository"> Git url of the target repository </param>
    /// <returns> The latest found tag of any branch </returns>
    public static string LatestGitTag(string repository)
    {
        return GitTags(repository)[^1];
    }

    #endregion

    #region Private Methods

    private static string[] GitTags(string repository)
    {
        List <string> tags = new();

        var outPut = RunGit($"ls-remote --tags {repository}");

        var ses = outPut.Split("tags/");

        for (var i = 1; i < ses.Length; i++)
        {
            var s = ses[i];
            tags.Add(s.Split("\n")[0]);
        }

        return tags.ToArray();
    }

    private static string RunGit(string arguments)
    {
        using var process = new Process();

        var exitCode = process.Run(
            "git",
            arguments,
            out var output,
            out var errors);

        if (exitCode == 0)
            return output;

        throw new Exception($"Git Exit Code: {exitCode} - {errors}");
    }

    #endregion
}

}
#endif
