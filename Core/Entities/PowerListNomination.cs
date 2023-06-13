using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class PowerListNomination
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Name {get; set;}
        public string Field {get; set;}
        public string Achievement {get; set;}
        [JsonIgnore]
        public DateTime DateAdded {get; set;}
    }
}