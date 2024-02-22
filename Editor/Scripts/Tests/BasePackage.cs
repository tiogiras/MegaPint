using NUnit.Framework;
using UnityEditor.PackageManager;

namespace Editor.Scripts.Tests
{

internal class BasePackage
{
    private PackageInfo _basePackage;

    #region Tests

    [Test]
    public void PrivateName()
    {
        Assert.IsNotEmpty(GetBasePackage().name);
    }
    
    [Test]
    public void DisplayName()
    {
        Assert.IsNotEmpty(GetBasePackage().displayName);
    }
    
    [Test]
    public void Author()
    {
        Assert.NotNull(GetBasePackage().author);
    }
    
    [Test]
    public void Version()
    {
        Assert.IsNotEmpty(GetBasePackage().version);
    }
    
    [Test]
    public void Category()
    {
        Assert.IsNotEmpty(GetBasePackage().category);
    }
    
    [Test]
    public void Description()
    {
        Assert.IsNotEmpty(GetBasePackage().description);
    }
    
    [Test]
    public void Repository()
    {
        Assert.IsNotNull(GetBasePackage().repository);
        Assert.IsNotEmpty(GetBasePackage().repository.url);
    }

    #endregion

    #region Private Methods

    private PackageInfo GetBasePackage()
    {
        _basePackage ??= Scripts.PackageManager.Tests.BasePackage();

        return _basePackage;
    }

    #endregion
}

}
