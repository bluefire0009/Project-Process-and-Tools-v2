
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

using System.Diagnostics.CodeAnalysis;
namespace CargoHub.Filters
{
    
    public class WarehouseManagerFilter : IAsyncActionFilter
    {
        private const string ApiKeyHeader = "Api-Key"; // Header
        private readonly IApiKeyValidationInterface _apiKeyValidationService;

        
        public WarehouseManagerFilter(IApiKeyValidationInterface apiKeyValidationService)
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
            if (!await _apiKeyValidationService.IsValidApiKeyAsync(apiKey))
            {
                context.Result = new UnauthorizedResult(); // Invalid API key, return 401 Unauthorized
                return;
            }

            // Proceed with program if the key is valid
            await next();
        }
    }

    // Define the WareHouseattribute as a TypeFilter that uses the WarehouseManagerOnlyFilter
    public class WarehouseManagerAttribute : TypeFilterAttribute
    {
       
        public WarehouseManagerAttribute() : base(typeof(WarehouseManagerFilter)) { }
    }
}