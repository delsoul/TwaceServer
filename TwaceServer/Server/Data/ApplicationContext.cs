using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwaceServer.Shared.Models;

namespace TwaceServer.Server.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Request> Requests { get; set; }
        public DbSet<ConfirmCode> ConfirmCodes { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Options> Options { get; set; }
    }
}
