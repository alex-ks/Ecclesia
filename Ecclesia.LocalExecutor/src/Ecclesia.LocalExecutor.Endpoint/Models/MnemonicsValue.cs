using Newtonsoft.Json;

namespace Ecclesia.LocalExecutor.Endpoint.Models
{
    public class MnemonicsValue
	{
        [JsonRequired]
		public string Value { get; set; }
        
		public DataType Type { get; set; }
	}
}