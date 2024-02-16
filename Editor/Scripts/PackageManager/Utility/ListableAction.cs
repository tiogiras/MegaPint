#if UNITY_EDITOR
using System;

namespace Editor.Scripts.PackageManager.Utility
{

internal class ListableAction
{
    public readonly string name;
    private readonly Action _action;

    public ListableAction(Action action, string name)
    {
        _action = action;
        this.name = name;
    }

    #region Public Methods

    public void Invoke()
    {
        _action.Invoke();
    }

    #endregion
}

internal class ListableAction <T>
{
    public readonly string name;
    private readonly Action <T> _action;

    public ListableAction(Action <T> action, string name)
    {
        _action = action;
        this.name = name;
    }

    #region Public Methods

    public void Invoke(T value)
    {
        _action.Invoke(value);
    }

    #endregion
}

}
#endif
