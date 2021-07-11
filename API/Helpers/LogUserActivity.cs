using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //ActionExecutingContext context - before
            //ActionExecutionDelegate next - after

            //context of the action has been executed
            var resultContext = await next();

            //check if the user authenticated
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            //update last active property

            var userId = resultContext.HttpContext.User.GetUserId();
            //service locator pattern
            //var repository = resultContext.HttpContext.RequestServices.GetService(typeof(IUserRepository));
            var repository = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();

            var user = await repository.GetUserByIdAsync(userId);
            user.LastActive = DateTime.Now;
            await repository.SaveAllAsync();


        }
    }
}