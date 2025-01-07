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
    public class AdminFilterTest
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
        public async Task TestAdminFilter_Allowed()
        {
            // Arrange
            var filterContext = CreateContextWithApiKey("admin_key");
            var filter = new AdminOnlyFilter(_apiKeyValidationService);

            // Act
            await filter.OnActionExecutionAsync(filterContext, () => Task.FromResult<ActionExecutedContext>(null));

            // Assert
            Assert.IsNull(filterContext.Result);
        }

        [TestMethod]
        //Invalid api
        public async Task TestAdminFilter_Unauthorized()
        {
            // Arrange
            var filterContext = CreateContextWithApiKey("invalid_key");
            var filter = new AdminOnlyFilter(_apiKeyValidationService);

            // Act
            await filter.OnActionExecutionAsync(filterContext, () => Task.FromResult<ActionExecutedContext>(null));

            // Assert
            Assert.IsInstanceOfType(filterContext.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        //wrong user with user api
        public async Task TestAdminFilter_WrongKey()
        {
            // Arrange
            var filterContext = CreateContextWithApiKey("user_key");
            var filter = new AdminOnlyFilter(_apiKeyValidationService);

            // Act
            await filter.OnActionExecutionAsync(filterContext, () => Task.FromResult<ActionExecutedContext>(null));

            // Assert
            Assert.IsInstanceOfType(filterContext.Result, typeof(UnauthorizedResult));
        }
        [TestMethod]
        //wrong user with Manager key
        public async Task TestAdminFilter_Admin_ManagerKey()
        {
            // Arrange
            var filterContext = CreateContextWithApiKey("floor_manager_key");
            var filter = new AdminOnlyFilter(_apiKeyValidationService);

            // Act
            await filter.OnActionExecutionAsync(filterContext, () => Task.FromResult<ActionExecutedContext>(null));

            // Assert
            Assert.IsInstanceOfType(filterContext.Result, typeof(UnauthorizedResult));
        }
        [TestMethod]
        //wrong user with WAREHOUSE key
        public async Task TestAdminFilter_Admin_WarehouseKey()
        {
            // Arrange
            var filterContext = CreateContextWithApiKey("warehouse_manager_key");
            var filter = new AdminOnlyFilter(_apiKeyValidationService);

            // Act
            await filter.OnActionExecutionAsync(filterContext, () => Task.FromResult<ActionExecutedContext>(null));

            // Assert
            Assert.IsInstanceOfType(filterContext.Result, typeof(UnauthorizedResult));
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
                { "admin_key", "admin" },
                { "user_key", "user" },
                { "floor_manager", "floor_manager_key" },
                {"warehouse_manager_key", "warehouse_manager"}
            };
        }

        // Check if the API key is valid and has the admin type
        public async Task<bool> IsValidApiKeyAsync(string apiKey)
        {
          
            await Task.CompletedTask;

            return _apiKeys.ContainsKey(apiKey) && _apiKeys[apiKey] == "admin";
        }
    }
}