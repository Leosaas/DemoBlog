using DTO;
using Microsoft.EntityFrameworkCore;
namespace DAO
{
    public class DemoDbContext : DbContext
    {
        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options) { }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<TinTuc> TinTucs { get; set; }
        public DbSet<DanhMucTinTuc> DanhMucTinTucs { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<DanhMucTinTuc>().HasKey(dt => new { dt.IDDanhMuc, dt.IDTintuc });
        //    modelBuilder.Entity<DanhMucTinTuc>()
        //        .HasOne<DanhMuc>(sc => sc.DanhMuc)
        //        .WithMany(s => s.DanhMucTinTucs)
        //        .HasForeignKey(sc => sc.IDDanhMuc);


        //    modelBuilder.Entity<DanhMucTinTuc>()
        //        .HasOne<TinTuc>(sc => sc.TinTuc)
        //        .WithMany(s => s.DanhMucTinTucs)
        //        .HasForeignKey(sc => sc.IDTintuc);
        //}
    }
}