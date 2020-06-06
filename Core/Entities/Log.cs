using System;

namespace Core.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Event { get; set; }
        public DateTime EventDate { get; set; }
    }
}