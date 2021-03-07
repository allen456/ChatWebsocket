using Microsoft.EntityFrameworkCore;

using System.Linq;

using ChatApp.Models;

namespace ChatApp.Data
{
    public class ChatContext : DbContext
    {
        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {
        }

        public DbSet<ChatClient> ChatClients { get; set; }
    }
}
