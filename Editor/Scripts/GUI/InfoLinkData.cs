using System.Collections.Generic;

namespace Editor.Scripts.GUI
{

public static class InfoLinkData
{
    private static Dictionary <string, string> s_data = new()
    {
        {"notePad","NotePad is a class that can be added to a MonoBehaviour to hold notes and display them in the inspector.\n\n<b>Assembly:</b>tiogiras.megapint.notepad.runtime"},
        {"gameObject","GameObjects are the fundamentals of every object in your game."},
        {"monoBehaviour","MonoBehaviours are scripts that inherit from the base class MonoBehaviour.\n\nOnly MonoBehaviours can be added to GameObjects."},
        // {"",""},
    };

    public static string Get(string infoKey)
    {
        return !s_data.ContainsKey(infoKey) ? $"No info found for [{infoKey}]" : s_data[infoKey];
    }
}

}
