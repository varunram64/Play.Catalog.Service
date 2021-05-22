using Play.Catalog.Entities;
using Play.Catalog.Service.DTOs;

namespace Play.Catalog.Service
{
    public static class Extensions
    {
        public static ItemDTO AsDTO(this Items item)
        {
            return new ItemDTO(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}
