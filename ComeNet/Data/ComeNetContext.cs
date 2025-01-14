﻿using System;
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
        public DbSet<ComeNet.Models.Friendlist> Friendlist { get; set; } = default!;
        public DbSet<ComeNet.Models.ActivityList> ActivityList { get; set; } = default!;
        public DbSet<ComeNet.Models.ActivityDetail> ActivityDetail { get; set; } = default!;
        public DbSet<ComeNet.Models.MessageContext> MessageContext { get; set; } = default!;
        public DbSet<ComeNet.Models.Rejectlist> Rejectlist { get; set; } = default!;
        public DbSet<ComeNet.Models.Toollist> Toollist { get; set; } = default!;
        public DbSet<ComeNet.Models.UserToollist> UserToollist { get; set; } = default!;
        public DbSet<ComeNet.Models.UserMessage> UserMessage { get; set; } = default!;
        public DbSet<ComeNet.Models.Notification> Notification { get; set; } = default!;
       


    }
}
