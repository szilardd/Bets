using System;
using System.Web.Http.Filters;

namespace Bets.Infrastructure
{
    public class ApiLogFilter : ActionFilterAttribute, IActionFilter
    {
        private void Trace()
        {
            System.Diagnostics.Trace.WriteLine(new String('-', 150));
            System.Diagnostics.Trace.WriteLine("");
        }

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            Trace();
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            Trace();
            base.OnActionExecuted(actionExecutedContext);
        }

        public override System.Threading.Tasks.Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, System.Threading.CancellationToken cancellationToken)
        {
            Trace();
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

        public override System.Threading.Tasks.Task OnActionExecutingAsync(System.Web.Http.Controllers.HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            Trace();
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}