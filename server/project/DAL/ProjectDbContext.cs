using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.DAL
{  
    public class ProjectDbContext: DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options): base(options)
        {
        }
       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
            modelBuilder.Entity<User>(entity =>
            {
                // אימייל ייחודי
                entity.HasIndex(u => u.Email)
                      .IsUnique();

                // שמירת Role כמחרוזת
                entity.Property(u => u.Role)
                      .HasConversion<string>();
            });

            // Gift ↔ Card
            modelBuilder.Entity<Card>()
                .HasOne(c => c.Gift)
                .WithMany(g => g.Cards)
                .HasForeignKey(c => c.GiftId)
                .OnDelete(DeleteBehavior.Restrict);

            //Quantityמוצר בסל ייחודי, אם מוסיפים יתווסף ב
            modelBuilder.Entity<BasketItem>()
                .HasIndex(i => new { i.UserId, i.GiftId })
                .IsUnique();

            //א"א למחוק מתנה שהוגרלה, רק לאחר איפוס הגרלה תהיה אפשרות
            modelBuilder.Entity<Raffle>()
               .HasOne(r => r.Gift)
               .WithMany(g => g.Winners)
               .HasForeignKey(r => r.GiftId)
               .OnDelete(DeleteBehavior.Restrict);
       }
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Raffle> Raffles { get; set; }
    }
}