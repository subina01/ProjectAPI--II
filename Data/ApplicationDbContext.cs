using Carrental.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Carrental.WebAPI.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }
        public DbSet<VehicleCategory> VehicleCategories { get; set; }
        public DbSet<VehicleBrand> VehicleBrands { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<VehicleImage> VehicleImages { get; set; }
        public DbSet<BookingConfirmation> BookingConfirmations { get; set; }
        public DbSet<ReturnConfirmation> ReturnConfirmations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VehicleBrand>().HasKey(vb => vb.BrandId);
            modelBuilder.Entity<VehicleCategory>().HasKey(vc => vc.CategoryId);
            modelBuilder.Entity<VehicleModel>().HasKey(vm => vm.ModelId);
            modelBuilder.Entity<Vehicle>().HasKey(v => v.VehicleId);
            modelBuilder.Entity<Booking>().HasKey(b => b.Id);
            modelBuilder.Entity<Return>().HasKey(rc => rc.Id);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Model)
                .WithMany(vm => vm.Vehicles)
                .HasForeignKey(v => v.ModelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Brand)
                .WithMany(vb => vb.Vehicles)
                .HasForeignKey(v => v.BrandId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Category)
                .WithMany(vc => vc.Vehicles)
                .HasForeignKey(v => v.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Vehicle)
                .WithMany()
                .HasForeignKey(b => b.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.VehicleImages)
                .WithOne(i => i.Vehicle)
                .HasForeignKey(i => i.VehicleId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.BookingConfirmation)
                .WithOne(bc => bc.Booking)
                .HasForeignKey<BookingConfirmation>(bc => bc.BookingId);

            modelBuilder.Entity<BookingConfirmation>(entity =>
            {
                entity.Property(e => e.DiscountAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.TotalBeforeDiscount)
                    .HasColumnType("decimal(18,2)");
            });
            modelBuilder.Entity<ReturnConfirmation>(entity =>
            {
               

                entity.Property(rc => rc.DamageFee)
                    .HasColumnType("decimal(18,2)");

                entity.Property(rc => rc.TotalBeforeFees)
                    .HasColumnType("decimal(18,2)");

                entity.Property(rc => rc.TotalAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(rc => rc.TotalLateFees)
                    .HasColumnType("decimal(18,2)");
            });



            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<VehicleBrand>()
                .Property(vb => vb.RentalCharge)
                .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<VehicleBrand>().HasData(
      new VehicleBrand { BrandId = 1, VehicleBrandName = "Hyundai", RentalCharge = 6000 },
      new VehicleBrand { BrandId = 2, VehicleBrandName = "Suzuki", RentalCharge = 5100 },
      new VehicleBrand { BrandId = 3, VehicleBrandName = "Toyota", RentalCharge = 7500 },
      new VehicleBrand { BrandId = 4, VehicleBrandName = "Honda", RentalCharge = 7000 },
      new VehicleBrand { BrandId = 5, VehicleBrandName = "Tata Motors", RentalCharge = 5500 },
      new VehicleBrand { BrandId = 6, VehicleBrandName = "Ashok Leyland", RentalCharge = 4000 },
      new VehicleBrand { BrandId = 7, VehicleBrandName = "Mahindra", RentalCharge = 4800 },
      new VehicleBrand { BrandId = 8, VehicleBrandName = "Eicher", RentalCharge = 5000 }
  );

            modelBuilder.Entity<VehicleCategory>().HasData(
                new VehicleCategory { CategoryId = 1, VehicleCategoryName = "Car" },
                new VehicleCategory { CategoryId = 2, VehicleCategoryName = "Bus" },
                new VehicleCategory { CategoryId = 3, VehicleCategoryName = "Sumo" },
                new VehicleCategory { CategoryId = 4, VehicleCategoryName = "Truck" },
                new VehicleCategory { CategoryId = 5, VehicleCategoryName = "Minivan" },
                new VehicleCategory { CategoryId = 6, VehicleCategoryName = "Jeep" },
                new VehicleCategory { CategoryId = 7, VehicleCategoryName = "Microbus" },
                new VehicleCategory { CategoryId = 8, VehicleCategoryName = "Tempo" },
                new VehicleCategory { CategoryId = 9, VehicleCategoryName = "Van" },
                new VehicleCategory { CategoryId = 10, VehicleCategoryName = "Scooter" },
                new VehicleCategory { CategoryId = 11, VehicleCategoryName = "Cycle" },
                new VehicleCategory { CategoryId = 12, VehicleCategoryName = "Bike" },
                new VehicleCategory { CategoryId = 13, VehicleCategoryName = "Scorpio" }
            );

            modelBuilder.Entity<VehicleModel>().HasData(
                new VehicleModel { ModelId = 1, VehicleModelName = "i10" },
                new VehicleModel { ModelId = 2, VehicleModelName = "i20" },
                new VehicleModel { ModelId = 3, VehicleModelName = "Creta" },
                new VehicleModel { ModelId = 4, VehicleModelName = "Santro" },
                new VehicleModel { ModelId = 5, VehicleModelName = "Alto" },
                new VehicleModel { ModelId = 6, VehicleModelName = "Swift" },
                new VehicleModel { ModelId = 7, VehicleModelName = "WagonR" },
                new VehicleModel { ModelId = 8, VehicleModelName = "Celerio" },
                new VehicleModel { ModelId = 9, VehicleModelName = "Corolla" },
                new VehicleModel { ModelId = 10, VehicleModelName = "Yaris" },
                new VehicleModel { ModelId = 11, VehicleModelName = "Vitz" },
                new VehicleModel { ModelId = 12, VehicleModelName = "City" },
                new VehicleModel { ModelId = 13, VehicleModelName = "Amaze" },
                new VehicleModel { ModelId = 14, VehicleModelName = "Jazz" },
                new VehicleModel { ModelId = 15, VehicleModelName = "Tiago" },
                new VehicleModel { ModelId = 16, VehicleModelName = "Nexon" },
                new VehicleModel { ModelId = 17, VehicleModelName = "Tigor" },
                new VehicleModel { ModelId = 18, VehicleModelName = "LP 407" },
                new VehicleModel { ModelId = 19, VehicleModelName = "LP 1512" },
                new VehicleModel { ModelId = 20, VehicleModelName = "Viking" },
                new VehicleModel { ModelId = 21, VehicleModelName = "Cheetah" },
                new VehicleModel { ModelId = 22, VehicleModelName = "Cruzio" },
                new VehicleModel { ModelId = 23, VehicleModelName = "Supro Bus" },
                new VehicleModel { ModelId = 24, VehicleModelName = "Skyline" },
                new VehicleModel { ModelId = 25, VehicleModelName = "Starline" },
                new VehicleModel { ModelId = 26, VehicleModelName = "Sumo Gold" },
                new VehicleModel { ModelId = 27, VehicleModelName = "Sumo Victa" },
                new VehicleModel { ModelId = 28, VehicleModelName = "Bolero" },
                new VehicleModel { ModelId = 29, VehicleModelName = "Scorpio" },
                new VehicleModel { ModelId = 30, VehicleModelName = "Blazo" },
                new VehicleModel { ModelId = 31, VehicleModelName = "Furio" },
                new VehicleModel { ModelId = 32, VehicleModelName = "Pro 3015" },
                new VehicleModel { ModelId = 33, VehicleModelName = "Pro 2049" },
                new VehicleModel { ModelId = 34, VehicleModelName = "Dost+" },
                new VehicleModel { ModelId = 35, VehicleModelName = "2516 IL" },
                new VehicleModel { ModelId = 36, VehicleModelName = "Eeco" },
                new VehicleModel { ModelId = 37, VehicleModelName = "Omni" },
                new VehicleModel { ModelId = 38, VehicleModelName = "Venture" },
                new VehicleModel { ModelId = 39, VehicleModelName = "Winger" },
                new VehicleModel { ModelId = 40, VehicleModelName = "Supro Van" },
                new VehicleModel { ModelId = 41, VehicleModelName = "Bolero Camper" }
            );








            base.OnModelCreating(modelBuilder);
        }

        public void ReseedAllTables()
        {
            try
            {
                ReseedTable("Users", Users.Any() ? Users.Max(u => u.Id) : 0);
                ReseedTable("VehicleModels", VehicleModels.Any() ? VehicleModels.Max(vm => vm.ModelId) : 0);
                ReseedTable("VehicleCategories", VehicleCategories.Any() ? VehicleCategories.Max(vc => vc.CategoryId) : 0);
                ReseedTable("VehicleBrands", VehicleBrands.Any() ? VehicleBrands.Max(vb => vb.BrandId) : 0);
                ReseedTable("Returns", Returns.Any() ? Returns.Max(r => r.Id) : 0);
                ReseedTable("Vehicles", Vehicles.Any() ? Vehicles.Max(v => v.VehicleId) : 0);
                ReseedTable("Bookings", Bookings.Any() ? Bookings.Max(b => b.Id) : 0);
                ReseedTable("VehicleImages", VehicleImages.Any() ? VehicleImages.Max(vi => vi.Id) : 0);
                ReseedTable("BookingConfirmations", BookingConfirmations.Any() ? BookingConfirmations.Max(bc => bc.Id) : 0);
                ReseedTable("ReturnConfirmations", ReturnConfirmations.Any() ? ReturnConfirmations.Max(rc => rc.Id) : 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reseeding tables: {ex.Message}");
            }
        }

        private void ReseedTable(string tableName, int maxId)
        {
            if (maxId > 0)
            {
                Database.ExecuteSqlRaw($"DBCC CHECKIDENT ('{tableName}', RESEED, {maxId})");
            }
        }
    }
}
