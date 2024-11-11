using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class AdminOnly : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext actionContext)
    {
         var admin = actionContext.HttpContext.Session.GetString("AdminStatus");

         if(admin!= "Admin")
         {
          Console.WriteLine(admin);
          actionContext.Result = new UnauthorizedResult();
         }

       
        
    }

}