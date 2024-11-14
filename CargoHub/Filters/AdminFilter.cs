using CargoHub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using CargoHub.Interface;
using System.Diagnostics.CodeAnalysis;
namespace CargoHub.Filters
{
    //Integration test for admin filter already exsists.
    [ExcludeFromCodeCoverage]
    public class AdminOnlyFilter : IAsyncActionFilter
    {
        private const string ApiKeyHeader = "Api-Key"; // Header
        private readonly IApiKeyValidationInterface _apiKeyValidationService;

        
        public AdminOnlyFilter(IApiKeyValidationInterface apiKeyValidationService)
        {
            _apiKeyValidationService = apiKeyValidationService;
        }

       
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Retrieve the API key from the request header
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeader, out var apiKey))
            {
                context.Result = new UnauthorizedResult(); // Missing API key, return 401 Unauthorized
                return;
            }

            // Using ApiKeyValidate Service
            if (!await _apiKeyValidationService.IsValidAdminApiKeyAsync(apiKey))
            {
                context.Result = new UnauthorizedResult(); // Invalid API key, return 401 Unauthorized
                return;
            }

            // Proceed with program if the key is valid
            await next();
        }
    }

    // Define the AdminOnly attribute as a TypeFilter that uses the AdminOnlyFilter
    public class AdminOnlyAttribute : TypeFilterAttribute
    {
       
        public AdminOnlyAttribute() : base(typeof(AdminOnlyFilter)) { }
    }
}