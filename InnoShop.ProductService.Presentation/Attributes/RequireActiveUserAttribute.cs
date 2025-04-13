using InnoShop.ProductService.Application.Interfaces.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InnoShop.ProductService.Presentation.Attributes
{
    public class RequireActiveUserAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirst("userId");

            var userId = Guid.Parse(userIdClaim.Value);
            Console.WriteLine("АЙДИ " + userId);
            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserServiceClient>();
            var isActive = await userService.IsUserActiveAsync(userId, context.HttpContext.RequestAborted);

            if (!isActive)
            {
                throw new Exception("User is not active");
            }
        }
    }
}
