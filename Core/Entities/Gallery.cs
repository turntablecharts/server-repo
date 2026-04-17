using static Common.AppEnums;

namespace Core.Entities;

public class Gallery
{
    public int Id { get; set; }
    public string Link { get; set; }
    public string Title { get; set; }
    public GalleryTypeEnum GalleryType { get; set; }
    public string Description { get; set; }
}
