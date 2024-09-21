using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Repositories.Categories;
using Dapper;
using System.Data;

namespace Catalog.Infrastructure.Repositories.Categories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbConnection dbConnection;

        public CategoryRepository(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var sql = $@"
                SELECT  c.Name  AS CategoryName, 
                        sc.Name AS SubcategoryName
                FROM    Categories AS c
                JOIN    Categories AS sc 
                        ON  c.Id = sc.ParentCategoryId
                WHERE   c.ParentCategoryId IS NULL
                ORDER
                BY      c.Name, sc.Name";

            var categoryDictionary = new Dictionary<string, CategoryDto>();

            var rows = await dbConnection.QueryAsync<string, string, (string CategoryName, string SubcategoryName)>(
                sql,
                (categoryName, subcategoryName) => (CategoryName: categoryName, SubcategoryName: subcategoryName),
                splitOn: "SubcategoryName"
            );

            foreach (var (CategoryName, SubcategoryName) in rows)
            {
                if (!categoryDictionary.TryGetValue(CategoryName, out var categoryEntry))
                {
                    categoryEntry = new CategoryDto
                    {
                        Name = CategoryName,
                        Subcategories = new List<string>()
                    };
                    categoryDictionary[CategoryName] = categoryEntry;
                }

                if (!string.IsNullOrEmpty(SubcategoryName))
                {
                    categoryEntry.Subcategories.Add(SubcategoryName);
                }
            }

            return categoryDictionary.Values.ToList();
        }
    }
}
