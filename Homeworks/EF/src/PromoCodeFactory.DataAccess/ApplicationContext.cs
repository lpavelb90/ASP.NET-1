using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.DataAccess
{
    public class ApplicationContext: DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PromoCode>()
                .HasOne(pc => pc.Preference)
                .WithOne(pr => pr.PromoCode)
                .HasForeignKey<PromoCode>(pc => pc.PreferenceId);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role)
                .WithOne(r => r.Employee)
                .HasForeignKey<Employee>(e => e.RoleId);

            //EF Core сам создаст CustomerPreferences
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Preferences)
                .WithMany();

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.PromoCodes)
                .WithOne();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public void EnsureDBCreatedWithData<T>(IEnumerable<BaseEntity<Guid>> data, bool ensureDeleted = false) where T: BaseEntity<Guid>
        {
            if (ensureDeleted)
                Database.EnsureDeleted();

            Database.EnsureCreated();

            var dbSet = Set<T>();

            if (!dbSet.Any()) // добавляем если в таблице нет данных
                dbSet.AddRange(data.Cast<T>());
            else
            {
                // исключаем из добавления записи, которые уже есть в БД
                var ids = data.Select(d => d.Id);
                var existsInDbIds = dbSet.Where(s => ids.Contains(s.Id)).Select(x => x.Id).ToList();

                data = data.Except(data.Where(d => existsInDbIds.Contains(d.Id)));

                dbSet.AddRange(data.Cast<T>());
            }

            SaveChanges();
        }
    }
}
