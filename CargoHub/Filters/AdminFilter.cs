using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class AdminOnly : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext actionContext)
    {
         var admin = actionContext.HttpContext.Session.GetString("AdminStatus");
         //Full authorization
         var AdminKey = "a1b2c3d4e5";
         
         var ApiKey = actionContext.HttpContext.Session.GetString("ApiKey");


         if(admin!= "Admin" || ApiKey!= AdminKey)
         {
          Console.WriteLine(admin, ApiKey);
          actionContext.Result = new UnauthorizedResult();
         }

       
        
    }

}