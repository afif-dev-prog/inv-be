using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_v2.Model;
using Microsoft.EntityFrameworkCore;

namespace inventory_v2.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Assets> Assets { get; set; }
        public DbSet<ChildAsset> ChildAsset { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Types> Types { get; set; }
        public DbSet<Movement> Movement { get; set; }
        public DbSet<MovementCart> MovementCart { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<UserLog> UserLog { get; set; }
        public DbSet<Activity> Activity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assets>()
                .HasMany(a => a.ChildAsset)
                .WithOne()
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}