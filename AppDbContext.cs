using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Plus.Moq
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions options) : base(options)
		{
		}

		public virtual DbSet<Entity> Entities { get; set; }
	}
}
