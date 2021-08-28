using System.Collections.Generic;

namespace Core.Entities {
    public class TtcUser {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Bio {get; set;}
        
        public string Gender {get;set;}
    }
}