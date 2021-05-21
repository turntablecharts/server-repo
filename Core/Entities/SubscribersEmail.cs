using System;
namespace Core.Entities
{
    public class SubscribersEmail
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime? SignUpDate { get; set; }
    }
}