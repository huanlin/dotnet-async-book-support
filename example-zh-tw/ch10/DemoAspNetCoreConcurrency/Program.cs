using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProductRepository, FakeProductRepository>();
builder.Services.AddSingleton<IStockApiClient, FakeStockApiClient>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapGet(
    "/",
    () => Results.Text(
        """
        DemoAspNetCoreConcurrency 已啟動。

        可測試的端點：
        - GET /Product/1
        - GET /Product/sequential/1
        - GET /Product/404
        - GET /Product/sequential/404

        concurrent 端點會先同時啟動兩個 I/O，再用 Task.WhenAll 等待。
        sequential 端點則是逐一 await。
        """,
        "text/plain"));

app.MapControllers();
app.Run();

[ApiController]
[Route("[controller]")]
public sealed class ProductController(
    IProductRepository productRepository,
    IStockApiClient stockApiClient,
    ILogger<ProductController> logger) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDetailsResponse>>
        GetProductDetailsAsync(
            int id,
            CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation(
            "併發端點開始處理產品 {ProductId}。",
            id);

        Task<Product?> productTask =
            productRepository.GetByIdAsync(id, cancellationToken);
        Task<int> stockTask =
            stockApiClient.GetStockCountAsync(id, cancellationToken);

        Task allTasks = Task.WhenAll(productTask, stockTask);
        await allTasks;

        Product? product = await productTask;
        if (product is null)
        {
            stopwatch.Stop();
            return NotFound(new ErrorResponse(
                "查無產品。",
                "這個端點是為了示範 concurrent I/O，所以庫存 API 已經先被啟動。"));
        }

        int stockCount = await stockTask;
        stopwatch.Stop();

        return Ok(new ProductDetailsResponse(
            ProductId: product.Id,
            Name: product.Name,
            StockCount: stockCount,
            ExecutionModel: "concurrent",
            ElapsedMilliseconds: stopwatch.ElapsedMilliseconds));
    }

    [HttpGet("sequential/{id:int}")]
    public async Task<ActionResult<ProductDetailsResponse>>
        GetProductDetailsSequentialAsync(
            int id,
            CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation(
            "循序端點開始處理產品 {ProductId}。",
            id);

        Product? product =
            await productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            stopwatch.Stop();
            return NotFound(new ErrorResponse(
                "查無產品。",
                "這個端點採分階段等待，因此不會呼叫庫存 API。"));
        }

        int stockCount =
            await stockApiClient.GetStockCountAsync(id, cancellationToken);

        stopwatch.Stop();

        return Ok(new ProductDetailsResponse(
            ProductId: product.Id,
            Name: product.Name,
            StockCount: stockCount,
            ExecutionModel: "sequential",
            ElapsedMilliseconds: stopwatch.ElapsedMilliseconds));
    }
}

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}

public interface IStockApiClient
{
    Task<int> GetStockCountAsync(
        int id,
        CancellationToken cancellationToken = default);
}

public sealed class FakeProductRepository(
    ILogger<FakeProductRepository> logger) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "資料庫查詢開始：產品 {ProductId}。",
            id);

        await Task.Delay(1000, cancellationToken);

        if (id == 404)
        {
            logger.LogInformation(
                "資料庫查詢完成：產品 {ProductId} 不存在。",
                id);
            return null;
        }

        logger.LogInformation(
            "資料庫查詢完成：產品 {ProductId}。",
            id);

        return new Product(id, $"產品-{id}");
    }
}

public sealed class FakeStockApiClient(
    ILogger<FakeStockApiClient> logger) : IStockApiClient
{
    public async Task<int> GetStockCountAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "庫存 API 呼叫開始：產品 {ProductId}。",
            id);

        await Task.Delay(2000, cancellationToken);

        logger.LogInformation(
            "庫存 API 呼叫完成：產品 {ProductId}。",
            id);

        return 20 + id;
    }
}

public sealed record Product(int Id, string Name);

public sealed record ProductDetailsResponse(
    int ProductId,
    string Name,
    int StockCount,
    string ExecutionModel,
    long ElapsedMilliseconds);

public sealed record ErrorResponse(
    string Message,
    string Note);
