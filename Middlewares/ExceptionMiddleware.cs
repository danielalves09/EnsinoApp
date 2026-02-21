namespace EnsinoApp.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro não tratado | Path: {Path} | User: {User}",
                context.Request.Path,
                context.User?.Identity?.Name ?? "Anônimo");

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                sucesso = false,
                mensagem = "Ocorreu um erro interno. Tente novamente mais tarde."
            });
        }
    }
}
