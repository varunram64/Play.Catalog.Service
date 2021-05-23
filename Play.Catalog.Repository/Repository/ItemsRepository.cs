using MongoDB.Driver;
using Play.Catalog.Entities;
using Play.Catalog.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Play.Catalog.Repository
{
    public class ItemsRepository : IItemsRepository
    {
        private const string CollectionName = "Items";
        private readonly IMongoCollection<Items> dbCollection;
        private readonly FilterDefinitionBuilder<Items> filterBuilder = Builders<Items>.Filter;

        public ItemsRepository(IMongoDatabase database)
        {
            dbCollection = database.GetCollection<Items>(CollectionName);
        }

        public async Task<IReadOnlyCollection<Items>> GetAll()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<Items> Get(Guid id)
        {
            FilterDefinition<Items> filter = filterBuilder.Eq(x => x.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task Create(Items entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await dbCollection.InsertOneAsync(entity);
        }

        public async Task Update(Items entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            FilterDefinition<Items> filter = filterBuilder.Eq(x => x.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task Delete(Guid id)
        {
            FilterDefinition<Items> filter = filterBuilder.Eq(x => x.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}
