#if UNITY_EDITOR
using UnityEngine;

namespace Editor.Scripts.Tests
{

/// <summary> Utility class for unit testing </summary>
internal static class TestsUtility
{
    #region Public Methods

    /// <summary> Validate a given condition and log if the condition is false </summary>
    /// <param name="valid"> Set to false if the condition is not met </param>
    /// <param name="condition"> Condition to be checked </param>
    /// <param name="log"> Message to be logged if the condition is not met </param>
    /// <returns> If the condition was met </returns>
    public static bool Validate(ref bool valid, bool condition, string log = "")
    {
        if (!condition)
            return false;

        valid = false;
        Debug.Log(log);

        return true;
    }

    #endregion
}

}
#endif
