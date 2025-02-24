
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Styra.Ucast.Linq;

/// <summary>
/// The JSON wrapper object for column masking responses.
/// </summary>
public struct MaskResult
{
    [JsonProperty("masks")]
    public Dictionary<string, MaskingFunc>? Masks;

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

/// <summary>
/// JSON object describing which column masking function to use, along with any
/// relevant parameter values for it to use.
/// </summary>
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
