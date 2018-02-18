using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ecclesia.LocalExecutor.Endpoint.Models
{
	public class ComputationGraph
	{
		[JsonRequired]
		public int[][] Dependecies { get; set; }
		
		[JsonRequired]
		public List<Operation> Operations { get; set; }

		[JsonRequired]
		public Dictionary<string, MnemonicsValue> MnemonicsTable { get; set; }
	}
}
