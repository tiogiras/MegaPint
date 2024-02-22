using UnityEngine;

namespace Editor.Scripts.Tests
{

public static class TestsUtility
{
    public static bool Validate(ref bool valid, bool condition, string log = "")
    {
        if (!condition)
            return false;

        valid = false;
        Debug.Log(log);

        return true;
    }
}

}
