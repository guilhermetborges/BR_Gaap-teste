using Microsoft.EntityFrameworkCore;
using BRGBackend.Models;

namespace BRGBackend.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<Todo> Todos { get; set; }
	}
}
