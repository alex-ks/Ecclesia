using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ecclesia.LocalExecutor.Endpoint.Models
{
	public class Operation
	{
		[JsonRequired]
		public int Id { get; set; }
		
		[JsonRequired]
		public string Name { get; set; }

		[JsonRequired]
		public string[] Input { get; set; }

		[JsonRequired]
		public string[] Output { get; set; }
		
		public List<DataType> Parameters { get; set; }
	}
}
