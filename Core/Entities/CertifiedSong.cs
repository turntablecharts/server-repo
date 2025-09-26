using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CertifiedSong
    {
        public long Id { get; set; }
        public string Milestone {get; set;}
        public string Title {get; set;}
        public string Artiste {get; set;}
        public string Format {get; set;}
        public string Label {get; set;}
        public DateTime? CertifiedDate {get; set;}
        public bool IsClaimed {get; set;}
    }
}