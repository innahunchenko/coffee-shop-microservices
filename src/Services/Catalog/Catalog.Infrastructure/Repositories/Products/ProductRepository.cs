using System.Data;
using Dapper;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Repositories.Products;
using Catalog.Domain.Models.Dtos;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnection dbConnection;

    public ProductRepository(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
    {
        var product = await GetProductsAsync("WHERE p.Id = @ProductId", new Dictionary<string, object> { { "ProductId", productId } }, new PaginationParameters(1, 1));
        return product.SingleOrDefault();
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParameters)
    {
        return await GetProductsAsync(string.Empty, new Dictionary<string, object>(), paginationParameters);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category, PaginationParameters paginationParameters)
    {
        return await GetProductsAsync("WHERE LOWER(parentC.Name) = LOWER(@Category)",
            new Dictionary<string, object> { { "Category", category } }, paginationParameters);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters)
    {
        return await GetProductsAsync("WHERE LOWER(c.Name) = LOWER(@Subcategory)",
            new Dictionary<string, object> { { "Subcategory", subcategory } }, paginationParameters);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByNameAsync(string productName, PaginationParameters paginationParameters)
    {
        return await GetProductsAsync("WHERE p.Name LIKE @ProductName",
            new Dictionary<string, object> { { "ProductName", $"%{productName.Replace("_", "[_]")}%" } }, paginationParameters);
    }

    private async Task<IEnumerable<ProductDto>> GetProductsAsync(string filterCondition,
        Dictionary<string, object> filterParameters, PaginationParameters paginationParameters)
    {
        var sqlQuery = $@"
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

        filterParameters.Add("Offset", (paginationParameters.PageNumber - 1) * paginationParameters.PageSize);
        filterParameters.Add("PageSize", paginationParameters.PageSize);

        var productDtos = await dbConnection.QueryAsync<ProductDto>(
            sqlQuery,
            filterParameters
        );

        return productDtos;
    }

    public async Task<int> GetAllProductsTotalCountAsync()
    {
        var query = @"
            SELECT  COUNT(*)
            FROM    Products";

        return await dbConnection.ExecuteScalarAsync<int>(query);
    }

    public async Task<int> GetProductsTotalCountByCategoryAsync(string category)
    {
        var query = @"
            SELECT  COUNT(*)
            FROM    Products    AS  p
            JOIN    Categories  AS  c 
                    ON  p.CategoryId = c.Id
            JOIN    Categories  AS  parentC 
                    ON  c.ParentCategoryId = parentC.Id
            WHERE   LOWER(parentC.Name) = LOWER(@Category)";

        return await dbConnection.ExecuteScalarAsync<int>(query, new { Category = category });
    }

    public async Task<int> GetProductsTotalCountBySubcategoryAsync(string subcategory)
    {
        var query = @"
            SELECT  COUNT(*)
            FROM    Products    AS  p
            JOIN    Categories  AS  c 
                    ON  p.CategoryId = c.Id
            WHERE   LOWER(c.Name) = LOWER(@Subcategory)";

        return await dbConnection.ExecuteScalarAsync<int>(query, new { Subcategory = subcategory });
    }

    public async Task<int> GetProductsTotalCountByNameAsync(string productName)
    {
        var query = @"
            SELECT  COUNT(*)
            FROM    Products    AS  p
            WHERE   p.Name  LIKE @ProductName";

        return await dbConnection.ExecuteScalarAsync<int>(query, new { ProductName = $"%{productName.Replace("_", "[_]")}%" });
    }
}
