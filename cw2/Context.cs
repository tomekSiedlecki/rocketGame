using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace cw2
{
    class Context : DbContext
    {
        public DbSet<Record> Records { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=dumbo.db.elephantsql.com;Database=oacpxnmz;Username=oacpxnmz;Password=5JXK6zhzciEgflhNfZmz9k2xk7AuBvE-");

    }
}
