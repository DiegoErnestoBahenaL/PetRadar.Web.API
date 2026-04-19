using Microsoft.EntityFrameworkCore;
using PetRadar.Common;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data
{
    public class PetRadarDbContext : DbContext
    {
        public PetRadarDbContext(DbContextOptions<PetRadarDbContext> options) : base (options) { }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserPetEntity> UserPets { get; set; }
        public DbSet<VeterinaryAppointmentEntity> VeterinaryAppointments { get; set; }
        public DbSet<AdoptionAnimalEntity> AdoptionAnimals { get; set; }
        public DbSet<ReportEntity> Reports { get; set; }
        public DbSet<MatchEntity> Matches { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<NotificationEntity> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.Entity<UserEntity>()
                .Property(x => x.Role)
                .HasConversion<string>();

            modelBuilder.Entity<UserPetEntity>()
                .Property(x => x.Sex)
                .HasConversion<string>();

            modelBuilder.Entity<UserPetEntity>()
                .Property(x => x.Species)
                .HasConversion<string>();

            modelBuilder.Entity<UserPetEntity>()
                .Property(x => x.Size)
                .HasConversion<string>();

            modelBuilder.Entity<VeterinaryAppointmentEntity>()
                .Property(x => x.AppointmentType)
                .HasConversion<string>();

            modelBuilder.Entity<VeterinaryAppointmentEntity>()
                .Property(x => x.AppointmentStatus)
                .HasConversion<string>();

            modelBuilder.Entity<VeterinaryAppointmentEntity>()
                .Property(x => x.Location)
                .HasColumnType("geography (point)");

            modelBuilder.Entity<AdoptionAnimalEntity>()
                .Property(x => x.Sex)
                .HasConversion<string>();

            modelBuilder.Entity<AdoptionAnimalEntity>()
                .Property(x => x.Species)
                .HasConversion<string>();

            modelBuilder.Entity<AdoptionAnimalEntity>()
                .Property(x => x.Size)
                .HasConversion<string>();

            modelBuilder.Entity<AdoptionAnimalEntity>()
                .Property(x => x.Status)
                .HasConversion<string>();

            modelBuilder.Entity<ReportEntity>()
                .Property(x => x.Location)
                .HasColumnType("geography (point)");

            modelBuilder.Entity<ReportEntity>()
                .Property(x => x.ReportType)
                .HasConversion<string>();

            modelBuilder.Entity<ReportEntity>()
                .Property(x => x.ReportStatus)
                .HasConversion<string>();

            modelBuilder.Entity<ReportEntity>()
                .Property(x => x.Sex)
                .HasConversion<string>();

            modelBuilder.Entity<ReportEntity>()
                .Property(x => x.Species)
                .HasConversion<string>();

            modelBuilder.Entity<ReportEntity>()
                .Property(x => x.Size)
                .HasConversion<string>();

            modelBuilder.Entity<ReportEntity>()
                .OwnsOne(x => x.ImageAnalysisResult, y => {
                    y.ToJson();
                    y.OwnsMany(y => y.TopPredictions);
                    y.OwnsMany(y => y.Colors);
                });

            modelBuilder.Entity<MatchEntity>()
                .Property(x => x.Status)
                .HasConversion<string>();

            modelBuilder.Entity<NotificationEntity>()
                .Property(x => x.Type)
                .HasConversion<string>();

            modelBuilder.Entity<UserEntity>()
                .HasData(new UserEntity("sa@petradar.com",Array.Empty<byte>(),Array.Empty<byte>(), "Super", "Admin", "000000000", null,null,null, RoleEnum.SuperAdmin, Constants.SuperAdminId) 
                    { 
                        Id = Constants.SuperAdminId,
                        EmailVerified = true,
                        IsActive = true,
                        CreatedAt = Constants.SuperAdminCreatedAt,
                        UpdatedAt = Constants.SuperAdminCreatedAt
                    }
                );
        }
    }
}
