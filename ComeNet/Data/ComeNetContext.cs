using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ComeNet.Models;

namespace ComeNet.Data
{
    public class ComeNetContext : DbContext
    {
        public ComeNetContext (DbContextOptions<ComeNetContext> options)
            : base(options)
        {
        }

        public DbSet<ComeNet.Models.User> User { get; set; } = default!;
    }
}
