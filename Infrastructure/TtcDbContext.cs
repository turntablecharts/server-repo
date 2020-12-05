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
        public DbSet<NewsItem> NewsItems {get; set;}
        public DbSet<PhotoItem> PhotoItems {get; set;}
        public DbSet<VideoItem> VideoItems {get; set;}
        public DbSet<MediaItem> MediaItems {get; set;}
        public DbSet<Chart> Charts {get; set;}
        public DbSet<Log> Logs {get; set;}
        public DbSet<SubscribersEmail> SubscribersEmails { get; set; }
        public DbSet<MagazineItem> MagazineItems { get; set; }







        /// new datas
        public DbSet<MagazineData> MagazineDatas { get; set; }
        public DbSet<MagazineEditionData> MagazineEditionDatas { get; set; }
    }
}