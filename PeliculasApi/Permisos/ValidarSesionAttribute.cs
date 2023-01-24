using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PeliculasApi.Permisos
{
    public class ValidarSesionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Session.GetString("usuario") == null)
            {
                context.Result = new RedirectResult("~/Acceso/Login");
            }

            base.OnActionExecuting(context);
        }
    }
}
