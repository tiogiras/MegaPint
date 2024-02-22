using UnityEngine;

namespace Editor.Scripts.Tests
{

public static class TestsUtility
{
    public static void Validate(ref bool valid, bool condition, string log = "")
    {
        if (!condition)
            return;

        valid = false;
        Debug.Log(log);
    }
}

}
