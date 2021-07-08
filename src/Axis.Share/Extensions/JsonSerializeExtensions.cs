using Newtonsoft.Json;

namespace Axis.Share.Extensions
{
    public static class JsonSerializeExtensions
    {
        public static string ToJson<T>(this T obj, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(obj, formatting);
        }
    }
}
