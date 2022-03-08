using Microsoft.AspNet.Identity.EntityFramework;
using Project1640.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Project1640.EF
{
    public class CMSContext : IdentityDbContext<UserInfo>
    {
        public CMSContext() : base("OOO")
        {

        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Idea> Idea { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<React> React { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<FileUpload> File { get; set; }
    }
}