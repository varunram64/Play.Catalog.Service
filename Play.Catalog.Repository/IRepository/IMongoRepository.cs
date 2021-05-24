using Play.Catalog.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Play.Catalog.IRepository
{
    public interface IMongoRepository<T> where T : IEntity
    {
        Task<IReadOnlyCollection<T>> GetAll();

        Task<T> Get(Guid id);

        Task Create(T entity);

        Task Update(T entity);

        Task Delete(Guid id);
    }
}
