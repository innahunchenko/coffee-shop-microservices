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

        public async Task UpdateCategoryAsync(string oldName, string newName, string? newParentCategoryName = null)
        {
            var updateCategorySql = @"
                    UPDATE c
                    SET 
                        c.Name = @NewName,
                        c.ParentCategoryId = COALESCE(parent.Id, c.ParentCategoryId)
                    FROM Categories c
                    LEFT JOIN Categories parent 
                        ON parent.Name = @NewParentCategoryName
                    WHERE c.Name = @OldName";

            var rowsAffected = await dbConnection.ExecuteAsync(updateCategorySql, new { OldName = oldName, NewName = newName, NewParentCategoryName = newParentCategoryName });

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Category '{oldName}' not found or parent category '{newParentCategoryName}' is invalid.");
            }
        }

        public async Task DeleteCategoryAsync(string categoryName)
        {
            var sql = @"
                    DELETE c
                    FROM Categories c
                    LEFT JOIN Categories parent 
                        ON c.ParentCategoryId = parent.Id
                    WHERE c.Name = @Name
                    OR (parent.Name = @Name AND parent.Id = c.ParentCategoryId)";

            await dbConnection.ExecuteAsync(sql, new { Name = categoryName });
        }
    }
}
