#if UNITY_EDITOR
using System.IO;
using MegaPint.Editor.Scripts.PackageManager.Cache;
using MegaPint.Editor.Scripts.PackageManager.Packages;
using Unity.Plastic.Newtonsoft.Json;

namespace MegaPint.Editor.Scripts.Tests.Utility
{

/// <summary> Partial utility class for unit testing </summary>
internal static partial class TestsUtility
{
    private static string GetAssemblyName(PackageKey key, string specialization, bool definitionFile = false)
    {
        var packageName = key == PackageKey.Undefined ? "" : $".{PackageCache.Get(key).DisplayName.ToLower()}";
        var spec = string.IsNullOrEmpty(specialization) ? "" : $".{specialization}";
        var extension = definitionFile ? "asmdef" : "asmref";
        
        return $"tiogiras.megapint{packageName}{spec}.{extension}";
    }
    
    // TODO Version naming ???
    private static void ValidateAssembly(ref bool isValid, string path, string assemblyName,  bool definitionFile = false)
    {
        var hasAsmdef = GetAsmdef(path, assemblyName, out var asmdefPath, definitionFile);

        if (!hasAsmdef)
        {
            Validate(ref isValid, true, $"Missing assembly definition file! Expected one at path [{asmdefPath}]");
            return;
        }

        ValidateAsmdef(ref isValid, asmdefPath, assemblyName);

        if (!definitionFile)
            ValidateAsmref(ref isValid, path, assemblyName);
    }

    private static bool GetAsmdef(string path, string assemblyName, out string asmdefPath, bool definitionFile = false)
    {
        if (definitionFile)
            asmdefPath = Path.Combine(path, assemblyName);
        else
        {
            var name = assemblyName[..^7];
            var dataPath = path.Split("com.tiogiras.megapint")[0];
            
            asmdefPath = Path.Combine(dataPath, "com.tiogiras.megapint", "Editor", "Additional Namespaces", name, $"{name}.asmdef");
        }

        return File.Exists(asmdefPath);
    }
    
    private static void ValidateAsmdef(ref bool isValid, string asmdefPath, string assemblyName)
    {
        dynamic asmdef = JsonConvert.DeserializeObject(File.ReadAllText(asmdefPath));

        var name = (string)asmdef!["name"];
        var rootNamespace = (string)asmdef!["rootNamespace"];
        var includePlatforms = asmdef!["includePlatforms"];
        var excludePlatforms = asmdef!["excludePlatforms"];

        var assemblyNameWithoutExtension = assemblyName[..^7];

        Validate(ref isValid, !name.Equals(assemblyNameWithoutExtension), $"Incorrect assembly definition naming: Is [{name}] Expected [{assemblyNameWithoutExtension}]");
        Validate(ref isValid, !rootNamespace.Equals("MegaPint"), "RootNameSpace of assembly definition file is not set to \"MegaPint\"");

        Validate(ref isValid, !includePlatforms.ToString().Equals("[]"), "Assembly definition file includes at least one platform");
        Validate(ref isValid, !excludePlatforms.ToString().Equals("[]"), "Assembly definition file excludes at least one platform");
    }

    private static void ValidateAsmref(ref bool isValid, string path, string assemblyName)
    {
        var asmrefPath = Path.Combine(path, assemblyName);
        dynamic asmref = JsonConvert.DeserializeObject(File.ReadAllText(asmrefPath));

        var pointer = (string)asmref!["reference"];
        var assemblyNameWithoutExtension = assemblyName[..^7];
            
        var validation = !pointer.Equals(assemblyNameWithoutExtension);

        Validate(ref isValid, validation, $"The assembly reference file is not pointing to the correct assembly definition file: Pointing to [{pointer}] expected [{assemblyNameWithoutExtension}]");

    }
}
}
#endif
