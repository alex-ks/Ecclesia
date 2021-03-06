using Newtonsoft.Json;

namespace Ecclesia.Models
{
    public class MnemonicsValue
	{
		[JsonRequired]
		public string Value { get; set; }

		[JsonRequired]
		public DataType DataType { get; set; }
	}
}