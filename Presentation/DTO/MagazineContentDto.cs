using System;
using Core.Entities;

namespace Presentation.DTO
{
    public class MagazineContentDto
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int TtcUserId { get; set; }
        public TtcUser TtcUser { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public string HeaderImage { get; set; }

        public int MagazineEditionId { get; set; }

        public int MagazineEditionDataId { get; set; }

        public int ArticlePosition { get; set; }

        public MagazineData  NextArticle {get; set;}
    }
}