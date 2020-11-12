using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LexiconLMS.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace LexiconLMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LexiconLMS.Models.Entities.AppUser> AppUser { get; set; }

        // TODO Has query filter?
        // TODO add morde DbSets entities    
    }
}
