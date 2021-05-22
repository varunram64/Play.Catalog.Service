using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Play.Catalog.Service.Controllers
{
    [Route("items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDTO> items = new() 
        { 
            new ItemDTO(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
            new ItemDTO(Guid.NewGuid(), "Antidode", "Cures poison", 7, DateTimeOffset.UtcNow),
            new ItemDTO(Guid.NewGuid(), "Bonze Sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow),
        };

        [HttpGet]
        public IEnumerable<ItemDTO> Get()
        {
            return items;
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDTO> GetById(Guid id)
        {
            var item = items.FirstOrDefault(x => x.id == id);

            if (item == null)
                return NotFound();

            return item;
        }

        [HttpPost]
        public ActionResult<ItemDTO> Post(CreateItemDTO createItemDTO)
        {
            var item = new ItemDTO(Guid.NewGuid(), createItemDTO.Name, createItemDTO.Description, createItemDTO.Price, DateTimeOffset.UtcNow);
            items.Add(item);

            return CreatedAtAction(nameof(GetById), new { id = item.id }, item);
        }

        [HttpPut]
        public IActionResult Put(Guid id, UpdateItemDTO updateItemDTO)
        {
            var existingItem = items.FirstOrDefault(x => x.id == id);

            if (existingItem == null)
                return NotFound();

            var updatedItem = existingItem with
            {
                Name = updateItemDTO.Name,
                Description = updateItemDTO.Description,
                Price = updateItemDTO.Price
            };

            int i = items.FindIndex(x => x.id == id);
            items[i] = updatedItem;

            return NoContent();
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            int i = items.FindIndex(x => x.id == id);
            if (i < 0)
                return NotFound();
            items.RemoveAt(i);
            return NoContent();
        }
    }
}
