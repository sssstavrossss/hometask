using Newtonsoft.Json;
namespace HometaskLib.Models.Fixer;

public class FixerResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }
        
    [JsonProperty("base")]
    public string Base { get; set; }
        
    [JsonProperty("date")]
    public string Date { get; set; }
        
    [JsonProperty("rates")]
    public Dictionary<string, decimal> Rates { get; set; }
}