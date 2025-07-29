using Microsoft.EntityFrameworkCore;
using System;

namespace API.Data
{
    public class ModuleDiemDanhDbContext : DbContext
    {
        public ModuleDiemDanhDbContext()
        {

        }
        public ModuleDiemDanhDbContext(DbContextOptions<ModuleDiemDanhDbContext> options) : base(options)
        {
        }

        public DbSet<HocKy> HocKys { get; set; }
        public DbSet<SinhVien> SinhViens { get; set; }
        public DbSet<DiemDanh> DiemDanhs { get; set; }
        public DbSet<NhomXuong> NhomXuongs { get; set; }
        public DbSet<KeHoach> KeHoachs { get; set; }
        public DbSet<KeHoachNhomXuong> KeHoachNhomXuongs { get; set; }
        public DbSet<KHNXCaHoc> KHNXCaHocs { get; set; }
        public DbSet<CaHoc> CaHocs { get; set; }
        public DbSet<LichHoc> LichHocs { get; set; }
        public DbSet<LichGiangDay> LichGiangDays { get; set; }
        public DbSet<DuAn> DuAns { get; set; }
        public DbSet<CoSo> CoSos { get; set; }
        public DbSet<BoMonCoSo> BoMonCoSos { get; set; }
        public DbSet<BanDaoTao> BanDaoTaos { get; set; }
        public DbSet<QuanLyBoMon> QuanLyBoMons { get; set; }
        public DbSet<PhuTrachXuong> PhuTrachXuongs { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<VaiTroNhanVien> VaiTroNhanViens { get; set; }
        public DbSet<CapDoDuAn> CapDoDuAns { get; set; }
        public DbSet<DiaDiem> DiaDiems { get; set; }
        public DbSet<IP> IPs { get; set; }
        public DbSet<LichSuDiemDanh> LichSuDiemDanhs { get; set; }
        public DbSet<ImportHistory> ImportHistory { get; set; } // Add this line

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints here
            modelBuilder.Entity<DiemDanh>()
                .HasOne(d => d.SinhVien)
                .WithMany(s => s.DiemDanhs)
                .HasForeignKey(d => d.IdSinhVien)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiemDanh>()
                .HasOne(d => d.CaHoc)
                .WithMany(c => c.DiemDanhs)
                .HasForeignKey(d => d.IdCaHoc)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DiemDanh>()
                .HasOne(d => d.NhomXuong)
                .WithMany(n => n.DiemDanhs)
                .HasForeignKey(d => d.IdNhomXuong)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DiemDanh>()
                .HasOne(d => d.PhuTrachXuong)
                .WithMany(p => p.DiemDanhs)
                .HasForeignKey(d => d.IdNhanVien)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KeHoachNhomXuong>()
                .HasOne(k => k.KeHoach)
                .WithMany(k => k.KeHoachNhomXuongs)
                .HasForeignKey(k => k.IdKeHoach)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KeHoachNhomXuong>()
                .HasOne(k => k.NhomXuong)
                .WithMany(n => n.KeHoachNhomXuongs)
                .HasForeignKey(k => k.IdNhomXuong)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KHNXCaHoc>()
                .HasOne(k => k.KeHoachNhomXuong)
                .WithMany(k => k.KHNXCaHocs)
                .HasForeignKey(k => k.IdKHNX);

            modelBuilder.Entity<KHNXCaHoc>()
                .HasOne(k => k.CaHoc)
                .WithMany(c => c.KHNXCaHocs)
                .HasForeignKey(k => k.IdCaHoc);

            modelBuilder.Entity<LichGiangDay>()
                .HasOne(l => l.KHNXCaHoc)
                .WithMany(k => k.LichGiangDays)
                .HasForeignKey(l => l.IdNXCH)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LichGiangDay>()
                .HasOne(l => l.DiaDiem)
                .WithMany(d => d.LichGiangDays)
                .HasForeignKey(l => l.IdDiaDiem)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LichGiangDay>()
                .HasOne(l => l.NhomXuong)
                .WithMany(n => n.LichGiangDays)
                .HasForeignKey(l => l.IdNhomXuong)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LichGiangDay>()
                .HasOne(l => l.DuAn)
                .WithMany(d => d.LichGiangDays)
                .HasForeignKey(l => l.IdDuAn);

            modelBuilder.Entity<PhuTrachXuong>()
                .HasOne(p => p.CoSo)
                .WithMany(c => c.PhuTrachXuongs)
                .HasForeignKey(p => p.IdCoSo);

            modelBuilder.Entity<NhomXuong>()
                .HasOne(n => n.PhuTrachXuong)
                .WithMany(p => p.NhomXuongs)
                .HasForeignKey(n => n.IdPhuTrachXuong);

            modelBuilder.Entity<QuanLyBoMon>()
                .HasMany(q => q.NhomXuongs)
                .WithOne(n => n.QuanLyBoMon)
                .HasForeignKey(n => n.IdBoMon);

            modelBuilder.Entity<QuanLyBoMon>()
                .HasMany(q => q.DuAns)
                .WithOne(d => d.QuanLyBoMon)
                .HasForeignKey(d => d.IdBoMon);

            modelBuilder.Entity<QuanLyBoMon>()
                .HasMany(q => q.BoMonCoSos)
                .WithOne(b => b.QuanLyBoMon)
                .HasForeignKey(b => b.IdBoMon);

            // Thêm cấu hình cho BoMonCoSo và CoSo
            modelBuilder.Entity<BoMonCoSo>()
                .HasOne(b => b.CoSo)
                .WithMany(c => c.BoMonCoSos)
                .HasForeignKey(b => b.IdCoSo)
                .IsRequired(false); // Cho phép IdCoSo là nullable

            modelBuilder.Entity<VaiTroNhanVien>()
                .HasOne(v => v.VaiTro)
                .WithMany(v => v.VaiTroNhanViens)
                .HasForeignKey(v => v.IdVaiTro);

            modelBuilder.Entity<LichSuDiemDanh>()
                .HasOne(l => l.DiemDanh)
                .WithMany(d => d.LichSuDiemDanhs)
                .HasForeignKey(l => l.IdDiemDanh);

            modelBuilder.Entity<DuAn>()
                .HasOne(d => d.HocKy)
                .WithMany(h => h.DuAns)
                .HasForeignKey(d => d.IdHocKy);

            modelBuilder.Entity<LichHoc>()
                .HasOne(l => l.KHNXCaHoc)
                .WithMany(k => k.LichHocs)
                .HasForeignKey(l => l.IdNXCH)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LichHoc>()
                .HasOne(l => l.DuAn)
                .WithMany(d => d.LichHocs)
                .HasForeignKey(l => l.IDHocKy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LichHoc>()
                .HasOne(l => l.NhomXuong)
                .WithMany(n => n.LichHocs)
                .HasForeignKey(l => l.IdNhomXuong)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LichSuDiemDanh>()
                .HasOne(l => l.DiemDanh)
                .WithMany(d => d.LichSuDiemDanhs)
                .HasForeignKey(l => l.IdDiemDanh)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LichSuDiemDanh>()
                .HasOne(lsdd => lsdd.KHNXCaHoc)
                .WithMany(kh => kh.LichSuDiemDanhs)
                .HasForeignKey(lsdd => lsdd.IdNXCH)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CoSo>()
                .Property(c => c.IdCaHoc)
                .IsRequired(false);

            modelBuilder.Entity<VaiTroNhanVien>()
                .HasOne(v => v.PhuTrachXuong)
                .WithMany(p => p.VaiTroNhanViens)
                .HasForeignKey(v => v.IdNhanVien)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VaiTroNhanVien>()
                .HasOne(v => v.VaiTro)
                .WithMany(vt => vt.VaiTroNhanViens)
                .HasForeignKey(v => v.IdVaiTro)
                .OnDelete(DeleteBehavior.Restrict);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=HOANG\\SQLEXPRESS;Database=nhom1ty;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }
    }
}