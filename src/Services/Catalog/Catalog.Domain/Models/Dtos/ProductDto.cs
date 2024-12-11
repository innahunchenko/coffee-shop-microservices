namespace Catalog.Domain.Models.Dtos
{
    public class ProductDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? CategoryName { get; set; }
        public string? SubcategoryName { get; set; }
        public string? Description { get; set; }
        public string? ImagePath {  get; set; }
        public decimal Price { get; set; }
    }
}
