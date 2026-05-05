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
        DemoAspNetCoreConcurrency は実行中です。

        次のエンドポイントを試してください:
        - GET /Product/1
        - GET /Product/sequential/1
        - GET /Product/404
        - GET /Product/sequential/404

        concurrent エンドポイントは、先に 2 つの I/O 操作を開始してから Task.WhenAll を await します。
        sequential エンドポイントは、各操作を 1 つずつ await します。
        """,
        "text/plain; charset=utf-8"));

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
            "product {ProductId} の concurrent エンドポイントを開始しました。",
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
                "商品が見つかりません。",
                "このエンドポイントは並行 I/O を示すため、在庫 API はすでに開始されています。"));
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
            "product {ProductId} の sequential エンドポイントを開始しました。",
            id);

        Product? product =
            await productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            stopwatch.Stop();
            return NotFound(new ErrorResponse(
                "商品が見つかりません。",
                "このエンドポイントは段階的に待機するため、在庫 API は呼び出されませんでした。"));
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
            "product {ProductId} のデータベース クエリを開始しました。",
            id);

        await Task.Delay(1000, cancellationToken);

        if (id == 404)
        {
            logger.LogInformation(
                "データベース クエリが完了しました: product {ProductId} は見つかりませんでした。",
                id);
            return null;
        }

        logger.LogInformation(
            "product {ProductId} のデータベース クエリが完了しました。",
            id);

        return new Product(id, $"商品-{id}");
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
            "product {ProductId} の在庫 API 呼び出しを開始しました。",
            id);

        await Task.Delay(2000, cancellationToken);

        logger.LogInformation(
            "product {ProductId} の在庫 API 呼び出しが完了しました。",
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
