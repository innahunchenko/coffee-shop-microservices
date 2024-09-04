using System.Data;
using Dapper;
using Catalog.Domain.Models;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Repositories.Products;
using Catalog.Application;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnection dbConnection;
    private readonly IAppDbContext context;

    public ProductRepository(IDbConnection dbConnection, IAppDbContext context)
    {
        this.dbConnection = dbConnection;
        this.context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(PaginationParameters paginationParameters)
    {
        var query = @"
            SELECT p.*
            FROM Products AS p";

        var products = await ExecuteProductQueryAsync(query, paginationParameters);
        await LoadCategoriesForProductsAsync(products);

        return products;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category, PaginationParameters paginationParameters)
    {
        var query = @"
            SELECT p.*
            FROM Products AS p
            JOIN Categories AS c ON p.CategoryId = c.Id
            JOIN Categories AS parentC ON c.ParentCategoryId = parentC.Id
            WHERE LOWER(parentC.Name) = LOWER(@Category)";

        var products = await ExecuteProductQueryAsync(query, paginationParameters, new { Category = category });
        await LoadCategoriesForProductsAsync(products);

        return products;
    }

    public async Task<IEnumerable<Product>> GetProductsBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters)
    {
        var query = @"
            SELECT p.*
            FROM Products AS p
            JOIN Categories AS c ON p.CategoryId = c.Id
            WHERE LOWER(c.Name) = LOWER(@Subcategory)";

        var products = await ExecuteProductQueryAsync(query, paginationParameters, new { Subcategory = subcategory });
        await LoadCategoriesForProductsAsync(products);

        return products;
    }

    public async Task<IEnumerable<Product>> GetProductsByProductNameAsync(string productName, PaginationParameters paginationParameters)
    {
        var query = @"
            SELECT p.*
            FROM Products AS p
            WHERE p.Name LIKE @ProductName";

        var products = await ExecuteProductQueryAsync(query, paginationParameters, new { ProductName = $"%{productName.Replace("_", "[_]")}%" });
        await LoadCategoriesForProductsAsync(products);

        return products;
    }

    public async Task<int> GetAllProductsTotalCountAsync()
    {
        var query = @"
            SELECT COUNT(*)
            FROM Products AS p";

        return await ExecuteCountQueryAsync(query);
    }

    public async Task<int> GetCategoryProductsTotalCountAsync(string category)
    {
        var query = @"
            SELECT COUNT(*)
            FROM Products AS p
            JOIN Categories AS c ON p.CategoryId = c.Id
            JOIN Categories AS parentC ON c.ParentCategoryId = parentC.Id
            WHERE LOWER(parentC.Name) = LOWER(@Category)";

        return await ExecuteCountQueryAsync(query, new { Category = category });
    }

    public async Task<int> GetSubcategoryProductsTotalCountAsync(string subcategory)
    {
        var query = @"
            SELECT COUNT(*)
            FROM Products AS p
            JOIN Categories AS c ON p.CategoryId = c.Id
            WHERE LOWER(c.Name) = LOWER(@Subcategory)";

        return await ExecuteCountQueryAsync(query, new { Subcategory = subcategory });
    }

    public async Task<int> GetProductNameTotalCountAsync(string productName)
    {
        var query = @"
            SELECT COUNT(*)
            FROM Products AS p
            WHERE p.Name LIKE @ProductName";

        return await ExecuteCountQueryAsync(query, new { ProductName = $"%{productName.Replace("_", "[_]")}%" });
    }

    private async Task<IEnumerable<Product>> ExecuteProductQueryAsync(string query, PaginationParameters paginationParameters, object? parameters = null)
    {
        var paginatedQuery = $"{query} ORDER BY p.Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        var queryParameters = new DynamicParameters(parameters);
        queryParameters.Add("@Offset", (paginationParameters.PageNumber - 1) * paginationParameters.PageSize);
        queryParameters.Add("@PageSize", paginationParameters.PageSize);

        return await dbConnection.QueryAsync<Product>(paginatedQuery, queryParameters);
    }

    private async Task<int> ExecuteCountQueryAsync(string query, object? parameters = null)
    {
        var queryParameters = new DynamicParameters(parameters);
        return await dbConnection.ExecuteScalarAsync<int>(query, queryParameters);
    }

    private async Task LoadCategoriesForProductsAsync(IEnumerable<Product> products)
    {
        if (products == null || products.Count() == 0)
            return;

        var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await context.Categories
            .Include(c => c.ParentCategory)
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync();

        foreach (var product in products)
        {
            product.Category = categories.FirstOrDefault(c => c.Id == product.CategoryId)!;
        }
    }
}
