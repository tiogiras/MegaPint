#if UNITY_EDITOR
using System.IO;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using NUnit.Framework;
using UnityEngine;

namespace MegaPint.Editor.Scripts.Tests.Utility
{

/// <summary> Partial utility class for unit testing </summary>
internal static partial class TestsUtility
{
    #region Public Methods

    /// <summary> Validate the structure of a package </summary>
    /// <param name="key"> Key to the targeted package </param>
    public static void CheckStructure(PackageKey key)
    {
        var dataPath = Application.dataPath[..^7];
        var path = Path.Combine(dataPath, "Packages", PackageCache.Get(key).Name);

        if (!Directory.Exists(path))
            Assert.Fail($"Package directory not found at path: {path}");

        var isValid = true;

        CheckTopLevelFiles(ref isValid, path);
        CheckTopLevelDirectories(ref isValid, path, key);

        Assert.IsTrue(isValid);
    }

    #endregion

    #region Private Methods

    /// <summary> Validate the editor directory of a package </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path of the package directory </param>
    /// <param name="key"> Key of the targeted package </param>
    private static void CheckEditorDirectory(ref bool isValid, string path, PackageKey key)
    {
        Debug.Log("\nChecking editor directory...");

        var localIsValid = true;
        path = Path.Combine(path, "Editor");

        ValidateDirectories(
            ref localIsValid,
            path,
            new[] {"Resources", "Scripts"},
            null,
            out var requiredDirectories,
            out var _);

        if (requiredDirectories[0])
            ValidateResourceDirectory(ref localIsValid, path, key);

        if (requiredDirectories[1])
        {
            ValidateNamingOfFilesInFolderAndSubFolders(ref localIsValid, Path.Combine(path, "Scripts"));

            ValidateFiles(
                ref localIsValid,
                Path.Combine(path, "Scripts"),
                new[] {"Constants.cs", "ContextMenu.cs", "DisplayContent.cs", "SaveValues.cs"},
                null,
                out var _,
                out var _,
                true);
        }

        var assemblyName = GetAssemblyName(PackageKey.Undefined, "editor");

        ValidateFiles(
            ref localIsValid,
            path,
            new[] {assemblyName},
            new[] {$"{assemblyName}.meta", "Resources.meta", "Scripts.meta"},
            out var required,
            out var _);

        if (required[0])
            ValidateAsmref(ref localIsValid, path, assemblyName);

        if (localIsValid)
            Debug.Log("\t===> No issues found!");
        else
            isValid = false;
    }

    /// <summary> Validate the license file of the package </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path to the license file </param>
    private static void CheckLicenseFile(ref bool isValid, string path)
    {
        Validate(
            ref isValid,
            string.IsNullOrEmpty(File.ReadAllText(path)),
            "License file is empty!");
    }

    /// <summary> Validate the package json of the package </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path to the json file </param>
    private static void CheckPackageJson(ref bool isValid, string path)
    {
        Validate(
            ref isValid,
            string.IsNullOrEmpty(File.ReadAllText(path)),
            "Package json file is empty!");

        // TODO move the package cache data evaluation for this package to here
    }

    /// <summary> Validate the readme of the package </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path to the readme file </param>
    private static void CheckReadMe(ref bool isValid, string path)
    {
        Validate(
            ref isValid,
            string.IsNullOrEmpty(File.ReadAllText(path)),
            "Readme file is empty!");
    }

