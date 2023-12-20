namespace Presentation.DTO
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName {get; set;}
        public string Bio { get; set; }
        public string Token {get; set;} 
        public string Role {get; set;}
        public string Email { get; set; }
    }
}