using System.Reflection;

namespace XeruUtils;
public static class FieldHelpers
{
    /// <summary>
    /// This function is a quick helper to allow us to retrieve private instance variables from internal objects
    /// </summary>
    /// <param name="Instance">This is the instance of the object we are getting the field from</param>
    /// <param name="FieldName">This is the name of the field we are retrieving</param>
    /// <typeparam name="TF">Type of the field we are returning</typeparam>
    /// <typeparam name="TI">Type of the instance object</typeparam>
    /// <returns></returns>
    public static TF GetPrivateFieldValue<TF, TI>(TI Instance, string FieldName)
    {
        FieldInfo field = typeof(TI).GetField(FieldName, Constants.PRIVATE_INSTANCE_FLAGS);
        return (TF)field.GetValue(Instance);
    }
}