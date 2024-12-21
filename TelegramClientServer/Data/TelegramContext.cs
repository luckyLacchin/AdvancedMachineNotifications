using TelegramClientServer.Models;
using Microsoft.EntityFrameworkCore;

namespace TelegramClientServer.Data
{
    public class TelegramContext : DbContext
    {
        public TelegramContext()
        {
        }

        public TelegramContext(DbContextOptions<TelegramContext> options) : base(options)
        {
        }

        public DbSet<Chatter> Chatters { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot config = builder.Build();

            var bho = config.GetConnectionString("mainDB");
            optionsBuilder.UseSqlite(config.GetConnectionString("mainDB"));
        }
    }
}
