using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class TtcDbContext : DbContext
    {
        public TtcDbContext(DbContextOptions<TtcDbContext> options) : base(options){ }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {}

        public DbSet<TtcUser> TtcUsers {get; set;}
        public DbSet<ChartItem> ChartItems {get; set;}
  
        public DbSet<Chart> Charts {get; set;}
        public DbSet<Log> Logs {get; set;}
        public DbSet<SubscribersEmail> SubscribersEmails { get; set; }
    
        public DbSet<PowerListNomination> PowerListNominations {get; set;}

        /// new datas
        public DbSet<MagazineData> MagazineDatas { get; set; }
        public DbSet<MagazineEditionData> MagazineEditionDatas { get; set; }

        public DbSet<News> News { get; set; }
        public DbSet<NewsCategory>  NewsCategories { get; set; }


        public DbSet<Photo> Photos { get; set; }
        public DbSet<PhotoCategory> PhotoCategories { get; set; }


        public DbSet<ChartHighlight> ChartHighlights { get; set; }

        public DbSet<ChartCategory> ChartCategories {get; set;}


        public DbSet<CertifiedSong> CertifiedSongs {get; set;}
        public DbSet<Gallery> Galleries { get; set; }


        public DbSet<PowerlistCategory> PowerlistCategories { get; set; }
        
        public DbSet<PowerlistEdition> PowerlistEditions { get; set; }

        public DbSet<PowerlistRecognition> PowerlistRecognitions { get; set; }

        public DbSet<UnderThirty> UnderThirties { get; set; }

    }
}