using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FaceAuth.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<UserPic>()
            .Property(p => p.Picture)
            .HasColumnType("MEDIUMBLOB");
        }

        //-------------------------------------------------------------------------------------

        #region User
        public DbSet<User> User { get; set; }
        public DbSet<UserPic> UserPics { get; set; }
        #endregion User
    }
}