    /// <summary> Validate the runtime directory of a package </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path to the package directory </param>
    /// <param name="key"> Key of the targeted package </param>
    private static void CheckRuntimeDirectory(ref bool isValid, string path, PackageKey key)
    {
        Debug.Log("\nChecking runtime directory...");

        var localIsValid = true;
        path = Path.Combine(path, "Runtime");

        ValidateDirectories(ref localIsValid, path, new[] {"Scripts"}, null, out var requiredDirectories, out var _);

        if (requiredDirectories[0])
            ValidateNamingOfFilesInFolderAndSubFolders(ref localIsValid, Path.Combine(path, "Scripts"));

        var assemblyName = GetAssemblyName(key, "runtime");

        ValidateFiles(
            ref localIsValid,
            path,
            new[] {assemblyName},
            new[] {$"{assemblyName}.meta", "Scripts.meta"},
            out var required,
            out var _);

        if (required[0])
            ValidateAssembly(ref localIsValid, path, assemblyName);

        if (localIsValid)
            Debug.Log("\t===> No issues found!");
        else
            isValid = false;
    }

    /// <summary> Validate the toplevel directories of a package's directory </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path to the package directory </param>
    /// <param name="key"> Key of the targeted package </param>
    private static void CheckTopLevelDirectories(ref bool isValid, string path, PackageKey key)
    {
        Debug.Log("\nChecking toplevel directories...");

        var localIsValid = true;

        ValidateDirectories(
            ref localIsValid,
            path,
            new[] {"Editor"},
            new[] {"Runtime", ".git"},
            out var required,
            out var tolerated);

        if (localIsValid)
            Debug.Log("\t===> No issues found!");
        else
            isValid = false;

        if (tolerated[0])
            CheckRuntimeDirectory(ref isValid, path, key);

        if (required[0])
            CheckEditorDirectory(ref isValid, path, key);
    }

    /// <summary> Validate the toplevel files of a package's directory </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path of the package directory </param>
    private static void CheckTopLevelFiles(ref bool isValid, string path)
    {
        Debug.Log("\nChecking toplevel files...");

        var localIsValid = true;

        ValidateFiles(
            ref localIsValid,
            path,
            new[] {"LICENSE", "package.json", "README.md"},
            new[] {"LICENSE.meta", "package.json.meta", "README.md.meta", "Editor.meta", "Runtime.meta"},
            out var required,
            out var _);

        if (required[0])
            CheckLicenseFile(ref localIsValid, Path.Combine(path, "LICENSE"));

        if (required[1])
            CheckPackageJson(ref localIsValid, Path.Combine(path, "package.json"));

        if (required[2])
            CheckReadMe(ref localIsValid, Path.Combine(path, "README.md"));

        if (localIsValid)
            Debug.Log("\t===> No issues found!");
        else
            isValid = false;
    }

    /// <summary> Validate the resources directory </summary>
    /// <param name="isValid"> Reference to the validation bool </param>
    /// <param name="path"> Path to the editor directory </param>
    /// <param name="key"> Key of the targeted package </param>
    private static void ValidateResourceDirectory(ref bool isValid, string path, PackageKey key)
    {
        path = Path.Combine(path, "Resources");

        ValidateDirectories(ref isValid, path, new[] {"MegaPint"}, null, out var required, out var _);
        ValidateFiles(ref isValid, path, null, new[] {"MegaPint.meta"}, out var _, out var _);

        if (!required[0])
            return;

        path = Path.Combine(path, "MegaPint");

        var packageName = key.ToString();

        ValidateDirectories(ref isValid, path, new[] {packageName}, null, out required, out var _);
        ValidateFiles(ref isValid, path, null, new[] {$"{packageName}.meta"}, out var _, out var _);

        if (!required[0])
            return;

        path = Path.Combine(path, packageName);

        ValidateDirectories(
            ref isValid,
            path,
            new[] {"User Interface"},
            new[] {"Images"},
            out required,
            out var tolerated);

        ValidateFiles(ref isValid, path, null, new[] {"User Interface.meta", "Images.meta"}, out var _, out var _);

        if (required[0])
            ValidateNamingOfFilesInFolderAndSubFolders(ref isValid, Path.Combine(path, "User Interface"));

        if (tolerated[0])
            ValidateNamingOfFilesInFolderAndSubFolders(ref isValid, Path.Combine(path, "Images"));
    }

    #endregion
}

}
#endif
