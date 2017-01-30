using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace App.DataContext
{
    using App.Models;
    
    public class DatabaseContext : DbContext
    {
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(b => b.email)
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=app.db");
        }
    }
}