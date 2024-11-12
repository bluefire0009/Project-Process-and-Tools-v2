using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Diagnostics.CodeAnalysis;



namespace TestProject1
{
    [ExcludeFromCodeCoverage]
    [TestClass]


     // Test Method for Admin-Only filter
    public class AdminFilterTest
    {
       
        [TestInitialize]
        public void setUp()
        {
            
        }

        
        [TestMethod]
        public void TestAdminFilter_Allowed()
        {
            // Arrange
            var filterContext = CreateContextWithSession("Admin", "a1b2c3d4e5");

            var filter = new AdminOnly();

            // Act
            filter.OnAuthorization(filterContext);

            // Assert
            Assert.IsNull(filterContext.Result);
        }

        // Test Method for Unauthorized access
        [TestMethod]
        public void TestAdminFilter_Unauthorized()
        {
            // Arrange
            var filterContext = CreateContextWithSession("User", "f6g7h8i9j0"); 
            var filter = new AdminOnly();

            // Act
            filter.OnAuthorization(filterContext);

            // Assert
            Assert.IsInstanceOfType(filterContext.Result, typeof(UnauthorizedResult));
        }
         // Test Method for Unauthorized access non admin user with admin key
        [TestMethod]
        public void TestAdminFilter_WrongUser_AdminKey()
        {
            // Arrange
            var filterContext = CreateContextWithSession("User", "a1b2c3d4e5"); 
            var filter = new AdminOnly();

            // Act
            filter.OnAuthorization(filterContext);

            // Assert
            Assert.IsInstanceOfType(filterContext.Result, typeof(UnauthorizedResult));
        }
        // Test Method for Unauthorized access
        [TestMethod]
        public void TestAdminFilter_AdminWrongKey_UserKey()
        {
            // Arrange
            var filterContext = CreateContextWithSession("Admin", "f6g7h8i9j0"); 
            var filter = new AdminOnly();

            // Act
            filter.OnAuthorization(filterContext);

            // Assert
            Assert.IsInstanceOfType(filterContext.Result, typeof(UnauthorizedResult));
        }


        // Method to Create FilterContext with a Session
        private AuthorizationFilterContext CreateContextWithSession(string adminStatus, string ApiKey)
        {
            // Create a new HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new InMemorySession();

            // setting "AdminStatus" in the session
            httpContext.Session.SetString("AdminStatus", adminStatus);
            //Setting "ApiKey" in the session
            httpContext.Session.SetString("ApiKey",ApiKey);

            // Create ActionContext with HttpContext
            var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

            // Return AuthorizationFilterContext
            return new AuthorizationFilterContext(actionContext, new IFilterMetadata[] { });
        }
    }

    
    public class InMemorySession : ISession
{
    private readonly Dictionary<string, byte[]> _storage = new Dictionary<string, byte[]>();

    public string Id => Guid.NewGuid().ToString();

    public bool IsAvailable => true;

    // Property for keys in the session
    public IEnumerable<string> Keys => _storage.Keys;

    public void Clear() => _storage.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Remove(string key) => _storage.Remove(key);

    public void Set(string key, byte[] value) => _storage[key] = value;

    public bool TryGetValue(string key, out byte[] value) => _storage.TryGetValue(key, out value);

    public string GetString(string key)
    {
        if (_storage.TryGetValue(key, out byte[] value))
        {
            return Encoding.UTF8.GetString(value);
        }
        return null;
    }

    public void SetString(string key, string value) => Set(key, Encoding.UTF8.GetBytes(value));
}
}