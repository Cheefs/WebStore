namespace WebStore.WebAPI.Infrastructure.Middlewares;

public class ExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            HandleExeption(context, ex);
            throw;
        }
    }

    private void HandleExeption(HttpContext context, Exception error)
    {
        _logger.LogError(error, $"Ошибка в процессе обработки запроса к {context.Request.Path}");
    }
}
