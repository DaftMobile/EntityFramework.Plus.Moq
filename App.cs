using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace EntityFramework.Plus.Moq
{
	public class App
	{
		private readonly AppDbContext _appDbContext;

		public App(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}

		public async Task DeleteEntities()
		{
			await _appDbContext.Entities.DeleteAsync();
		}
	}
}
