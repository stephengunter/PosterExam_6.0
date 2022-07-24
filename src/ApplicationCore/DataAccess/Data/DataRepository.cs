using Infrastructure.DataAccess;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.DataAccess
{
	
	public interface IDataRepository<T> : IRepository<T>, IAsyncRepository<T> where T : BaseEntity, IAggregateRoot
	{

	}
	
	public class DataRepository<T> : EfRepository<T>, IDataRepository<T> where T : BaseEntity, IAggregateRoot
	{
		public DataRepository(DataContext context) : base(context)
		{

		}
	}
}
