using System.Diagnostics.CodeAnalysis;
using CargoHub.Models;

[ExcludeFromCodeCoverage]
public static class FieldCopier
{
    public static void CopyFields<T>(T source, T target)
    {
        foreach (var property in typeof(T).GetProperties())
        {
            if (property.CanWrite)
            {
                var sourceValue = property.GetValue(source);
                var targetValue = property.GetValue(target);

                // Skip if the value hasn't changed
                if (!Equals(sourceValue, targetValue))
                {
                    property.SetValue(target, sourceValue);
                }
            }
        }
    }
}

