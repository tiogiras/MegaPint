#if UNITY_EDITOR
#if UNITY_INCLUDE_TESTS
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MegaPint.Editor.Scripts.Tests.Utility
{

/// <summary> Partial utility class for unit testing </summary>
internal static partial class TestsUtility
{
    #region Public Methods

    /// <summary> Validate the naming of all files in the folder and subfolders </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path to the targeted folder </param>
    /// <param name="excludedDirectories"> Directories and content are ignored </param>
    public static void ValidateNamingOfFilesInFolderAndSubFolders(
        ref bool isValid,
        string path,
        params string[] excludedDirectories)
    {
        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            if (IsInExcludedDirectory(file, excludedDirectories))
                continue;

            var extension = Path.GetExtension(file);

            switch (extension)
            {
                case ".cs" or ".uss":
                    ValidateCSharpFileNaming(ref isValid, file);

                    break;

                case ".asmdef":
                    ValidateAssemblyFileNaming(ref isValid, file);

                    break;

                case ".asmref":
                    ValidateAssemblyFileNaming(ref isValid, file);

                    break;

                case ".meta":
                    break;

                default:
                    ValidateDefaultFileNaming(ref isValid, file);

                    break;
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary> Insert a space in front of any starting digit sequence </summary>
    /// <param name="wanted"></param>
    private static void InsertSpaceBeforeDigit(ref string wanted)
    {
        if (!wanted.Any(char.IsDigit))
            return;

        var wasDigit = false;

        for (var i = wanted.Length - 1; i >= 0; i--)
        {
            var c = wanted[i];

            if (c.Equals(' ') || c.Equals('('))
            {
                wasDigit = false;

                continue;
            }

            if (!char.IsDigit(c))
            {
                if (!wasDigit)
                    continue;

                wanted = wanted.Insert(i + 1, " ");
                wasDigit = false;
            }
            else
                wasDigit = true;
        }
    }

    /// <summary> Check if the path contains any of the excluded directories </summary>
    /// <param name="path"> Targeted path </param>
    /// <param name="excludedDirectories"> Directories to exclude </param>
    /// <returns> If the path contains any of the excluded directories </returns>
    private static bool IsInExcludedDirectory(string path, params string[] excludedDirectories)
    {
        return excludedDirectories.Any(directory => path.Contains($"{Path.DirectorySeparatorChar}{directory}"));
    }

    /// <summary> Remove any space that is after any of the specified special characters </summary>
    /// <param name="source"> Source string </param>
    /// <returns> Cleaned string </returns>
    private static string RemoveSpaceAfterSpecialCharacters(string source)
    {
        var wasSpace = false;

        var output = source;

        for (var i = source.Length - 1; i >= 0; i--)
        {
            var c = source[i];

            if (wasSpace)
            {
                if (c.Equals('('))
                    output = output.Remove(i + 1, 1);
                else
                    wasSpace = false;
            }

            if (c.Equals(' '))
                wasSpace = true;
        }

        return output;
    }

    /// <summary> Enforce that the first character is upper case </summary>
    /// <param name="source"> Targeted string </param>
    /// <returns> String with upper case letter at the start </returns>
    private static string StartWithUpperCase(string source)
    {
        return source.Length <= 1 ? source : $"{char.ToUpper(source[0])}{source[1..]}";
    }

    /// <summary> Validate the naming of an assembly </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path to the assembly </param>
    private static void ValidateAssemblyFileNaming(ref bool isValid, string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);

        var valid = !fileName.Any(c => c is ' ' or '_');

        if (fileName.Any(char.IsUpper))
            valid = false;

        Validate(ref isValid, !valid, $"Naming issue found in file [{path}]");
    }

    /// <summary> Validate the naming of an cs file </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path to the cs file </param>
    private static void ValidateCSharpFileNaming(ref bool isValid, string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);

        var valid = !fileName.Any(c => c is ' ' or '.' or '_');

        if (!char.IsUpper(fileName[0]))
            valid = false;

        Validate(ref isValid, !valid, $"Naming issue found in file [{path}]");
    }

    /// <summary> Validate the naming of the given file </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path to the file </param>
    private static void ValidateDefaultFileNaming(ref bool isValid, string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);

        var wanted = fileName.Replace("MegaPint", "Megapint");

        wanted = Regex.Replace(wanted, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
        wanted = wanted.Replace("_", " ");

        wanted = RemoveSpaceAfterSpecialCharacters(wanted);

        var spaceSplits = wanted.Split(" ");

        if (spaceSplits.Length > 0)
        {
            var resultBuilder = new StringBuilder(wanted.Length);

            foreach (var split in spaceSplits)
            {
                var cleanedSplit = StartWithUpperCase(split);

                if (string.IsNullOrEmpty(cleanedSplit))
                    continue;

                resultBuilder.Append($"{cleanedSplit} ");
            }

            wanted = resultBuilder.ToString()[..^1];
        }

        InsertSpaceBeforeDigit(ref wanted);

        wanted = wanted.Replace("Megapint", "MegaPint");

        var valid = wanted.Equals(fileName);

        Validate(ref isValid, !valid, $"Naming issue found in file [{path}] should be [{wanted}]");
    }

    #endregion
}

}
#endif
#endif
