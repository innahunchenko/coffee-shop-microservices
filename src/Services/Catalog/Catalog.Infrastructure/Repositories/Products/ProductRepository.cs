using System.Data;
using Dapper;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Repositories.Products;
using Catalog.Domain.Models.Dtos;
using System.Dynamic;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnection dbConnection;

    public ProductRepository(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    private async Task<IEnumerable<ProductDto>> GetProductsAsync(
        string filterCondition, 
        object parameters, 
        PaginationParameters paginationParameters)
    {
        var sql = $@"
        SELECT  CONVERT(VARCHAR(36), 
                p.Id)           AS  {nameof(ProductDto.Id)},  
                p.Name          AS  {nameof(ProductDto.Name)},
                p.Description   AS  {nameof(ProductDto.Description)},
                p.Price         AS  {nameof(ProductDto.Price)},
                CASE 
                    WHEN parentC.Id IS NOT NULL THEN parentC.Name
                    ELSE c.Name
                END             AS  {nameof(ProductDto.CategoryName)},
                CASE 
                    WHEN parentC.Id IS NULL THEN c.Name
                    ELSE ''
                END             AS  {nameof(ProductDto.SubcategoryName)}
        FROM    Products        AS  p
        JOIN    Categories      AS  c 
                ON  p.CategoryId = c.Id
        LEFT 
        JOIN    Categories  AS   parentC 
                ON c.ParentCategoryId = parentC.Id
        {filterCondition}
        ORDER 
        BY      p.Name
        OFFSET  @Offset ROWS 
        FETCH 
        NEXT    @PageSize ROWS ONLY";

        var productDtos = await dbConnection.QueryAsync<ProductDto>(
            sql,
            MergeObjects(parameters, new
            {
                Offset = (paginationParameters.PageNumber - 1) * paginationParameters.PageSize,
                paginationParameters.PageSize
            })
        );

        return productDtos;
    }

    public static object MergeObjects(object first, object second)
    {
        var result = new ExpandoObject() as IDictionary<string, object>;
        foreach (var property in first.GetType().GetProperties())
        {
            result[property.Name] = property.GetValue(first)!;
        }
        foreach (var property in second.GetType().GetProperties())
        {
            result[property.Name] = property.GetValue(second)!;
        }
        return result;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(PaginationParameters paginationParameters)
    {
        return await GetProductsAsync(string.Empty, new { }, paginationParameters);
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category, PaginationParameters paginationParameters)
    {
        return await GetProductsAsync("WHERE LOWER(parentC.Name) = LOWER(@Category)", new { Category = category }, paginationParameters);
    }

    public async Task<IEnumerable<ProductDto>> GetBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters)
    {
        return await GetProductsAsync("WHERE LOWER(c.Name) = LOWER(@Subcategory)", new { Subcategory = subcategory }, paginationParameters);
    }

    public async Task<IEnumerable<ProductDto>> GetByProductNameAsync(string productName, PaginationParameters paginationParameters)
    {
        return await GetProductsAsync("WHERE p.Name LIKE @ProductName", new { ProductName = $"%{productName.Replace("_", "[_]")}%" }, paginationParameters);
    }

    public async Task<int> GetAllTotalCountAsync()
    {
        var query = @"
            SELECT COUNT(*)
            FROM Products";

        return await dbConnection.ExecuteScalarAsync<int>(query);
    }

    public async Task<int> GetTotalCountByCategoryAsync(string category)
    {
        var query = @"
            SELECT COUNT(*)
            FROM Products AS p
            JOIN Categories AS c ON p.CategoryId = c.Id
            JOIN Categories AS parentC ON c.ParentCategoryId = parentC.Id
            WHERE LOWER(parentC.Name) = LOWER(@Category)";

        return await dbConnection.ExecuteScalarAsync<int>(query, new { Category = category });
    }

    public async Task<int> GetTotalCountBySubcategoryAsync(string subcategory)
    {
        var query = @"
            SELECT COUNT(*)
            FROM Products AS p
            JOIN Categories AS c ON p.CategoryId = c.Id
            WHERE LOWER(c.Name) = LOWER(@Subcategory)";

        return await dbConnection.ExecuteScalarAsync<int>(query, new { Subcategory = subcategory });
    }

    public async Task<int> GetTotalCountByNameAsync(string productName)
    {
        var query = @"
            SELECT COUNT(*)
            FROM Products AS p
            WHERE p.Name LIKE @ProductName";

        return await dbConnection.ExecuteScalarAsync<int>(query, new { ProductName = $"%{productName.Replace("_", "[_]")}%" });
    }
}
