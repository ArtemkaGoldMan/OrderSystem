﻿using Microsoft.EntityFrameworkCore;
using OrderSystem.Models;

namespace OrderSystem.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() { }
        public DbSet<Order> Orders { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                string projectDir = Path.GetFullPath(Path.Combine(baseDir, "..\\..\\.."));

                string dbPath = Path.Combine(projectDir, "orders.db");

                options.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(o => o.ProductName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Order>()
                .Property(o => o.Amount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.DeliveryAddress)
                .HasMaxLength(255);

            modelBuilder.Entity<Order>()
                .Property(o => o.Payment)
                .IsRequired();

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasDefaultValue(OrderStatus.New);

            modelBuilder.Entity<Order>()
                .Property(o => o.Customer)
                .IsRequired();
        }
    }
}
