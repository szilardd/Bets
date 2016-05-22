using System;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;
using Bets.Data;

namespace Bets
{
    /// <summary>
    /// Returns custom message when an exception occurs in an api call
    /// </summary>
    public class ApiGlobalExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            context.Result = new ResponseMessageResult
            (
                context.Request.CreateResponse
                (
                    HttpStatusCode.OK,
                    new ActionStatus
                    {
                        Success = false,
                        Message = "A server error occurred. Please try again!"
                    }
                )
            );
        }
    }
}
