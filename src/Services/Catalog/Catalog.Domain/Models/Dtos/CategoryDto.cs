namespace Catalog.Domain.Models.Dtos
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public List<string> Subcategories { get; set; }
    }
}
