using Play.Catalog.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Play.Catalog.IRepository
{
    public interface IItemsRepository
    {
        Task<IReadOnlyCollection<Items>> GetAll();

        Task<Items> Get(Guid id);

        Task Create(Items entity);

        Task Update(Items entity);

        Task Delete(Guid id);
    }
}
