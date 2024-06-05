#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using UnityEngine;

[assembly: InternalsVisibleTo("tiogiras.megapint.editor.tests")]
[assembly: InternalsVisibleTo("tiogiras.megapint.alphabutton.editor.tests")]
[assembly: InternalsVisibleTo("tiogiras.megapint.validators.editor.tests")]
[assembly: InternalsVisibleTo("tiogiras.megapint.playmodestartscene.editor.tests")]
[assembly: InternalsVisibleTo("tiogiras.megapint.notepad.editor.tests")]
[assembly: InternalsVisibleTo("tiogiras.megapint.autosave.editor.tests")]
[assembly: InternalsVisibleTo("tiogiras.megapint.screenshot.editor.tests")]
namespace MegaPint.Editor.Scripts.Tests.Utility
{

/// <summary> Partial utility class for unit testing </summary>
internal static partial class TestsUtility
{
    #region Public Methods

    /// <summary> Execute an async task as an <see cref="IEnumerator{T}" /> to call them in other unit tests </summary>
    /// <param name="task"> Targeted async task </param>
    /// <typeparam name="T"> Type of the task </typeparam>
    /// <returns> Task as <see cref="IEnumerator{T}" /> </returns>
    public static IEnumerator AsIEnumeratorReturnNull <T>(this Task <T> task)
    {
        while (!task.IsCompleted)
            yield return null;

        if (task.IsFaulted)
        {
            if (task.Exception != null)
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
        }

        yield return null;
    }

    /// <summary> Validate a given condition and log if the condition is true </summary>
    /// <param name="valid"> Set to false if the condition is not met </param>
    /// <param name="condition"> Condition to be checked </param>
    /// <param name="log"> Message to be logged if the condition is not met </param>
    /// <returns> If the condition was met </returns>
    public static bool Validate(ref bool valid, bool condition, string log = "")
    {
        if (!condition)
            return false;

        valid = false;
        Debug.LogWarning($"\t- {log}");

        return true;
    }

    #endregion

    #region Private Methods

    /// <summary> Validate directories inside a directory </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path to the targeted directory </param>
    /// <param name="required"> Required directories to pass the test </param>
    /// <param name="tolerated"> Tolerated directories </param>
    /// <param name="requiredDirectories"> Existence of the required directories </param>
    /// <param name="toleratedDirectories"> Existence of the tolerated directories </param>
    /// <param name="tolerateAny"> If true do not fail if any directory is found </param>
    private static void ValidateDirectories(
        ref bool isValid,
        string path,
        IReadOnlyList <string> required,
        IReadOnlyList <string> tolerated,
        out bool[] requiredDirectories,
        out bool[] toleratedDirectories,
        bool tolerateAny = false)
    {
        requiredDirectories = null;
        toleratedDirectories = null;

        if (required != null)
            requiredDirectories = new bool[required.Count];

        if (tolerated != null)
            toleratedDirectories = new bool[tolerated.Count];

        List <string> directories = Directory.GetDirectories(path).ToList();

        if (required != null)
        {
            for (var i = 0; i < required.Count; i++)
            {
                var directoryName = required[i];
                var directoryPath = Path.Combine(path, directoryName);

                var exists = directories.Contains(directoryPath);
                requiredDirectories[i] = exists;

                Validate(ref isValid, !exists, $"\t- Missing directory with the name: {directoryName}");

                directories.Remove(directoryPath);
            }
        }

        if (tolerated != null)
        {
            for (var i = 0; i < tolerated.Count; i++)
            {
                var directoryName = tolerated[i];
                var directoryPath = Path.Combine(path, directoryName);

                var exists = directories.Contains(directoryPath);
                toleratedDirectories[i] = exists;

                directories.Remove(directoryPath);
            }
        }

        if (directories.Count == 0 || tolerateAny)
            return;

        isValid = false;
        Debug.LogWarning($"\t- Excess sub directories found! \n\t\t{string.Join("\n\t\t", directories)}");
    }

    /// <summary> Validate files inside a directory </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path to the targeted directory </param>
    /// <param name="required"> Required files to pass the test </param>
    /// <param name="tolerated"> Tolerated files </param>
    /// <param name="requiredFiles"> Existence of the required files </param>
    /// <param name="toleratedFiles"> Existence of the tolerated files </param>
    /// <param name="tolerateAny"> If true do not fail if any file is found </param>
    private static void ValidateFiles(
        ref bool isValid,
        string path,
        IReadOnlyList <string> required,
        IReadOnlyList <string> tolerated,
        out bool[] requiredFiles,
        out bool[] toleratedFiles,
        bool tolerateAny = false)
    {
        requiredFiles = null;
        toleratedFiles = null;

        if (required != null)
            requiredFiles = new bool[required.Count];

        if (tolerated != null)
            toleratedFiles = new bool[tolerated.Count];

        List <string> files = Directory.GetFiles(path).ToList();

        if (required != null)
        {
            for (var i = 0; i < required.Count; i++)
            {
                var fileName = required[i];
                var filePath = Path.Combine(path, fileName);

                var exists = files.Contains(filePath);
                requiredFiles[i] = exists;

                Validate(ref isValid, !exists, $"Missing file with the name: {fileName}");

                files.Remove(filePath);
            }
        }

        if (tolerated != null)
        {
            for (var i = 0; i < tolerated.Count; i++)
            {
                var directoryName = tolerated[i];
                var directoryPath = Path.Combine(path, directoryName);

                var exists = files.Contains(directoryPath);
                toleratedFiles[i] = exists;

                files.Remove(directoryPath);
            }
        }

        if (files.Count == 0 || tolerateAny)
            return;

        isValid = false;
        Debug.LogWarning($"\t- Excess files found!\n\t\t{string.Join("\n\t\t", files)}");
    }

    #endregion
}

}
#endif
