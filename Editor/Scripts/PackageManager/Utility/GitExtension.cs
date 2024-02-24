using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

namespace Editor.Scripts.PackageManager.Utility
{

internal static class GitExtension
{
    #region Public Methods

    /*public static string LatestGitTag(string repository)
    {
        //return RunGit(@"git describe --tags --abbrev=0", repository);
    }

    public static string LatestGitCommit(string repository, string branch)
    {
        //return RunGit($@"git rev-parse {branch}", repository);
    }*/

    public static string LatestGitTag(string repository)
    {
        return GitTags(repository)[^1];
    }
    
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
    
    public static string LatestGitCommitRemote(string repository, string branch)
    {
        var outPut = RunGit($"ls-remote {repository} refs/heads/{branch}");
        var index = outPut.IndexOf("refs", StringComparison.Ordinal);

        return outPut[..(index - 1)];
    }

    #endregion

    #region Private Methods

    private static string RunGit(string arguments/*, string path*/)
    {
        using var process = new Process();

        var exitCode = process.Run(
            "git",
            arguments,
            /*path,*/
            out var output,
            out var errors);

        if (exitCode == 0)
            return output;

        throw new Exception($"Git Exit Code: {exitCode} - {errors}");
    }

    #endregion
}

}
