using Newtonsoft.Json;

namespace HometaskLib.Models.Fixer;

public class FixerSymbolsResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("symbols")]
    public Dictionary<string, string> Symbols { get; set; }
}