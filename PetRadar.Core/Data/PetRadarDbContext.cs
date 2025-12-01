using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data.Entities;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>()
                .Property(x => x.Role)
                .HasConversion<string>();

            var salt = UserDomain.GenerateSalt();
            var hashPassword = UserDomain.GenerateHash("test", salt);

            modelBuilder.Entity<UserEntity>()
                .HasData(new UserEntity("sa@test.com",hashPassword,salt, "Super", "Admmin", "000000000", null,null,null, RoleEnum.SuperAdmin, 1) 
                    { 
                        Id = 1,
                        EmailVerified = true,
                        IsActive = true,
                    }
                );
        }
    }
}
