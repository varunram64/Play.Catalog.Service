using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Entities;
using Play.Catalog.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Play.Common.IRepository;
using MassTransit;
using static Play.Catalog.Contracts.Constracts;

namespace Play.Catalog.Service.Controllers
{
    [Route("items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IMongoRepository<Items> _itemsRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        //private static int RequestCounter;

        public ItemsController(IMongoRepository<Items> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            _itemsRepository = itemsRepository;
            _publishEndpoint = publishEndpoint;
        }

        //private static readonly List<ItemDTO> items = new()
        //{
        //    new ItemDTO(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
        //    new ItemDTO(Guid.NewGuid(), "Antidode", "Cures poison", 7, DateTimeOffset.UtcNow),
        //    new ItemDTO(Guid.NewGuid(), "Bronze Sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow),
        //};

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDTO>>> Get()
        {
            //RequestCounter++;
            //Console.WriteLine($"Request {RequestCounter} Starting...");

            //if(RequestCounter < 2)
            //{
            //    Console.WriteLine($"Request delaying {RequestCounter} Starting...");
            //    await Task.Delay(TimeSpan.FromSeconds(10));
            //}

            //if (RequestCounter < 4)
            //{
            //    Console.WriteLine($"Request {RequestCounter}: 500(Internal Server Error)");
            //    return StatusCode(500);
            //}

            var items = (await _itemsRepository.GetAll())
                        .Select(x => x.AsDTO());
            //Console.WriteLine($"Request {RequestCounter}: 200(Ok)");
            return Ok(items);
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

            await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

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

            await _publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingItem = await _itemsRepository.Get(id);
            if (existingItem == null)
                return NotFound();

            await _itemsRepository.Delete(existingItem.Id);

            await _publishEndpoint.Publish(new CatalogItemDeleted(existingItem.Id));

            return NoContent();
        }
    }
}
