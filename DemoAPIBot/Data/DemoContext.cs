using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using DemoAPIBot.Models;
using TelegramClientServer.Models;

namespace DemoAPIBot.Data
{
    public class DemoContext : DbContext
    {

        public DemoContext(DbContextOptions<DemoContext> options) : base(options)
        {
        }

        public DemoContext()
        {
        }

        public DbSet<Sub> Subs { get; set; } //se faccio public DbSet <Sub>? Users { get; set; } lo dichiaro nullable!
        public DbSet<Macchina> Machines { get; set; }
        public DbSet<SubMachine> SubMachines { get; set; }
        public DbSet<InProgressSub> InProgressSubs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet <Message> Messages { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot config = builder.Build();

            optionsBuilder.UseSqlite(config.GetConnectionString("mainDB"));
        }
    }
}
