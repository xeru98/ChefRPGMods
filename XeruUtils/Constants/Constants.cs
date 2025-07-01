using System.Reflection;

namespace XeruUtils;

public static class Constants
{
    /// <summary>
    /// These are the reflection fields necessary for retrieving a private instance field.
    /// </summary>
    public static readonly BindingFlags PRIVATE_INSTANCE_FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;
    
}