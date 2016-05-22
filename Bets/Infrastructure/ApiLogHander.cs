using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bets.Data;

namespace Bets.Infrastructure
{
    /// <summary>
    /// Logs all requests and responses to trace
    /// </summary>
    public class ApiLogHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith((task) =>
            {
                HttpResponseMessage response = task.Result;

                Trace.WriteLine(request.ToString(), "LogHandler Request");
                Trace.WriteLine(response.ToString(), "LogHandler Response");

                //try to log response value
                try
                {
                    if (response.Content is System.Net.Http.ObjectContent)
                        Trace.WriteLine(((System.Net.Http.ObjectContent)response.Content).Value.ToJSON(), "LogHandler Response Value");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error serializing response value " + ex.Message, "LogHandler");

                    //fall back to response as a whole
                    try
                    {
                        Trace.WriteLine(response.Content.ToJSON(), "LogHandler Response Data");
                    }
                    catch (Exception ex2)
                    {
                        Trace.WriteLine("Error serializing response " + ex2.Message, "LogHandler");
                    }
                }

                return response;
            });
        }
    }
}