using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Entities;
using Play.Common.IRepository;
using Play.Catalog.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Catalog.Service.Controllers
{
    [Route("items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IMongoRepository<Items> _itemsRepository;

        public ItemsController(IMongoRepository<Items> itemsRepository)
        {
            _itemsRepository = itemsRepository;
        }

        private static readonly List<ItemDTO> items = new()
        {
            new ItemDTO(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
            new ItemDTO(Guid.NewGuid(), "Antidode", "Cures poison", 7, DateTimeOffset.UtcNow),
            new ItemDTO(Guid.NewGuid(), "Bronze Sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow),
        };

        [HttpGet]
        public async Task<IEnumerable<ItemDTO>> Get()
        {
            var items = (await _itemsRepository.GetAll())
                        .Select(x => x.AsDTO());
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetById(Guid id)
        {
            var item = await _itemsRepository.Get(id);

            if (item == null)
                return NotFound();

            return item.AsDTO();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDTO>> Post(CreateItemDTO createItemDTO)
        {
            var item = new Items
            {
                Name = createItemDTO.Name,
                Description = createItemDTO.Description,
                Price = createItemDTO.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _itemsRepository.Create(item);

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid id, UpdateItemDTO updateItemDTO)
        {
            var existingItem = await _itemsRepository.Get(id);

            if (existingItem == null)
                return NotFound();

            existingItem.Name = updateItemDTO.Name;
            existingItem.Description = updateItemDTO.Description;
            existingItem.Price = updateItemDTO.Price;

            await _itemsRepository.Update(existingItem);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingItem = await _itemsRepository.Get(id);
            if (existingItem == null)
                return NotFound();
            await _itemsRepository.Delete(existingItem.Id);
            return NoContent();
        }
    }
}
