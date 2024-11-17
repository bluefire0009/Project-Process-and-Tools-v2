using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CargoHub.Filters;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

using System.Diagnostics.CodeAnalysis;

namespace TestProject1
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ManagerFilterTest
    {
        private IApiKeyValidationInterface _apiKeyValidationService;

        [TestInitialize]
        public void SetUp()
        {
            // Initialize 
            _apiKeyValidationService = new InMemoryApiKeyValidationService();
        }

        [TestMethod]
        // valid Api
        public async Task TestManagerFilter_Allowed()
        {
            // Arrange
            var filterContext = CreateContextWithApiKey("floor_manager_key");
            var filter = new ManagerOnlyFilter(_apiKeyValidationService);

            // Act
            await filter.OnActionExecutionAsync(filterContext, () => Task.FromResult<ActionExecutedContext>(null));

            // Assert
            
            Assert.IsNull(filterContext.Result);
        }

        [TestMethod]
        //Invalid api
        public async Task TestManagerFilter_Unauthorized()
        {
            // Arrange
            var filterContext = CreateContextWithApiKey("invalid_key");
            var filter = new ManagerOnlyFilter(_apiKeyValidationService);

            // Act
            await filter.OnActionExecutionAsync(filterContext, () => Task.FromResult<ActionExecutedContext>(null));

            // Assert
            Assert.IsInstanceOfType(filterContext.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        // User Key
        public async Task TestManagerFilter_WrongKey()
        {
            // Arrange
            var filterContext = CreateContextWithApiKey("user_key");
            var filter = new ManagerOnlyFilter(_apiKeyValidationService);

            // Act
            await filter.OnActionExecutionAsync(filterContext, () => Task.FromResult<ActionExecutedContext>(null));

            // Assert
            Assert.IsInstanceOfType(filterContext.Result, typeof(UnauthorizedResult));
        }
        [TestMethod]
        //Admin Key
        public async Task TestManagerFilter_Manager_ManagerKey()
        {
            // Arrange
            var filterContext = CreateContextWithApiKey("admin_key");
            var filter = new ManagerOnlyFilter(_apiKeyValidationService);

            // Act
            await filter.OnActionExecutionAsync(filterContext, () => Task.FromResult<ActionExecutedContext>(null));

             // Assert
            Assert.IsNull(filterContext.Result);
        }
        [TestMethod]
        //Warehouse Key
        public async Task TestManagerFilter_Manager_WarehouseKey()
        {
            // Arrange
            var filterContext = CreateContextWithApiKey("warehouse_manager_key");
            var filter = new ManagerOnlyFilter(_apiKeyValidationService);

            // Act
            await filter.OnActionExecutionAsync(filterContext, () => Task.FromResult<ActionExecutedContext>(null));

             // Assert
            Assert.IsNull(filterContext.Result);
        }

       //Create context
        private ActionExecutingContext CreateContextWithApiKey(string apiKey)
        {
          
            var httpContext = new DefaultHttpContext();

            // Add the API key to the request headers
            httpContext.Request.Headers["Api-Key"] = apiKey;

           
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new ActionExecutingContext(actionContext, new IFilterMetadata[] { }, new Dictionary<string, object>(), new object());
        }
    }

    
    public class InMemoryApiKeyValidationService : IApiKeyValidationInterface
    {
        private readonly Dictionary<string, string> _apiKeys;

        public InMemoryApiKeyValidationService()
        {
            // Api keys
            _apiKeys = new Dictionary<string, string>
            {
                { "admin_key", "admin"},
                { "user_key", "user" },
                { "floor_manager_key", "floor_manager" },
                {"warehouse_manager_key", "warehouse_manager"}
            };
        }

        // Check if the API key is valid and has the Manager type
        public async Task<bool> IsValidApiKeyAsync(string apiKey)
        {// List of valid API key types
            List<string> validKeyTypes = new List<string> { "floor_manager", "warehouse_manager", "admin" };
          
            await Task.CompletedTask;

            return _apiKeys.ContainsKey(apiKey) && validKeyTypes.Contains(_apiKeys[apiKey]);

            

           
        }
    }
}