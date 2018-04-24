using Newtonsoft.Json;

public class AtomIdArgument
{
    [JsonRequired]
    public string Kind { get; set; }
    [JsonRequired]
    public string Name { get; set; }
    public string Version { get; set; }
}