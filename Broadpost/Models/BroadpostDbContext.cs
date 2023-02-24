using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broadpost.Models
{
    public class BroadpostDbContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=BroadCast;Trusted_Connection=Yes");
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<ChannelUser> ChannelUsers { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasMany(c => c.Channels)
                .WithOne(u => u.User)
                .IsRequired();

            builder.Entity<Channel>()
                .HasMany(p => p.Posts)
                .WithOne(c => c.Channel)
                .IsRequired();

            builder.Entity<User>()
                .HasMany(p => p.Posts)
                .WithOne(u => u.User)
                .IsRequired();

            builder.Entity<ChannelUser>()
                .HasKey(k => new { k.ChannelId, k.UserId });

            builder.Entity<Invitation>()
                .HasKey(k => new { k.ChannelId, k.ReceverUserId });



            base.OnModelCreating(builder);
        }
    }
}
