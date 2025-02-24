
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Styra.Ucast.Linq;

public struct MaskResult
{
    [JsonProperty("masks")]
    public Dictionary<string, MaskingFunc>? Masks;

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public struct MaskingFunc
{
    [JsonProperty("replace", NullValueHandling = NullValueHandling.Ignore)]
    public ReplaceFunc? Replace;

    public struct ReplaceFunc
    {
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public object? Value;
    }

    public override readonly string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}