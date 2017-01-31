using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using App.DataContext;

namespace aspnetcoreapi.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20170131181226_name=User")]
    partial class nameUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("App.Models.User", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("email")
                        .IsRequired();

                    b.Property<string>("opt_enabled");

                    b.Property<string>("opt_secret");

                    b.Property<string>("password")
                        .IsRequired();

                    b.Property<DateTime>("updated_at");
                    
                    b.Property<DateTime>("created_at");

                    b.Property<int>("verified");

                    b.HasKey("id");

                    b.HasIndex("email")
                        .IsUnique();

                    b.ToTable("User");
                });
        }
    }
}
