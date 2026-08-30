using System;

namespace Presentation.DTO;

public class UnderThirtyResponseDto
{
    public int Id { get; set; }
    public string Image { get; set; }
    public string Headerquote { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Role { get; set; }
    public string Bio { get; set; }
    public string Citation { get; set; }
    public string CitationAuthor { get; set; }
    public DateTime DateAdded { get; set; }
    public bool IsActive { get; set; }
}
