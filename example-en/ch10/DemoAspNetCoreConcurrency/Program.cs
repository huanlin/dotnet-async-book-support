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
        DemoAspNetCoreConcurrency is running.

        Try these endpoints:
        - GET /Product/1
        - GET /Product/sequential/1
        - GET /Product/404
        - GET /Product/sequential/404

        The concurrent endpoint starts both I/O operations first and then awaits Task.WhenAll.
        The sequential endpoint awaits each operation one by one.
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
            "Concurrent endpoint started for product {ProductId}.",
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
                "Product not found.",
                "The stock API had already been started because this endpoint demonstrates concurrent I/O."));
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
            "Sequential endpoint started for product {ProductId}.",
            id);

        Product? product =
            await productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            stopwatch.Stop();
            return NotFound(new ErrorResponse(
                "Product not found.",
                "The stock API was not called because this endpoint waits in stages."));
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
            "Database query started for product {ProductId}.",
            id);

        await Task.Delay(1000, cancellationToken);

        if (id == 404)
        {
            logger.LogInformation(
                "Database query completed: product {ProductId} was not found.",
                id);
            return null;
        }

        logger.LogInformation(
            "Database query completed for product {ProductId}.",
            id);

        return new Product(id, $"Product-{id}");
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
            "Stock API call started for product {ProductId}.",
            id);

        await Task.Delay(2000, cancellationToken);

        logger.LogInformation(
            "Stock API call completed for product {ProductId}.",
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
