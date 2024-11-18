using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CargoHub.HelperFuctions;

[ExcludeFromCodeCoverage]
public static class ObjectCopier
{
    public static T Clone<T>(T source)
    {
        var settings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Prevent self-referencing loop errors
        };
        var serialized = JsonConvert.SerializeObject(source, settings);
        return JsonConvert.DeserializeObject<T>(serialized, settings)!;
    }
}