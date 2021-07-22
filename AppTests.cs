using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace EntityFramework.Plus.Moq
{
	public class AppTests
	{
		private readonly Mock<AppDbContext>  _appDbContext;
		private readonly Mock<DbSet<Entity>> _entitiesDbSetMock;

		private readonly App _sut;

		public AppTests()
		{
			var entities = new List<Entity>
			{
				new Entity()
			};

			_entitiesDbSetMock = entities.AsQueryable().BuildMockDbSet();

			_appDbContext = new Mock<AppDbContext>(MockBehavior.Default, new DbContextOptionsBuilder().Options);
			_appDbContext.SetupGet(x => x.Entities).Returns(_entitiesDbSetMock.Object);

			_sut = new App(_appDbContext.Object);
		}

		/// <summary>
		/// This should throw exception with message
		/// <code>Field '_queryCompiler' defined on type 'Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider' is not a field on the target object which is of type 'MockQueryable.EntityFrameworkCore.TestAsyncEnumerableEfCore`1[EntityFramework.Plus.Moq.Entity]'.</code>
		/// because EntityFramework.Plus uses reflection and assumption that used `QueryProvider` is `Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider`, not `MockQueryable.EntityFrameworkCore.TestAsyncEnumerableEfCore`1[EntityFramework.Plus.Moq.Entity]`.
		/// </summary>
		[Fact]
		public async Task DeleteEntities_NotOverridingQueryExpression_ShouldFail()
		{
			await _sut.DeleteEntities();
		}

		/// <summary>
		/// Solution defined here uses fact that this implementation of EntityFramework.Plus is failing fast when given query expression has `.Where([any var] => False` part.
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task DeleteEntities_OverridingQueryExpression_ShouldPass()
		{
			OverrideQueryExpressionOnMockedDbSet(_entitiesDbSetMock);
			await _sut.DeleteEntities();
		}

		private static void OverrideQueryExpressionOnMockedDbSet<T>(Mock<DbSet<T>> mockedDbSet) where T : class
		{
			mockedDbSet.As<IQueryable<T>>().SetupGet(x => x.Expression)
				.Returns(new List<T>().AsQueryable().Where(x => false).Expression);
		}
	}
}
