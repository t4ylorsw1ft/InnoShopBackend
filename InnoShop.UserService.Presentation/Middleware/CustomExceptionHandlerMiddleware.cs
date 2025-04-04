using System.Net;
using System.Text.Json;
using FluentValidation;
using InnoShop.UserService.Application.Common.Exceptions;


namespace InnoShop.UserService.Presentation.Middleware
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomExceptionHandlerMiddleware(RequestDelegate next) =>
            _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            Console.WriteLine(ex);
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;
            switch (ex)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(validationException.Errors);
                    break;
                case NotFoundException:
                    code = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new { error = ex.Message });
                    break;
                case LoginException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { error = ex.Message });
                    break;
                case UnauthorizedAccessException:
                    code = HttpStatusCode.Forbidden;
                    result = JsonSerializer.Serialize(new { error = ex.Message });
                    break;
            }
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            if (result == string.Empty)
            {
                result = JsonSerializer.Serialize(new { error = ex.Message });
            }

            return context.Response.WriteAsync(result);
        }
    }
}
