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
                ORDER BY CASE 
                            WHEN c.Name = 'Coffee' THEN 0 
                            ELSE 1 
                         END, 
                         c.Name, sc.Name";

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

        public async Task<CategoryDto> GetCategoryByName(string name)
        {
            var sql = $@"
                SELECT  c.Name  AS CategoryName, 
                        sc.Name AS SubcategoryName
                FROM    Categories AS c
                LEFT JOIN Categories AS sc 
                    ON c.Id = sc.ParentCategoryId
                WHERE   c.Name = @Name";

            var categoryResult = await dbConnection.QueryAsync<CategoryDto, string, CategoryDto>(
                sql,
                (category, subcategoryName) =>
                {
                    if (!string.IsNullOrEmpty(subcategoryName))
                    {
                        category.Subcategories.Add(subcategoryName);
                    }
                    return category;
                },
                new { Name = name },
                splitOn: "SubcategoryName" 
            );

            var category = categoryResult.Single();
            return category;
        }

        public async Task<Guid> AddCategoryAsync(string name, string? parentCategoryName)
        {
            var id = Guid.NewGuid();

            Guid? parentCategoryId = null;
            if (!string.IsNullOrEmpty(parentCategoryName))
            {
                var findParentSql = @"
                    SELECT Id 
                    FROM Categories 
                    WHERE Name = @Name";

                parentCategoryId = await dbConnection.QueryFirstOrDefaultAsync<Guid?>(findParentSql, new { Name = parentCategoryName });

                if (parentCategoryId == null)
                {
                    throw new Exception($"Parent category with name '{parentCategoryName}' not found.");
                }
            }

            var sql = @"
                INSERT INTO Categories (Id, Name, ParentCategoryId)
                VALUES (@Id, @Name, @ParentCategoryId)";

            await dbConnection.ExecuteAsync(sql, new
            {
                Id = id,
                Name = name,
                ParentCategoryId = parentCategoryId
            });

            return id;
        }

        public async Task UpdateCategoryAsync(string oldName, string newName)
        {
            var sql = @"
                UPDATE Categories
                SET Name = @NewName
                WHERE Name = @OldName";

            await dbConnection.ExecuteAsync(sql, new { OldName = oldName, NewName = newName });
        }


        public async Task DeleteCategoryAsync(string categoryName)
        {
            var sql = @"
                DELETE FROM Categories
                WHERE Name = @Name
                OR ParentCategoryId = (
                    SELECT Id FROM Categories WHERE Name = @Name
                )";

            await dbConnection.ExecuteAsync(sql, new { Name = categoryName });
        }
    }
}
