using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ecclesia.Models
{
    public class DataType
    {
        [JsonRequired]
        public string Name { get; set; }
        
        public List<DataType> Parameters { get; set; }
    }
}