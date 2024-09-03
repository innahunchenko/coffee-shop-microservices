namespace Catalog.Domain.Models.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public decimal Price { get; set; }
    }
}
